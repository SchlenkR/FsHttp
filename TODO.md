
# TODO

## Docu (Readme.md)

* form url encoded
    * Alternative: ContentType mit "text" body und string
    * document .> and >. operators
* content-type
* edit raw request
* a word to ContentType / Body
* explain: expand, preview, raw, etc.
* explain the DSL operators
* Write Response to a file
* auth use case mit aufnehmen
* alle "content" headers (z.B. contentEncoding) sind in HeaderContext und müssen in BodyContext
* Docu: FSI / nonFSI Szenarien besser herausarbeiten
    * |> send    |> sendAsync verwenden anstatt .> oder >. (diese nur in FSI)
* open FsHttp
  open FsHttp.Dsl
  open FsHttp.Fsi
* aufteilen in: Basic und Advanced / Production and FSI
* DSL: If you want it all sync, use just "go". If you want to control sync/async, use "id .> go" or "id >. go"
* builder operations auch camelCase
* make functions 'inline' when using send inside
* -- und |> sind gleich
* Adressing B and H explicitly
* Body besser erklären
* Multipart
