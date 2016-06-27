module EpubBuilder

open System.IO
open ReadableContent
open Ionic.Zip
open Utils
open System.Text

type Epub = 
    { Name : string
      Data : byte [] }

let containerContent = """<?xml version="1.0"?>
       <container version="1.0" xmlns="urn:oasis:names:tc:opendocument:xmlns:container">
        <rootfiles>
            <rootfile full-path="content.opf" media-type="application/oebps-package+xml"/>
        </rootfiles>
       </container>"""
let contentPackageContent = sprintf """<?xml version='1.0' encoding='UTF-8'?>
               <package xmlns:dc="http://purl.org/dc/elements/1.1/"
                        xmlns="http://www.idpf.org/2007/opf" version="2.0" unique-identifier="bookid">
                 <metadata>
                   <dc:title id="title">%s</dc:title>
                   <dc:identifier id="bookid">123</dc:identifier>
                   <dc:language>CZ</dc:language>
                   <dc:creator  id="creator">%s</dc:creator>
                   <dc:type>reference</dc:type>
                 </metadata>
                 <manifest>
                   <item id="content" href="content.xhtml" media-type="application/xhtml+xml"/>
                 </manifest>
                 <spine>
                   <itemref idref="content"/>
                 </spine>
               </package>"""
let mainContent = sprintf """<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
                <html xmlns="http://www.w3.org/1999/xhtml" lang="cs" xml:lang="cs">
                    <head></head>
                    <body>%s</body>
                </html>"""
                
let getEpubName book = 
    let book = removeForbiddenCharacters book.Title
    book + ".epub"

let setCustomContentIfExists customContent book= 
    match customContent with
    | Some content -> { book with Content = content }
    | None -> book
    
let buildEpub (book : Book) = 
    use zipFile = new ZipFile(System.Text.Encoding.UTF8)
    zipFile.AddEntry(Path.Combine("META-INF", "container.xml"), containerContent, Encoding.UTF8)
    zipFile.AddEntry("content.opf", contentPackageContent book.Title book.Author, Encoding.UTF8)
    zipFile.AddEntry("mimetype", "application/epub+zip", Encoding.UTF8)
    zipFile.AddEntry("content.xhtml", mainContent book.Content, Encoding.UTF8)
    use outputStream = new MemoryStream()
    zipFile.Save(outputStream)
    let bytes = outputStream.ToArray()
    { Data = bytes
      Name = getEpubName book }
