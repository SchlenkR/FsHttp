(*
shell> dotnet fsi .\issue-109-114.fsx

System.FormatException: The format of value '' is invalid.
   at System.Net.Http.Headers.HttpHeaderParser.ParseValue(String value, Object storeValue, Int32& index)
   at System.Net.Http.Headers.HttpHeaders.ParseAndAddValue(HeaderDescriptor descriptor, HeaderStoreItemInfo info, String value)
   at System.Net.Http.Headers.HttpHeaders.Add(HeaderDescriptor descriptor, String value)
   at FsHttp.Request.toAsync@133-4.Invoke(CancellationToken ctok)
   at Microsoft.FSharp.Control.AsyncPrimitives.CallThenInvokeNoHijackCheck[a,b](AsyncActivation`1 ctxt, b result1, FSharpFunc`2 userCode) in D:\a\_work\1\s\src\FSharp.Core\async.fs:line 528
   at Microsoft.FSharp.Control.Trampoline.Execute(FSharpFunc`2 firstAction) in D:\a\_work\1\s\src\FSharp.Core\async.fs:line 112
--- End of stack trace from previous location ---
   at Microsoft.FSharp.Control.AsyncResult`1.Commit() in D:\a\_work\1\s\src\FSharp.Core\async.fs:line 454
   at Microsoft.FSharp.Control.AsyncPrimitives.QueueAsyncAndWaitForResultSynchronously[a](CancellationToken token, FSharpAsync`1 computation, FSharpOption`1 timeout) in D:\a\_work\1\s\src\FSharp.Core\async.fs:line 1140
   at Microsoft.FSharp.Control.FSharpAsync.RunSynchronously[T](FSharpAsync`1 computation, FSharpOption`1 timeout, FSharpOption`1 cancellationToken) in D:\a\_work\1\s\src\FSharp.Core\async.fs:line 1511
   at <StartupCode$FSI_0002>.$FSI_0002.main@() in C:\Users\ronal\source\repos\github\FsHttp\fiddle\dotnet_fsi_net7\issue-109-114.fsx:line 10
   at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
   at System.Reflection.MethodInvoker.Invoke(Object obj, IntPtr* args, BindingFlags invokeAttr)
*)

#r "nuget: FsHttp, 9.0.0"

open FsHttp

let req = http {
    GET "http://google.com"
}

let res = Request.send req

printfn $"res = %A{res}"
