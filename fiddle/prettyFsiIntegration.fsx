#r "nuget: PrettyFsi"
PrettyFsi.addPrinters(fsi, PrettyFsi.TableMode.Implicit)

#r "nuget: FsHttp"
open FsHttp
open FsHttp.Operators


% http {
    GET "https://api.github.com/users/ronaldschlenker"
    UserAgent "FsHttp"
}



// 
typeof<int[]>.IsSZArray
