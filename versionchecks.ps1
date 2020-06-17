write-host "GITD " (git describe --tags --always)
write-host "JSON " (gc Properties/Info.json | ConvertFrom-Json | Select -Expand Version)
