open Suave
open Suave.Filters
open Suave.Files
open Suave.Operators
open Suave.Successful
open ReadableContent
open EpubBuilder
open System.Net

let setAttachmentHeader attachmentFileName = 
    let attachmentFileName = System.Uri.EscapeDataString attachmentFileName
    Suave.Writers.setHeader "Content-Disposition" ("attachment; filename*=UTF-8''" + attachmentFileName)

let downloadEpub (webPageUri : string) = 
    let epub = 
        webPageUri
        |> getBookFromUrl
        |> buildEpub
    setAttachmentHeader epub.Name >=> ok epub.Data

let convert = 
    request (fun r -> 
        let queryChoice = r.queryParam "webPageUri"
        match queryChoice with
        | Choice1Of2 webPageUri -> webPageUri |> downloadEpub
        | Choice2Of2 msg -> Suave.RequestErrors.BAD_REQUEST(sprintf "webPageUri: %s" msg))

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
                              path "/convert" >=> convert
                              path "/canConvert" >=> canConvert
                              path "/style.css" >=> file "style.css" ] ]

// change default bindings to avoid problems with Docker ports accesibility
let config = { defaultConfig with bindings = [ HttpBinding.mk HTTP IPAddress.Any 8083us ] }

startWebServer config webPart
