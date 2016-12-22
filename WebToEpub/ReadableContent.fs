module ReadableContent

open FSharp.Data
open System

type ReadableContent = JsonProvider< """ {  "title": "title",  "content": "content",  "date_published": "2016-09-30T07:00:12.000Z",  "lead_image_url": "image_url",  "dek": "dek",  "url": "url",  "domain": "domain",  "excerpt": "excerpt",  "word_count": 123,  "direction": "ltr",  "total_pages": 1,  "rendered_pages": 1,  "next_page_url": null } """ >

type Book = 
    { Content : string
      Title : string
      Author : string }

let authenticationToken = System.IO.File.ReadAllText "ReadabilityAuthenticationToken.txt"

let parserApiRequest url key=
    let apiUrl = sprintf "https://mercury.postlight.com/parser?url=%s" url
    let headers = [("x-api-key", key) ]
    Http.RequestString (apiUrl, headers=headers)
    

let getReadableContent url = 
    parserApiRequest url authenticationToken|> ReadableContent.Parse

let getAuthor (article : ReadableContent.Root) = 
    article.Domain

let canGetBookFromUrl url = 
    try
        let result = parserApiRequest url authenticationToken
        true
    with 
        | :? System.Net.WebException as ex -> false

let getBookFromUrl url = 
    let readableContent = getReadableContent url
    { Content = readableContent.Content
      Title = readableContent.Title
      Author = getAuthor (readableContent) }
