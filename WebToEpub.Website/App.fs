open Suave
open Suave.Filters
open Suave.Files
open Suave.Operators
open Suave.Successful
open ReadableContent
open EpubBuilder
open System.Net
open System.IO
open Utils

let setAttachmentHeader attachmentFileName = 
    let attachmentFileName = System.Uri.EscapeDataString attachmentFileName
    Suave.Writers.setHeader "Content-Disposition" ("attachment; filename*=UTF-8''" + attachmentFileName)

let saveEpub (webPageUri : string) = 
    let epub = 
        webPageUri
        |> getBookFromUrl
        |> buildEpub
    File.WriteAllBytes(epub.Name, epub.Data)
    OK epub.Name

let convert = 
    request (fun r -> 
        let badRequest msg = badRequest "webPageUri" msg
        processParameter r "webPageUri" saveEpub badRequest)

let download = 
    request (fun r -> 
        let downloadEpub epubName = setAttachmentHeader epubName >=> ok (File.ReadAllBytes epubName)
        let badRequest msg = badRequest "epubName" msg
        processParameter r "epubName" downloadEpub badRequest)

let canConvert = 
    request (fun r -> 
        let canConvert webPageUri = 
            if canGetBookFromUrl webPageUri then "OK"
            else "KO"
        processParameter r "webPageUri" canConvert (fun _ -> "KO") |> OK)

let webPart = 
    choose [ GET >=> choose [ path "/" >=> file "Index.html"
                              path "/download" >=> download
                              path "/canConvert" >=> canConvert
                              path "/style.css" >=> file "style.css" ]
             POST >=> choose [ path "/convert" >=> convert ] ]

// change default bindings to avoid problems with Docker ports accesibility
let config = { defaultConfig with bindings = [ HttpBinding.mk HTTP IPAddress.Any 8083us ] }

startWebServer config webPart
