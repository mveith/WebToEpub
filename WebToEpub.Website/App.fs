open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

let getIndexPage = 
    "Index.html"
    |> System.IO.File.ReadAllText
    |> OK

let setAttachmentHeader attachmentFileName = 
    Suave.Writers.setHeader "Content-Disposition" ("attachment; filename=" + attachmentFileName)

let downloadEpub (webPageUri : string) = 
    let (epubName, epubBytes) = ("test.epub", System.Text.Encoding.UTF8.GetBytes webPageUri)
    setAttachmentHeader epubName >=> ok epubBytes

let convert = 
    request (fun r -> 
        let queryChoice = r.queryParam "webPageUri"
        match queryChoice with
        | Choice1Of2 webPageUri -> webPageUri |> downloadEpub
        | Choice2Of2 msg -> Suave.RequestErrors.BAD_REQUEST(sprintf "webPageUri: %s" msg))

let webPart = 
    choose [ GET >=> choose [ path "/" >=> getIndexPage
                              path "/convert" >=> convert ] ]

startWebServer defaultConfig webPart
