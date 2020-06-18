$gitVer = git describe --tags --always
$gitBranch = git rev-parse --abbrev-ref HEAD
$jsonVer = [Version](gc Properties/Info.json | ConvertFrom-Json | Select -Expand Version)
$VersionsMatch = if ($gitVer -like "$($jsonVer.ToString(3))*") { [char]8730 }

write-host ("GITD  {0,-20} {1}" -f $gitVer,$gitBranch)
write-host ("JSON  {0,-20} " -f $jsonVer) -nonewline
write-host $VersionsMatch -Fore Green
