
dotnet tool restore

if ($args[0] -eq $null) {
	$mode = "build"
} else {
	$mode = "watch"
}

fsdocs $mode --clean --sourcefolder ./src --input ./docs --output c:/temp/FsHttpDocs --properties Configuration=Release --sourcerepo https://github.com/ronaldschlenker/FsHttp/blob/master/src
