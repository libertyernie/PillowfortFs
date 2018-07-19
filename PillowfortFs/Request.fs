namespace PillowfortFs

type PhotoPostRequest = {
    // post_to -> "current_user" or ID of community
    // post_type -> "picture"
    title: string // optional
    // picture[][file] -> empty application/octet-stream
    pic_url: string // actual name is picture[][pic_url]
    // picture[][row] -> 1
    // picture[][col] -> 0
    content: string // html
    tags: seq<string> // comma-delimited stirng
    privacy: string // public followers private
    rebloggable: bool
    commentable: bool
    nsfw: bool
}