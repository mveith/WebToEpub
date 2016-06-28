module CleanupTimer

open System
open System.IO

let interval = TimeSpan.FromMinutes(5.0)
let oldTimeTreshold() = DateTime.UtcNow.AddMinutes(-5.0)

let oldBookFiles treshold = 
    Directory.GetFiles(Directory.GetCurrentDirectory(), "*.epub")
    |> Array.map FileInfo
    |> Array.filter (fun fi -> fi.LastWriteTimeUtc < treshold)

let deleteOldBooks() = 
    let oldBooks = oldBookFiles (oldTimeTreshold())
    for book in oldBooks do
        File.Delete book.FullName

let startCleanupTimer() = 
    let interval = interval.TotalMilliseconds
    let timer = new System.Timers.Timer(interval)
    timer.Elapsed.Add(fun _ -> deleteOldBooks())
    timer.Start()
