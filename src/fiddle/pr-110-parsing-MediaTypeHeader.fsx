
open System.Text
open System.Net.Http.Headers

let ct = $"text/xxx; charset={Encoding.UTF8.WebName}"

// -----
// tests
// -----

// fails
MediaTypeHeaderValue ct

// works
MediaTypeHeaderValue.Parse ct

