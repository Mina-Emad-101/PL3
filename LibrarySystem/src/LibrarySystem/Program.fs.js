import { startImmediate } from "./fable_modules/fable-library-js.4.28.0/Async.js";
import { singleton } from "./fable_modules/fable-library-js.4.28.0/AsyncBuilder.js";
import { uploadImage, saveLibrary, loadLibrary } from "./Storage.fs.js";
import { searchByTitleAndAuthor, searchByTitle, searchByAuthor } from "./Search.fs.js";
import { cons, maxBy, isEmpty, filter, map, iterate } from "./fable_modules/fable-library-js.4.28.0/List.js";
import { parse } from "./fable_modules/fable-library-js.4.28.0/Int32.js";
import { Library, Book } from "./Models.fs.js";
import { comparePrimitives } from "./fable_modules/fable-library-js.4.28.0/Util.js";
import { some } from "./fable_modules/fable-library-js.4.28.0/Option.js";

startImmediate(singleton.Delay(() => singleton.Bind(loadLibrary(), (_arg) => {
    const initial = _arg;
    let currentLibrary = initial;
    const getInput = (id) => document.getElementById(id);
    const getEl = (id_1) => document.getElementById(id_1);
    const searchBooks = () => {
        const titleInput = getInput("titleSearch");
        const authorInput = getInput("authorSearch");
        const title = titleInput.value;
        const author = authorInput.value;
        let books;
        if (title === "") {
            if (author === "") {
                books = currentLibrary.Books;
            }
            else {
                const author_1 = author;
                books = searchByAuthor(currentLibrary, author_1);
            }
        }
        else if (author === "") {
            const title_1 = title;
            books = searchByTitle(currentLibrary, title_1);
        }
        else {
            const title_2 = title;
            const author_2 = author;
            books = searchByTitleAndAuthor(currentLibrary, title_2, author_2);
        }
        return books;
    };
    const renderTable = () => {
        const tbody = getEl("bookTableBody");
        tbody.innerHTML = "";
        const books_1 = searchBooks();
        iterate((book) => {
            const row = document.createElement("tr");
            const statusText = book.Available ? "Available" : "Borrowed";
            row.innerHTML = (`
                <td>${book.Id}</td>
                <td><div style="width: 200px; height: 200px;"><img src="${book.Image}" style="max-width: 100${"%"}; max-height: 100${"%"}; object-fit: contain;"></div></td>
                <td>${book.Title}</td>
                <td>${book.Author}</td>
                <td>${statusText}</td>
                <td>
                    <button class="toggle-btn" data-id="${book.Id}">
                        ${book.Available ? "Borrow" : "Return"}
                    </button>
                    <button class="delete-btn" data-id="${book.Id}">
                        Remove
                    </button>
                </td>
            `);
            tbody.appendChild(row);
        }, books_1);
        const toggleBtns = document.getElementsByClassName("toggle-btn");
        for (let i = 0; i <= (toggleBtns.length - 1); i++) {
            const btn = toggleBtns[i];
            const id_2 = parse(btn.getAttribute("data-id"), 511, false, 32) | 0;
            btn.onclick = ((_arg_1) => {
                const updatedBooks = map((b) => {
                    if (b.Id === id_2) {
                        return new Book(b.Id, b.Title, b.Author, b.Image, !b.Available);
                    }
                    else {
                        return b;
                    }
                }, currentLibrary.Books);
                currentLibrary = (new Library(updatedBooks));
                saveLibrary(currentLibrary);
                renderTable();
            });
        }
        const deleteBtns = document.getElementsByClassName("delete-btn");
        for (let i_1 = 0; i_1 <= (deleteBtns.length - 1); i_1++) {
            const btn_1 = deleteBtns[i_1];
            const id_3 = parse(btn_1.getAttribute("data-id"), 511, false, 32) | 0;
            btn_1.onclick = ((_arg_2) => {
                if (window.confirm(`Are you sure you want to delete book ${id_3}?`)) {
                    const updatedBooks_1 = filter((b_1) => (b_1.Id !== id_3), currentLibrary.Books);
                    currentLibrary = (new Library(updatedBooks_1));
                    saveLibrary(currentLibrary);
                    renderTable();
                }
            });
        }
    };
    const handleAdd = () => {
        const titleInput_1 = getInput("titleInput");
        const authorInput_1 = getInput("authorInput");
        const imageInput = getInput("imageInput");
        let id_4;
        const matchValue_1 = currentLibrary.Books;
        if (isEmpty(matchValue_1)) {
            id_4 = 1;
        }
        else {
            const books_2 = matchValue_1;
            id_4 = (maxBy((b_2) => b_2.Id, books_2, {
                Compare: comparePrimitives,
            }).Id + 1);
        }
        return singleton.Delay(() => {
            const title_3 = titleInput_1.value;
            const author_3 = authorInput_1.value;
            if (((title_3 === "") ? true : (author_3 === "")) ? true : (imageInput.files.length <= 0)) {
                window.alert(some("Please fill in all fields"));
                return singleton.Zero();
            }
            else {
                const file = imageInput.files.item(0);
                return singleton.Bind(uploadImage(file), (_arg_3) => {
                    const image = _arg_3;
                    const newBook = new Book(id_4, title_3, author_3, image, true);
                    currentLibrary = (new Library(cons(newBook, currentLibrary.Books)));
                    saveLibrary(currentLibrary);
                    renderTable();
                    titleInput_1.value = "";
                    authorInput_1.value = "";
                    imageInput.value = "";
                    return singleton.Zero();
                });
            }
        });
    };
    const handleSearch = () => {
        renderTable();
    };
    const init = () => {
        const addBtn = getEl("addBtn");
        addBtn.onclick = ((_arg_4) => {
            startImmediate(handleAdd());
        });
        const searchBtn = getEl("searchBtn");
        searchBtn.onclick = ((_arg_5) => {
            handleSearch();
        });
        renderTable();
    };
    init();
    return singleton.Zero();
})));

