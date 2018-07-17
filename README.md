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

Visual Basic usage is similar to C#.

### Other languages

The library also makes non-async methods available, which use
[Async.RunSynchronously](https://msdn.microsoft.com/en-us/visualfsharpdocs/conceptual/async.runsynchronously%5B%27t%5D-method-%5Bfsharp%5D)
under the hood. These methods are not guaranteed to work in all environments;
use the async versions if you can.
