Add-Type -AssemblyName System.Windows.Forms

$PBConfig = [xml]@'
<GameInfo Name="Planetbase">
	<Folder>Planetbase</Folder>
	<ModsDirectory>Mods</ModsDirectory>
	<ModInfo>Info.json</ModInfo>
	<GameExe>Planetbase.exe</GameExe>
	<EntryPoint>[UnityEngine.UI.dll]UnityEngine.Canvas.cctor:Before</EntryPoint>
	<StartingPoint>[Assembly-CSharp.dll]Planetbase.ErrorManager.init:Before</StartingPoint>
	<MinimalManagerVersion>0.22.3</MinimalManagerVersion>
</GameInfo>
'@


function Main {
    Write-Host "IPTUMM v1.1.0"
    Write-Host "Asking for file location"

    $FileBrowser = New-Object Windows.Forms.OpenFileDialog -Property @{
	    Filter = "Config File|UnityModManagerConfig.xml|All XML|*.xml|All Files|*.*";
	    ShowHelp = $false;
	    Title = "Select configuration file..."
    }

    if ($FileBrowser.ShowDialog() -eq [Windows.Forms.DialogResult]::Ok) {
	    Write-Host "File: $($FileBrowser.FileName)"

	    $configFile = [xml](get-content $FileBrowser.FileName)
	    $PBNode = $configFile.Config.GameInfo | Where-Object{
		    $_.GetAttribute("Name") -eq "Planetbase" }

	    if ($PBNode -eq $null) {
            Write-Host "Generating Planetbase node..."
            GeneratePBNode

		    $configFile.Save($FileBrowser.FileName + ".bak")
		    Write-Host $BackupString
		    $null = $configFile.Config.AppendChild($newNode)
		    $configFile.Save($FileBrowser.FileName)
		    Write-Host $SaveString
	    } else {
            $changed = $false

            foreach ($node in $PBConfig.GameInfo.ChildNodes) {
                $segment = $node.LocalName
                $PBInner = $PBNode.$segment

			    if ($PBInner -ne $node.InnerText -or $PBInner -eq $null) {
                    Write-Host "Config file out of date. Updating..."
		            $configFile.Save($FileBrowser.FileName + ".bak")
		            Write-Host $BackupString


                    Write-Host "Generating a new Planetbase node..."
                    GeneratePBNode
                    # Don't store the replaced node, but don't propagate either
                    $null = $configFile.Config.ReplaceChild( $newNode, $PBNode )

		            $configFile.Save($FileBrowser.FileName)
		            Write-Host $SaveString
                    $changed = $true
                    break
                }
		    }

            if (!$changed) {
                Write-Host "Config file already contains an entry for Planetbase."
            }
	    }
    }

    Write-Host "Script done. Press any key to continue."
    $null = $host.ui.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}


function GeneratePBNode {
	$script:newNode = $configFile.CreateElement("GameInfo")
	$newNode.SetAttribute("Name", "Planetbase")

	foreach ($node in $PBConfig.GameInfo.ChildNodes) {
		$tmp = $configFile.CreateElement( $node.LocalName )
		$tmp.InnerText = $node.InnerText
		$null = $newNode.AppendChild($tmp)
	}
}


$BackupString = "Backed up config file."
$SaveString = "Saved new config."
Main
