namespace PillowfortFs

open System
open System.Net
open System.IO
open System.Text.RegularExpressions

exception PillowfortClientException of string

type PillowfortClient() =
    let pillowfail str = raise (PillowfortClientException str)

    let cookies = new CookieContainer()
    let cookie_wrapper = new SingleCookieWrapper(cookies, new Uri("https://pillowfort.io"), "_Pillowfort_session")

    let createRequest (url: string) =
        WebRequest.CreateHttp(url, UserAgent = "PillowfortFs/0.1 (https://github.com/libertyernie)", CookieContainer = cookies)

    member __.Cookie
        with get() = cookie_wrapper.getCookieValue() |> Option.defaultValue null
        and set (v: string) = cookie_wrapper.setCookieValue <| Option.ofObj v

    member __.AsyncWhoami = async {
        let req = createRequest "https://pillowfort.io/edit/username"

        use! resp = req.AsyncGetResponse()
        use sr = new StreamReader(resp.GetResponseStream())

        let! html = sr.ReadToEndAsync() |> Async.AwaitTask
        let m = Regex.Match(html, """value="([^"]+)" name="user\[username\]" """)
        if not m.Success then
            pillowfail "Could not find username on /edit/username page"

        return m.Groups.[1].Value
    }

    member this.Whoami = Async.RunSynchronously this.AsyncWhoami
    member this.WhoamiAsync = Async.StartAsTask this.AsyncWhoami