param (
    [string[]]$mask = @(throw "Must specify a -mask parameter (eg. -mask web.config, app.config)")
)

<#
  call as follows:

  VSTSConfig.ps1 -mask app.config web.config
  VSTSConfig.ps1 -mask *.cscfg
#>

$SourcePath = (Get-Item -Path ".\" -Verbose).FullName

$testPath = Test-Path $SourcePath

$regex = "__[A-Za-z0-9.]*__"
$matches = @()


if($testPath)
{
	Write-Output "Path exists: $SourcePath"
	
	$list = Get-ChildItem $SourcePath -recurse -Include $mask

	Foreach($file in $list)
	{
		$destinationPath = $file.FullName

    	Write-Host "Processing $destinationPath"

		$tempFile = join-path $file.DirectoryName ($file.BaseName + ".tmp")
		
		Copy-Item -Force $file.FullName $tempFile

		$matches = select-string -Path $tempFile -Pattern $regex -AllMatches | % { $_.Matches } | % { $_.Value }
		
		ForEach($match in $matches)
		{
		  $matchedItem = $match
		  $matchedItem = $matchedItem.Trim('_')
		  $matchedItem = $matchedItem -replace '\.','_'
		  (Get-Content $tempFile) | 
		  Foreach-Object {
			$_ -replace $match,(get-item env:$matchedItem).Value
		  } | 
		Set-Content $tempFile -Force
		}

		Copy-Item -Force $tempFile $DestinationPath
		Remove-Item -Force $tempFile
	}
}
else
{
	Write-Output "Path Does Not Exist"
}