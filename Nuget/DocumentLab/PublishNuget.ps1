$nugetServer = "https://nuget.org"
$apiKey = "oy2cfwztwryfk2bp4rma6wd6sp62dxme5lgcvjk25n3hvm"

$package = get-childitem | where {$_.extension -eq ".nupkg"}
nuget push -Source $nugetServer $package $apiKey