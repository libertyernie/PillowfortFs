module AuthenticityToken

open System.IO
open System.Net
open System.Text.RegularExpressions

let get_authenticity_token (url: string) cookies = async {
    let req = WebRequest.CreateHttp(url, CookieContainer = cookies, UserAgent = "PillowfortFs/0.1 (https://github.com/libertyernie)")

    use! response = req.AsyncGetResponse()
    use sr = new StreamReader(response.GetResponseStream())
    let! contents = sr.ReadToEndAsync() |> Async.AwaitTask

    let m = Regex.Match(contents, """<input type="hidden" name="authenticity_token" value="([^"]+)" """)

    return if m.Success
        then Some m.Groups.[1].Value
        else None
}