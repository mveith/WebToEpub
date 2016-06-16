﻿open Suave
open Suave.Filters
open Suave.Files
open Suave.Operators
open Suave.Successful
open ReadableContent
open EpubBuilder
open System.Net
open System.IO

let setAttachmentHeader attachmentFileName = 
    let attachmentFileName = System.Uri.EscapeDataString attachmentFileName
    Suave.Writers.setHeader "Content-Disposition" ("attachment; filename*=UTF-8''" + attachmentFileName)

let saveEpub (webPageUri : string) = 
    let epub = 
        webPageUri
        |> getBookFromUrl
        |> buildEpub
    File.WriteAllBytes(epub.Name, epub.Data)
    epub.Name

let convert = 
    request (fun r -> 
        let queryChoice = r.queryParam "webPageUri"
        match queryChoice with
        | Choice1Of2 webPageUri -> 
            webPageUri
            |> saveEpub
            |> OK
        | Choice2Of2 msg -> Suave.RequestErrors.BAD_REQUEST(sprintf "webPageUri: %s" msg))

let download = 
    request (fun r -> 
        let queryChoice = r.queryParam "epubName"
        match queryChoice with
        | Choice1Of2 epubName -> setAttachmentHeader epubName >=> ok (File.ReadAllBytes epubName)
        | Choice2Of2 msg -> Suave.RequestErrors.BAD_REQUEST(sprintf "epubName: %s" msg))

let canConvert = 
    request (fun r -> 
        let queryChoice = r.queryParam "webPageUri"
        
        let canConvertValue = 
            match queryChoice with
            | Choice1Of2 webPageUri -> 
                if canGetBookFromUrl webPageUri then "OK"
                else "KO"
            | Choice2Of2 msg -> "KO"
        OK canConvertValue)

let webPart = 
    choose [ GET >=> choose [ path "/" >=> file "Index.html"
                              path "/download" >=> download
                              path "/canConvert" >=> canConvert
                              path "/style.css" >=> file "style.css" ]
             POST >=> choose [ path "/convert" >=> convert ] ]

// change default bindings to avoid problems with Docker ports accesibility
let config = { defaultConfig with bindings = [ HttpBinding.mk HTTP IPAddress.Any 8083us ] }

startWebServer config webPart
