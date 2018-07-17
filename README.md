# PillowfortFs

An F# / .NET library for logging into https://pillowfort.io and getting your
username, avatar, and posts.

This will probably break at some point as the site is updated and I can't
promise I'll try to fix it.

## Example usage

### F#

    open PillowfortFs

    let f = async {
        let! client = PillowfortClientFactory.AsyncLogin username password
        let! username = client.AsyncWhoami
        return username
    }

### C#

    using PillowfortFs;

    Task<string> f() {
        var client = await PillowfortClientFactory.LoginAsync(username, password);
        var username = await client.WhoamiAsync();
        return username;
    }

### Visual Basic

    Imports PillowfortFs

    Async Function f() As Task(Of String)
        Dim client = Await PillowfortClientFactory.LoginAsync(username, password)
        Dim username = Await client.WhoamiAsync()
        Return username
    End Function