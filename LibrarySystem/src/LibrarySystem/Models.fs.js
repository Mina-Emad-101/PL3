import { Record } from "./fable_modules/fable-library-js.4.28.0/Types.js";
import { list_type, record_type, bool_type, string_type, int32_type } from "./fable_modules/fable-library-js.4.28.0/Reflection.js";

export class Book extends Record {
    constructor(Id, Title, Author, Image, Available) {
        super();
        this.Id = (Id | 0);
        this.Title = Title;
        this.Author = Author;
        this.Image = Image;
        this.Available = Available;
    }
}

export function Book_$reflection() {
    return record_type("Models.Book", [], Book, () => [["Id", int32_type], ["Title", string_type], ["Author", string_type], ["Image", string_type], ["Available", bool_type]]);
}

export class Library extends Record {
    constructor(Books) {
        super();
        this.Books = Books;
    }
}

export function Library_$reflection() {
    return record_type("Models.Library", [], Library, () => [["Books", list_type(Book_$reflection())]]);
}

