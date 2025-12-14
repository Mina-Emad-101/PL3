module App

open Browser.Dom
open Models
open Storage

// 1. STATE
// We keep a mutable reference to our library in memory
async {
    let! initial = Storage.loadLibrary()
    let mutable currentLibrary = initial

    // 2. HELPER: Get HTML elements safely
    // This is a wrapper to casting the element to the right type (like HTMLInputElement)
    let getInput (id: string) = document.getElementById(id) :?> Browser.Types.HTMLInputElement
    let getEl (id: string) = document.getElementById(id)

    let searchBooks () =
        let titleInput = getInput "titleSearch"
        let authorInput = getInput "authorSearch"

        let title = titleInput.value
        let author = authorInput.value

        let books = 
            match (title, author) with
            | ("", "") -> currentLibrary.Books
            | (title, "") -> (Search.searchByTitle currentLibrary title)
            | ("", author) -> (Search.searchByAuthor currentLibrary author)
            | (title, author) -> (Search.searchByTitleAndAuthor currentLibrary title author)

        books

    // 3. RENDER FUNCTION
    // This function clears the table and rebuilds it based on 'currentLibrary'
    let rec renderTable () =
        let tbody = getEl "bookTableBody"
        tbody.innerHTML <- "" // Clear existing rows

        // Loop through books and create HTML rows
        let books = searchBooks()

        books
        |> List.iter (fun book ->
            let row = document.createElement("tr")
            
            // Status text
            let statusText = if book.Available then "Available" else "Borrowed"
            
            // We set the innerHTML of the row directly
            // Note: We use buttons with 'data-id' attributes to handle clicks later
            row.innerHTML <- $"""
                <td>{book.Id}</td>
                <td><div style="width: 200px; height: 200px;"><img src="{book.Image}" style="max-width: 100{"%"}; max-height: 100{"%"}; object-fit: contain;"></div></td>
                <td>{book.Title}</td>
                <td>{book.Author}</td>
                <td>{statusText}</td>
                <td>
                    <button class="toggle-btn" data-id="{book.Id}">
                        {if book.Available then "Borrow" else "Return"}
                    </button>
                    <button class="delete-btn" data-id="{book.Id}">
                        Remove
                    </button>
                </td>
            """
            tbody.appendChild(row) |> ignore
        )

        // RE-ATTACH EVENT LISTENERS
        // Since we destroyed and recreated the buttons, we must add onclick events again
        let toggleBtns = document.getElementsByClassName("toggle-btn")
        for i in 0 .. toggleBtns.length - 1 do
            let btn = toggleBtns.[i] :?> Browser.Types.HTMLElement
            let id = int (btn.getAttribute("data-id"))
            btn.onclick <- fun _ ->
                // Logic to toggle status
                let updatedBooks = 
                    currentLibrary.Books 
                    |> List.map (fun b -> if b.Id = id then { b with Available = not b.Available } else b)
                
                currentLibrary <- { Books = updatedBooks }
                Storage.saveLibrary currentLibrary
                renderTable()

        let deleteBtns = document.getElementsByClassName("delete-btn")
        for i in 0 .. deleteBtns.length - 1 do
            let btn = deleteBtns.[i] :?> Browser.Types.HTMLElement
            let id = int (btn.getAttribute("data-id"))
            btn.onclick <- fun _ ->
                // Logic to delete
                if window.confirm($"Are you sure you want to delete book {id}?") then
                    let updatedBooks = currentLibrary.Books |> List.filter (fun b -> b.Id <> id)
                    currentLibrary <- { Books = updatedBooks }
                    Storage.saveLibrary currentLibrary
                    renderTable() 

    // 4. ADD BOOK HANDLER
    let handleAdd () =
        let titleInput = getInput "titleInput"
        let authorInput = getInput "authorInput"
        let imageInput = getInput "imageInput"

        let id =
            match currentLibrary.Books with
            | [] -> 1 // If empty, start at 1
            | books -> (books |> List.maxBy (fun b -> b.Id)).Id + 1 // Find max book, take its ID, add 1

        async {
            let title = titleInput.value
            let author = authorInput.value

            // Basic Validation
            if title = "" || author = "" || imageInput.files.length <= 0 then
                window.alert("Please fill in all fields")
            else
                let file = imageInput.files.item(0)
                let! image = Storage.uploadImage file

                // Update State
                let newBook = { Id = id; Title = title; Author = author; Available = true; Image = image }
                currentLibrary <- { Books = newBook :: currentLibrary.Books }
                
                // Save and Update UI
                Storage.saveLibrary currentLibrary
                renderTable()

                // Clear inputs
                titleInput.value <- ""
                authorInput.value <- ""
                imageInput.value <- ""
        }

    // 5. SEARCH BOOK HANDLER
    let handleSearch () =
        renderTable()


    // 5. INITIALIZATION
    // This runs when the script loads
    let init () =
        // Attach the Add button click listener
        let addBtn = getEl "addBtn"
        addBtn.onclick <- fun _ -> handleAdd() |> Async.StartImmediate

        let searchBtn = getEl "searchBtn"
        searchBtn.onclick <- fun _ -> handleSearch()

        // Initial render
        renderTable()

    // Run init
    init()
} |> Async.StartImmediate
