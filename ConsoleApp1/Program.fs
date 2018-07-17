// Learn more about F# at http://fsharp.org

open System

[<EntryPoint>]
let main argv =
    let c = PillowfortFs.PillowfortClientFactory.login "user@example.com" "password" |> Async.RunSynchronously
    let n = c.AsyncWhoami |> Async.RunSynchronously
    printfn "%s" n
    0 // return an integer exit code
