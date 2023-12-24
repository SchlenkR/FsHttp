#r "nuget: PrettyFsi"
PrettyFsi.addPrinters(fsi, PrettyFsi.TableMode.Implicit)

#r "nuget: RestInPeace"
open RestInPeace
open RestInPeace.Operators


% http {
    GET "https://api.github.com/users/ronaldschlenker"
    UserAgent "RestInPeace"
}



// 
typeof<int[]>.IsSZArray
