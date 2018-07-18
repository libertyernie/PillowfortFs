namespace PillowfortFs

open System
open System.Net
open System.IO
open System.Text.RegularExpressions
open System.Threading.Tasks
open Newtonsoft.Json

type PillowfortClientException(message: string) =
    inherit ApplicationException(message)

type PillowfortClient() =
    let pillowfail str = raise (PillowfortClientException str)

    let defaultAvatarUrl = "https://www.gravatar.com/avatar/00000000000000000000000000000000?f=y&d=mp"

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
            pillowfail "Could not find username on /edit/username page (are you logged in?)"

        return m.Groups.[1].Value
    }

    member __.AsyncGetAvatar = async {
        let req = createRequest "https://pillowfort.io/settings"

        use! resp = req.AsyncGetResponse()
        use sr = new StreamReader(resp.GetResponseStream())

        let! html = sr.ReadToEndAsync() |> Async.AwaitTask
        let m = Regex.Match(html, """http://s3.amazonaws.com/pillowfortmedia/settings/avatars/[^"]+""")
        return if m.Success then m.Value else defaultAvatarUrl
    }

    member this.AsyncGetPosts page = async {
        let! username = this.AsyncWhoami

        let url = sprintf " https://pillowfort.io/%s/json/?p=%d" (WebUtility.UrlEncode(username)) page
        let req = createRequest url

        use! resp = req.AsyncGetResponse()
        use sr = new StreamReader(resp.GetResponseStream())

        let! json = sr.ReadToEndAsync() |> Async.AwaitTask
        return JsonConvert.DeserializeObject<PillowfortPostsResponse>(json)
    }

    member __.AsyncSignout = async {
        let req = createRequest "https://pillowfort.io/signout"
        use! resp = req.AsyncGetResponse()
        return ignore resp
    }
    
    member this.WhoamiAsync() = Async.StartAsTask this.AsyncWhoami
    member this.GetAvatarAsync() = Async.StartAsTask this.AsyncGetAvatar
    member this.GetPostsAsync page = Async.StartAsTask (this.AsyncGetPosts page)
    member this.SignoutAsync() = Async.StartAsTask this.AsyncSignout :> Task