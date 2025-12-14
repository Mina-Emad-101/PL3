module Server.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Thoth.Json.Net

// 1. CONFIGURATION
let uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")
let libraryFile = Path.Combine(Directory.GetCurrentDirectory(), "data", "library.json")

// Ensure folders exist
if not (Directory.Exists uploadsFolder) then Directory.CreateDirectory uploadsFolder |> ignore
if not (Directory.Exists (Path.GetDirectoryName libraryFile)) then Directory.CreateDirectory (Path.GetDirectoryName libraryFile) |> ignore

// 2. MODELS
type Book = {
    Id: int
    Title: string
    Author: string
    Available: bool
    Image: string
}
type Library = { Books: Book list }

// 3. HANDLERS

// GET /api/library
let getLibrary : HttpHandler =
    fun next ctx ->
        task {
            if File.Exists libraryFile then
                let! json = File.ReadAllTextAsync libraryFile
                return! text json next ctx
            else
                return! json { Books = [] } next ctx
        }

// POST /api/library
let saveLibrary : HttpHandler =
    fun next ctx ->
        task {
            // We read the raw body as a string and save it directly
            // In a real app, you should decode/validate it first
            let! body = ctx.ReadBodyFromRequestAsync()
            do! File.WriteAllTextAsync(libraryFile, body)
            return! text "Saved" next ctx
        }

// POST /api/upload
let uploadImage : HttpHandler =
    fun next ctx ->
        task {
            // "files" corresponds to the form-data key we will send from frontend
            if ctx.Request.Form.Files.Count > 0 then
                let file = ctx.Request.Form.Files.[0]
                // Generate unique name to prevent overwrites
                let fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}"
                let filePath = Path.Combine(uploadsFolder, fileName)
                
                use stream = new FileStream(filePath, FileMode.Create)
                do! file.CopyToAsync(stream)

                // Return the public URL
                let url = $"/uploads/{fileName}"
                return! text url next ctx
            else
                return! RequestErrors.BAD_REQUEST "No file uploaded" next ctx
        }

// 4. ROUTING
let webApp =
    choose [
        route "/api/library" >=> choose [
            GET >=> getLibrary
            POST >=> saveLibrary
        ]
        route "/api/upload" >=> POST >=> uploadImage
    ]

// 5. SETUP
[<EntryPoint>]
let main _ =
    let builder = WebApplication.CreateBuilder()
    builder.Services.AddGiraffe()
    builder.Services.AddCors() // Allow frontend to talk to backend

    let app = builder.Build()

    // Enable static files so we can serve the images from uploads
    app.UseStaticFiles()
    
    // Configure CORS (Allow your Vite frontend port)
    app.UseCors(fun builder -> 
        builder.WithOrigins("http://localhost:5173") // CHANGE THIS if your vite port is different
               .AllowAnyMethod()
               .AllowAnyHeader()
               |> ignore)

    app.UseGiraffe(webApp)
    app.Run()
    0
