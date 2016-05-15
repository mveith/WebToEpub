open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open ReadableContent
open EpubBuilder
open System.Net

let getIndexPage = 
    "Index.html"
    |> System.IO.File.ReadAllText
    |> OK

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

let webPart = 
    choose [ GET >=> choose [ path "/" >=> getIndexPage
                              path "/convert" >=> convert ] ]

// change default bindings to avoid problems with Docker ports accesibility
let config = { defaultConfig with bindings = [ HttpBinding.mk HTTP IPAddress.Any 8083us ] }
startWebServer config webPart
