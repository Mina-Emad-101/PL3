import { singleton } from "./fable_modules/fable-library-js.4.28.0/AsyncBuilder.js";
import { awaitPromise } from "./fable_modules/fable-library-js.4.28.0/Async.js";
import { Types_RequestProperties, Types_HttpRequestHeaders, fetch$ } from "./fable_modules/Fable.Fetch.2.7.0/Fetch.fs.js";
import { ofArray, singleton as singleton_1, empty } from "./fable_modules/fable-library-js.4.28.0/List.js";
import { fromString, Auto_generateBoxedDecoder_Z6670B51 } from "./fable_modules/Thoth.Json.10.4.1/./Decode.fs.js";
import { Library, Library_$reflection } from "./Models.fs.js";
import { uncurry2 } from "./fable_modules/fable-library-js.4.28.0/Util.js";
import { toString, Auto_generateBoxedEncoder_437914C6 } from "./fable_modules/Thoth.Json.10.4.1/./Encode.fs.js";

const apiUrl = "http://localhost:5039/api";

export function loadLibrary() {
    return singleton.Delay(() => singleton.TryWith(singleton.Delay(() => singleton.Bind(awaitPromise(fetch$(`${apiUrl}/library`, empty())), (_arg) => {
        const response = _arg;
        return singleton.Bind(awaitPromise(response.text()), (_arg_1) => {
            const json = _arg_1;
            let matchValue;
            const decoder = Auto_generateBoxedDecoder_Z6670B51(Library_$reflection(), undefined, undefined);
            matchValue = fromString(uncurry2(decoder), json);
            if (matchValue.tag === 1) {
                return singleton.Return(new Library(empty()));
            }
            else {
                const lib = matchValue.fields[0];
                return singleton.Return(lib);
            }
        });
    })), (_arg_2) => singleton.Return(new Library(empty()))));
}

export function saveLibrary(library) {
    let json;
    const encoder = Auto_generateBoxedEncoder_437914C6(Library_$reflection(), undefined, undefined, undefined);
    json = toString(0, encoder(library));
    const headers = singleton_1(new Types_HttpRequestHeaders(11, ["application/json"]));
    const props = ofArray([new Types_RequestProperties(0, ["POST"]), new Types_RequestProperties(2, [json])]);
    const pr = fetch$(`${apiUrl}/library`, props);
    pr.then();
}

export function uploadImage(file) {
    return singleton.Delay(() => {
        const formData = new FormData();
        formData.append("file", file);
        const props = ofArray([new Types_RequestProperties(0, ["POST"]), new Types_RequestProperties(2, [formData])]);
        return singleton.Bind(awaitPromise(fetch$(`${apiUrl}/upload`, props)), (_arg) => {
            const response = _arg;
            return (response.ok) ? singleton.Bind(awaitPromise(response.text()), (_arg_1) => {
                const url = _arg_1;
                return singleton.Return(`http://localhost:5039${url}`);
            }) : singleton.Return("");
        });
    });
}

