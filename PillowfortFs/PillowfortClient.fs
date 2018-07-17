namespace PillowfortFs

open System
open System.Net

type PillowfortClient() =
    let cookies = new CookieContainer()
    let cookie_wrapper = new SingleCookieWrapper(cookies, new Uri("https://pillowfort.io"), "_Pillowfort_session")

    member __.Cookie
        with get() = cookie_wrapper.getCookieValue() |> Option.defaultValue null
        and set (v: string) = cookie_wrapper.setCookieValue <| Option.ofObj v