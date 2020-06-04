Add-Type -AssemblyName System.Windows.Forms

Write-Host "Asking for file location"

$FileBrowser = New-Object Windows.Forms.OpenFileDialog -Property @{
	Filter = "Config File|UnityModManagerConfig.xml|All XML|*.xml|All Files|*.*";
	ShowHelp = $false;
	Title = "Select configuration file..."
}

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

if ($FileBrowser.ShowDialog() -eq [Windows.Forms.DialogResult]::Ok) {
	Write-Host "File: $($FileBrowser.FileName)"
	
	$configFile = [xml](get-content $FileBrowser.FileName)
	$PBNode = $configFile.Config.GameInfo | Where-Object{
		$_.GetAttribute("Name") -eq "Planetbase" }
		
	if ($PBNode -eq $null) {
		$newNode = $configFile.CreateElement("GameInfo")
		$newNode.SetAttribute("Name", "Planetbase")
		
		foreach ($node in $PBConfig.GameInfo.ChildNodes) {
			$tmp = $configFile.CreateElement( $node.LocalName )
			$tmp.InnerText = $node.InnerText
			$newNode.AppendChild($tmp)
		}
		
		$configFile.Save($FileBrowser.FileName + ".bak")
		Write-Host "Backed up config file."
		$configFile.Config.AppendChild($newNode)
		$configFile.Save($FileBrowser.FileName)
		Write-Host "Saved new config."
	} else {
		Write-Host "Config file already contains an entry for Planetbase."
	}
}

Write-Host "Script done. Press any key to continue."
$null = $host.ui.RawUI.ReadKey("NoEcho,IncludeKeyDown")
