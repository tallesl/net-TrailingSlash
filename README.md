# TrailingSlash

[![][build-img]][build]
[![][nuget-img]][nuget]

Redirects the request to the same URL with a trailing slash for a given set of cases.

I'm not here to argue "to slash or not slash" ([1]&nbsp;[2]&nbsp;[3]), but if you choose to do so, this is what I use
for ASP.NET (MVC) web sites.

[build]:     https://ci.appveyor.com/project/TallesL/net-TrailingSlash
[build-img]: https://ci.appveyor.com/api/projects/status/github/tallesl/net-TrailingSlash?svg=true
[nuget]:     https://www.nuget.org/packages/TrailingSlash
[nuget-img]: https://badge.fury.io/nu/TrailingSlash.svg

[1]: http://googlewebmastercentral.blogspot.com/2010/04/to-slash-or-not-to-slash.html
[2]: http://stackoverflow.com/q/5948659
[3]: http://webmasters.stackexchange.com/q/2498

## Usage

```cs
using TrailingSlashLibrary;

var slash = new TrailingSlash();
```

You can configure some set of exceptions (urls to leave alone).
There are flags for empty urls (`ExceptEmpty`), with extension (`ExceptWithExtension`) and from API (`ExceptFromApi`).
All set to `True` by default.

You can also add exceptions of your own in the `Except` property, such as:

```cs
slash.Except =
    url =>
    {
        return url.Contains("mini-profiler-resources");
    };
```

(here we are ignoring [MiniProfiler] calls)

Now it's just a matter to put an `Append` call in you `Application_BeginRequest`:

```cs
protected void Application_BeginRequest(object sender, EventArgs e)
{
    slash.Append();
}
```

Done.

[MiniProfiler]: http://miniprofiler.com