open Models
open CRUD
open Search
open BorrowReturn
open Storage

let libraryFile = "data/library.json"
let library = Storage.loadLibrary libraryFile

let rec menu () =
    printfn "\n--- Library System ---"
    printfn "1. List books"
    printfn "2. Add book"
    printfn "3. Remove book"
    printfn "4. Search by title"
    printfn "5. Search by author"
    printfn "6. Borrow book"
    printfn "7. Return book"
    printfn "0. Exit"
    printf "Enter choice: "

    match System.Console.ReadLine() with
    | "1" -> 
        listBooks library
        menu()
    | "2" ->
        printf "Book Id: "
        let id = int (System.Console.ReadLine())
        printf "Title: "
        let title = System.Console.ReadLine()
        printf "Author: "
        let author = System.Console.ReadLine()

        // check for duplicate Id
        if library.Books |> List.exists (fun b -> b.Id = id) then
            printfn "A book with Id %d already exists!" id
        else
            library.Books <- { Id=id; Title=title; Author=author; Available=true } :: library.Books
            printfn "Book added successfully."
            Storage.saveLibrary library libraryFile

        menu()
    | "3" ->
        printf "Book Id to remove: "
        let id = int (System.Console.ReadLine())
        removeBook library id
        Storage.saveLibrary library libraryFile
        menu()
    | "4" ->
        printf "Title to search: "
        let title = System.Console.ReadLine()
        searchByTitle library title
        |> List.iter (fun b -> printfn "%d: %s by %s" b.Id b.Title b.Author)
        menu()
    | "5" ->
        printf "Author to search: "
        let author = System.Console.ReadLine()
        searchByAuthor library author
        |> List.iter (fun b -> printfn "%d: %s by %s" b.Id b.Title b.Author)
        menu()
    | "6" ->
        printf "Book Id to borrow: "
        let id = int (System.Console.ReadLine())
        borrowBook library id
        Storage.saveLibrary library libraryFile
        menu()
    | "7" ->
        printf "Book Id to return: "
        let id = int (System.Console.ReadLine())
        returnBook library id
        Storage.saveLibrary library libraryFile
        menu()
    | "0" -> 
        printfn "Goodbye!"
    | _ -> 
        printfn "Invalid choice"
        menu()

[<EntryPoint>]
let main argv =
    menu()
    0
