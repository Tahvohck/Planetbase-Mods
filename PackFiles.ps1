param (
	[string]$OutputDir,
	[string]$SolutionDir = ".",
	[string]$TargetFileName,
	[string]$TargetFileExt = ".dll",
	[string]$Project = "none",
	[string]$BuildConfig,
	[switch]$WhatIf = $false
)

########################################
# Configuration
$Errors = @{
	BadDirectory = 2
}
$MetaFile = "Info.json"
$copyItems = @(
	"$OutputDir/$TargetFileName$TargetFileExt",
	"$OutputDir/Properties/$MetaFile"
)
$PBMBH = get-command "$SolutionDir/Output/PBMBH/PBModBuildHelper.exe" -ea SilentlyContinue


########################################
# Code start
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

if ($PBMBH -ne $null -and (Test-Path "$workingDir/$MetaFile")) {
    $entryMethod = & $PBMBH $copyItems[0]
    if ($entryMethod -notlike "ERROR*") {
        $json = gc "$workingDir/$MetaFile" | ConvertFrom-Json
        $json.EntryMethod = $entryMethod
        $json | ConvertTo-Json | out-file -encoding ascii "$workingDir/$MetaFile"
    } else {
        Write-Error $entryMethod
    }
} else {
    Write-Host "PBMBH missing."
}
