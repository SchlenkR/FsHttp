(**
---
title: Logging
category: Documentation
categoryindex: 1
index: 14
---
*)

(**
## Turn on/off Logging

How does logging work in FsHttp:

- In F# interactive, console logging (via logfn) are enabled per default.
- It's then possible to deactivate logging globally, using `Fsi.disableDebugLogs()`
- In a non-F# interactive environment, there should be no logging at all per default.
- It's possible to turn on logging via `Fsi.enableDebugLogs()` (which is also applied to non-interactive environments).

*)