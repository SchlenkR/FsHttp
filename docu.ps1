
if ((Test-Path .\.fsdocs) -eq $true) {
	Remove-Item .\.fsdocs\ -Force -Recurse
}

dotnet tool restore
dotnet build ./src/FsHttp/FsHttp.fsproj -c Release -f net6.0

# what a hack...
if ($null -eq $args[0]) {
	$mode = "build"
} else {
	$mode = "watch"
}

dotnet fsdocs `
	$mode `
	--clean `
	--sourcefolder ./src `
	--input ./src/docs `
	--output ./docs `
	--properties Configuration=Release `
	--sourcerepo https://github.com/fsprojects/FsHttp/blob/master/src `
	--parameters `
		root /FsHttp/ `
	    fsdocs-list-of-namespaces ''
