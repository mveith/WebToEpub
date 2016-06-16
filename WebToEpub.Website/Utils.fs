module Utils

open Suave

let processParameter (request : HttpRequest) paramName processValue processInvalidValue = 
    let queryChoice = request.queryParam paramName
    match queryChoice with
    | Choice1Of2 value -> processValue value
    | Choice2Of2 msg -> processInvalidValue msg

let badRequest parameterName msg = Suave.RequestErrors.BAD_REQUEST(sprintf "%s: %s" parameterName msg)
