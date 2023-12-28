#r "nuget: FsHttp"

open System
open FsHttp
open FsHttp.Operators

type Issue = {
    url: string
    repository_url: string
    labels_url: string
    comments_url: string
    events_url: string
    html_url: string
    id: int
    node_id: string
    number: int
    title: string
    user: {|
        login: string
        id: int
        node_id: string
        url: string
        html_url: string
        followers_url: string
        following_url: string
        gists_url: string
        starred_url: string
        subscriptions_url: string
        organizations_url: string
        repos_url: string
        events_url: string
        received_events_url: string
        ``type``: string
        site_admin: bool
    |}
    labels: string list
    state: string
    locked: bool
    assignee: string
    assignees: string list
    milestone: string
    comments: int
    created_at: DateTimeOffset
    updated_at: DateTimeOffset option
    closed_at: DateTimeOffset option
    author_association: string
    active_lock_reason: string
    body: string
    timeline_url: string
    performed_via_github_app: string
    state_reason: string
}

let githubGet route =
    http {
        GET ("https://api.github.com" </> route)
        AuthorizationBearer "**************"
        Accept "application/vnd.github.v3+json"
        UserAgent "FsHttp"
        header "X-GitHub-Api-Version" "2022-11-28"
    }
    
let getIssues (repoOwner: string) (repo: string) = 
    githubGet $"repos/{repoOwner}/{repo}/issues"
    |> Request.send
    |> Response.deserializeJson<Issue list>

getIssues "vide-collabo" "vide"
|> List.filter (fun x -> x.closed_at.IsNone)
|> List.map (fun x -> {| title = x.title; user = x.user.login |})
