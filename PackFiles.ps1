param (
	[string]$OutputDir,
	[string]$SolutionDir = ".",
	[string]$TargetFileName,
	[string]$TargetFileExt = ".dll",
	[string]$Project = "none",
	[string]$BuildConfig,
	[switch]$WhatIf = $false
)

$Errors = @{
	BadDirectory = 2
}
$copyItems = @(
	"$OutputDir/$TargetFileName$TargetFileExt",
	"$OutputDir/Properties/Info.json"
)


if (!(Test-Path $SolutionDir)) {
	exit $Errors.BadDirectory }
if ($OutputDir -eq "" -or !(Test-Path $OutputDir)) {
	exit $Errors.BadDirectory }

$workingDir = "$SolutionDir/Output"
if (!(Test-Path $workingDir)) {
	mkdir -force $workingDir -WhatIf:$WhatIf | Foreach-Object {
		Write-Host "Created directory: $_"
	}
}
copy-item $copyItems[0] "$workingDir/$TargetFileName-$BuildConfig$TargetFileExt" `
	-WhatIf:$WhatIf `
	-ea Continue

$workingDir = "$SolutionDir/Output/$Project"
if (Test-Path $workingDir) {
	remove-item $workingDir -force -recurse -WhatIf:$WhatIf
}

mkdir -force $workingDir -WhatIf:$WhatIf | Foreach-Object {
	Write-Host "Created directory: $_"
}
foreach ($file in $copyItems) { 
	try {
		copy-item $file $workingDir `
			-WhatIf:$WhatIf `
			-ea stop
	} catch [Management.Automation.ItemNotFoundException] {
		Write-Host $_
	}
}