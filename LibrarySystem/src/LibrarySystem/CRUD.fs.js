import { iterate, filter, cons } from "./fable_modules/fable-library-js.4.28.0/List.js";
import { printf, toConsole } from "./fable_modules/fable-library-js.4.28.0/String.js";

export function addBook(library, book) {
    library.Books = cons(book, library.Books);
}

export function removeBook(library, bookId) {
    library.Books = filter((b) => (b.Id !== bookId), library.Books);
}

export function listBooks(library) {
    iterate((b) => {
        const arg_3 = b.Available ? "Available" : "Borrowed";
        toConsole(printf("%d: %s by %s - %s"))(b.Id)(b.Title)(b.Author)(arg_3);
    }, library.Books);
}

