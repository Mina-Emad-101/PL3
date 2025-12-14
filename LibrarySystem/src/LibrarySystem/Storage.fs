module Storage

open Fable.Core
open Fable.Core.JsInterop
open Fetch
open Thoth.Json
open Models
open Browser.Types

let private apiUrl = "http://localhost:5039/api"

// HELPER: Create FormData using Javascript 'new'
[<Emit("new FormData()")>]
let createFormData () : FormData = jsNative

// Load from Server
let loadLibrary () : Async<Library> =
    async {
        try
            let! response = Async.AwaitPromise(fetch $"{apiUrl}/library" [])
            let! json = Async.AwaitPromise(response.text())
            match Decode.Auto.fromString<Library>(json) with
            | Ok lib -> return lib
            | Error _ -> return { Books = [] }
        with _ -> return { Books = [] }
    }

// Save to Server
let saveLibrary (library: Library) =
    let json = Encode.Auto.toString(0, library)
    
    // NUCLEAR OPTION: createObj
    // We create a raw JS object { "Content-Type": "application/json" }
    // and cast it (!^) to whatever Fetch expects.
    let headers = [
        HttpRequestHeaders.ContentType "application/json"
    ]

    let props = [ 
        RequestProperties.Method HttpMethod.POST
        RequestProperties.Body !^json 
    ]
    
    fetch $"{apiUrl}/library" props |> Promise.start

// Upload Image to Server
let uploadImage (file: File) : Async<string> =
    async {
        let formData = createFormData()
        formData.append("file", file)

        let props = 
            [ RequestProperties.Method HttpMethod.POST
              RequestProperties.Body !^formData ]
        
        let! response = Async.AwaitPromise(fetch $"{apiUrl}/upload" props)
        
        if response.Ok then
            let! url = Async.AwaitPromise(response.text())
            return $"http://localhost:5039{url}"
        else
            return ""
    }
