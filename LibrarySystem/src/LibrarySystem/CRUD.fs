module CRUD
open Models

let addBook (library: Library) (book: Book) =
    library.Books <- book :: library.Books

let removeBook (library: Library) (bookId: int) =
    library.Books <- library.Books |> List.filter (fun b -> b.Id <> bookId)

let listBooks (library: Library) =
    library.Books |> List.iter (fun b ->
        printfn "%d: %s by %s - %s" b.Id b.Title b.Author (if b.Available then "Available" else "Borrowed"))
