module Search

open Models

let searchByTitle (library: Library) (title: string) =
    library.Books |> List.filter (fun b -> b.Title.ToLower().Contains(title.ToLower()))

let searchByAuthor (library: Library) (author: string) =
    library.Books |> List.filter (fun b -> b.Author.ToLower().Contains(author.ToLower()))

let searchByTitleAndAuthor (library: Library) (title: string) (author: string) =
    library.Books |> List.filter (fun b -> b.Title.ToLower().Contains(title.ToLower()) && b.Author.ToLower().Contains(author.ToLower()))
