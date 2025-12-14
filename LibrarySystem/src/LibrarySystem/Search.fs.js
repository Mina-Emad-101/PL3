import { filter } from "./fable_modules/fable-library-js.4.28.0/List.js";

export function searchByTitle(library, title) {
    return filter((b) => (b.Title.toLocaleLowerCase().indexOf(title.toLocaleLowerCase()) >= 0), library.Books);
}

export function searchByAuthor(library, author) {
    return filter((b) => (b.Author.toLocaleLowerCase().indexOf(author.toLocaleLowerCase()) >= 0), library.Books);
}

export function searchByTitleAndAuthor(library, title, author) {
    return filter((b) => {
        if (b.Title.toLocaleLowerCase().indexOf(title.toLocaleLowerCase()) >= 0) {
            return b.Author.toLocaleLowerCase().indexOf(author.toLocaleLowerCase()) >= 0;
        }
        else {
            return false;
        }
    }, library.Books);
}

