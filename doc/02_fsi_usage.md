

## FSharp Interactive Usage

When using FsHttp in F# Interactive, you should not reference the dll, but rather load the `FsHttp.fsx` file to add pretty print for requests and responses, and you should open the FsHttp.Fsi module to have control over the printing:

```fsharp
#load @"./packages/SchlenkR.FsHttp/netstandard2.0/FsHttp.fsx"

open FsHttp
open FsHttp.Fsi
// open your desired flavour - see sample files!
```

### Response Printing

When working inside FSI, there are several 'hints' that can be given to specify the way FSI will print the response. Have a look at [FsiPrinting](src/FsHttp/Fsi.fs) for details.

To specify 

**Examples**

```fsharp
// Default print options (don't print request; print response headers, a formatted preview of the content)
get @"https://reqres.in/api/users?page=2&delay=3" run go

// Default print options (see above) + max. content length of 100
get @"https://reqres.in/api/users?page=2&delay=3" run (show 100)

// Default print options (don't print request; print response headers, whole content formatted)
get @"https://reqres.in/api/users?page=2&delay=3" run expand
```
