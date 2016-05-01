open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

let webPart = choose [ GET >=> choose [ path "/" >=> OK "Response..." ] ]

startWebServer defaultConfig webPart
