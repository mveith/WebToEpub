open ReadableContent
open EpubBuilder

[<EntryPoint>]
let main argv = 
    let saveToFile (epub : Epub) = System.IO.File.WriteAllBytes(epub.Name, epub.Data)
    
    let buildEpub url = 
        url
        |> getBookFromUrl
        |> buildEpub
    argv
    |> Seq.map buildEpub
    |> Seq.iter saveToFile
    0
