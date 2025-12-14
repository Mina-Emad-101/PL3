namespace LibrarySystem.Tests

open Xunit
open Models
open BorrowReturn  // نستخدم الكود الأصلي مباشرة

type LibraryTests() =

    [<Fact>]
    member _.``borrowBook should mark book as borrowed if available``() =
        let book = { Id = 1; Title = "Test Book"; Author = "Author"; Available = true }
        let library = { Books = [book] }

        let result = borrowBook library 1

        Assert.True(result)
        Assert.False(book.Available)

    [<Fact>]
    member _.``borrowBook should not borrow if book is unavailable``() =
        let book = { Id = 2; Title = "Borrowed Book"; Author = "Author"; Available = false }
        let library = { Books = [book] }

        let result = borrowBook library 2

        Assert.False(result)
        Assert.False(book.Available)

    [<Fact>]
    member _.``returnBook should mark book as available if borrowed``() =
        let book = { Id = 3; Title = "Some Book"; Author = "Author"; Available = false }
        let library = { Books = [book] }

        let result = returnBook library 3

        Assert.True(result)
        Assert.True(book.Available)

    [<Fact>]
    member _.``returnBook should not change if book is already available``() =
        let book = { Id = 4; Title = "Available Book"; Author = "Author"; Available = true }
        let library = { Books = [book] }

        let result = returnBook library 4

        Assert.False(result)
        Assert.True(book.Available)
