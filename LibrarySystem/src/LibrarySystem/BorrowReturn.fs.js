import { tryFind } from "./fable_modules/fable-library-js.4.28.0/List.js";
import { printf, toConsole } from "./fable_modules/fable-library-js.4.28.0/String.js";

export function borrowBook(library, bookId) {
    const _arg = tryFind((b) => {
        if (b.Id === bookId) {
            return b.Available;
        }
        else {
            return false;
        }
    }, library.Books);
    if (_arg == null) {
        toConsole(printf("Book not available or not found"));
        return false;
    }
    else {
        const book = _arg;
        book.Available = false;
        toConsole(printf("You borrowed \'%s\'"))(book.Title);
        return true;
    }
}

export function returnBook(library, bookId) {
    const _arg = tryFind((b) => {
        if (b.Id === bookId) {
            return !b.Available;
        }
        else {
            return false;
        }
    }, library.Books);
    if (_arg == null) {
        toConsole(printf("Book not found or already returned"));
        return false;
    }
    else {
        const book = _arg;
        book.Available = true;
        toConsole(printf("You returned \'%s\'"))(book.Title);
        return true;
    }
}

