namespace PillowfortFs

type PrivacyLevel =
    Public = 0 | Followers = 1 | Private = 2

type PhotoPostRequest = {
    title: string
    pic_url: string
    content: string
    tags: seq<string>
    privacy: PrivacyLevel
    rebloggable: bool
    commentable: bool
    nsfw: bool
}