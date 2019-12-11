
#load "./bin/Debug/netstandard2.0/FsHttp.fsx"

open FsHttp
open FsHttp.DslCE



http {
    POST "http://www.csm-testcenter.org/test"
    multipart
    filePart "c:\\temp\\test.txt"
    filePart "c:\\temp\\test.txt"
}
