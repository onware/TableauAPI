You will need the NuGet Packager Visual Studio Extension in order to open the
Packager project: https://visualstudiogallery.msdn.microsoft.com/daf5c6db-386b-4994-bdd7-b6cd52f11b72

API Key can be found under 3rd Party Services in SharePoint.

Had to make modifications to the NuGetPackage.ps1 file based on a post on the NuGetPackager Q&A section
'apikeys' to push package on a server
1 Posts | Last post August 07, 2015

Written August 07, 2015
Jérôme HUGON
When trying to push the generated package on a remote server automatically after a build, I notice that the 'apikeys' in the NuGet.config file were not used by NuGetPackage.ps1. So packages are never send to a remote server.

Maybe I there is something that I've missed but my solution (based on version 2.1.1) is to:

- Insert at line 227:
$serverkey = ""
$nugetConfig.configuration.apikeys.add | ForEach-Object {
	if ($_.key -eq $url) {
		$serverkey = $_.value
	}
}
Write-Log "Repository key: $serverkey"

- Replace line 239 (line 232 before insertion above) with :
$task = Create-Process .\NuGet.exe ("push " + $_.Name + " -Source " + $url + " " + $serverkey)

Is there another way to push packages on a remote server using 'apikeys' section ?