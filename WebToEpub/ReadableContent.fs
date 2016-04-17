module ReadableContent

open FSharp.Data
open System

type ReadableContent = JsonProvider< """ { "content":"content", "url":"url", "excerpt":"excerpt", "title":"title", "author":"author", "domain":"domain" } """ >

type Book = 
    { Content : string
      Title : string
      Author : string }

let parserApiUrl = sprintf "https://www.readability.com/api/content/v1/parser?url=%s&token=%s"
let authenticationToken = System.IO.File.ReadAllText "ReadabilityAuthenticationToken.txt"

let getReadableContent url = 
    let apiUrl = parserApiUrl url authenticationToken
    Http.RequestString apiUrl |> ReadableContent.Parse

let getAuthor (article : ReadableContent.Root) = 
    if String.IsNullOrEmpty(article.Author) then article.Domain
    else article.Author

let getBookFromUrl url = 
    let readableContent = getReadableContent url
    { Content = readableContent.Content
      Title = readableContent.Title
      Author = getAuthor (readableContent) }
