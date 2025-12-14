module Models

type Book = {
    Id: int
    Title: string
    Author: string
    Image: string
    mutable Available: bool
}

type Library = {
    mutable Books: Book list
}
