module Program

open ReadableContent
open EpubBuilder

[<EntryPoint>]
let main argv = 
    let getOutputPath() = System.Guid.NewGuid().ToString()
    
    let buildEpub url = 
        url
        |> getBookFromUrl
        |> buildEpub (getOutputPath())

    argv |> Seq.iter buildEpub
    0
