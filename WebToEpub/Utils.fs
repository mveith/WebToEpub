module Utils

let removeForbiddenCharacters (string:string) =
    let map = [ (":", " -"); ("?", "")]
    let mutable result = string
    for (forbidden, replace) in map do
        result <- result.Replace(forbidden, replace)
    result
