module EpubBuilder

open System.IO
open ReadableContent
open Ionic.Zip

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

let createZipFile directoryPath (zipFileName : string) = 
    use zipFile = new ZipFile()
    zipFile.AddDirectory(directoryPath)
    zipFile.Save(zipFileName)

let getEpubName book = book.Title + ".epub"

let buildEpub outputPath (book : Book) = 
    let contentPackageContent = contentPackageContent book.Title book.Author
    let mainContent = mainContent book.Content
    let metadataPath = Path.Combine(outputPath, "META-INF")
    let containerFileName = Path.Combine(metadataPath, "container.xml")
    let rootFileName = Path.Combine(outputPath, "content.opf")
    let mimeTypeFileName = Path.Combine(outputPath, "mimetype")
    let contentFileName = Path.Combine(outputPath, "content.xhtml")
    Directory.CreateDirectory(outputPath)
    Directory.CreateDirectory(metadataPath)
    File.WriteAllText(containerFileName, containerContent)
    File.WriteAllText(rootFileName, contentPackageContent)
    File.WriteAllText(mimeTypeFileName, "application/epub+zip")
    File.WriteAllText(contentFileName, mainContent)
    createZipFile outputPath (getEpubName book)
    Directory.Delete(outputPath, true)
