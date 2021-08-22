---
layout: default
---

# FsHttp

FsHttp is an F# / .Net HTTP client library. It aims for describing and executing HTTP 
requests in idiomatic and convenient ways that can be used for production, tests and 
FSI (F# Interactive).

[![NuGet Badge](http://img.shields.io/nuget/v/SchlenkR.FsHttp.svg?style=flat)](https://www.nuget.org/packages/SchlenkR.FsHttp) [![Build Status](https://travis-ci.org/ronaldschlenker/FsHttp.svg?branch=master)](https://travis-ci.org/ronaldschlenker/FsHttp)


NuGet
-----

Install FsHttp via NuGet command line:

{% highlight dosbatch %}
PM> Install-Package SchlenkR.FsHttp
{% endhighlight %}

or via F# Interactive:

{% highlight fsharp %}
#r "nuget: SchlenkR.FsHttp"
{% endhighlight %}


[Nuget SchlenkR.FsHttp](https://www.nuget.org/packages/SchlenkR.FsHttp)

A simple request
----------------

{% highlight fsharp %}
http {
    POST "https://reqres.in/api/users"
    CacheControl "no-cache"
    body
    json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
}
{% endhighlight %}

