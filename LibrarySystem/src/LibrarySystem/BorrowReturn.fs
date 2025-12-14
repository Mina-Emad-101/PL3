module BorrowReturn
open Models

let borrowBook (library: Library) (bookId: int) : bool =
    library.Books
    |> List.tryFind (fun b -> b.Id = bookId && b.Available)
    |> function
       | Some book -> 
           book.Available <- false
           printfn "You borrowed '%s'" book.Title
           true
       | None -> 
           printfn "Book not available or not found"
           false

let returnBook (library: Library) (bookId: int) : bool =
    library.Books
    |> List.tryFind (fun b -> b.Id = bookId && not b.Available)
    |> function
       | Some book -> 
           book.Available <- true
           printfn "You returned '%s'" book.Title
           true
       | None -> 
           printfn "Book not found or already returned"
           false
