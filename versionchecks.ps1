write-host ("GITD  {0,-20} {1}" -f (git describe --tags --always),(git rev-parse --abbrev-ref HEAD))
write-host ("JSON  {0}" -f (gc Properties/Info.json | ConvertFrom-Json | Select -Expand Version))
