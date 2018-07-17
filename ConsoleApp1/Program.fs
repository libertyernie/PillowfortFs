// Learn more about F# at http://fsharp.org

open System

[<EntryPoint>]
let main argv =
    let c = PillowfortFs.PillowfortClientFactory.login "user@example.com" "password" |> Async.RunSynchronously
    printfn "%s" c.Cookie
    0 // return an integer exit code
