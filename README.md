# Planetbase Mods
## General
Master repository for all of my Planetbase mods. All public mods are submodules with links to their correct
repositor on Github, all of which can be imported into a solution file standalone (as long as the
requirements are met). In general unless you want to download all my mods at once, you should clone one of
the submodules instead of this one.

## To Build
Built on VS2017, C# version 7.3 .NET Framework version 3.5. Most mods reference the following files:

* Assembly-CSharp.dll
* UnityEngine.dll
* UnityEngine.UI.dll
* 0Harmony.dll (Version 2, only some mods)

These mods are **not** built on the Patched version of the game. They use [Unity Mod Manager](
https://github.com/newman55/unity-mod-manager) instead. Support for Planetbase can be added to Unity Mod Manager
via the Powershell installation script I've written (UnityModManagerSetup/install-planetbase-to-umm.ps1)
just for that purpose. It will provide you with a nice file dialog to select UMM's configuration file and
make a backup before editing it.

## Utilities
 - PackFiles.ps1: utility to move output into Output folder
 - PBModBuildHelper.exe: searches for the first method in a DLL with the [LoaderOptimizationAttribute] and
   updates spits it out to the console. Used in conjunction with PackFiles.ps1 to update a given mod's
   info.json.

## JPFarias
João Farias was a major contributor to the planetbase modding community back in 2016, writing both the
original patcher and a plethora of mods. Although their mods were impressive, they used the hacky Redirector
mod to overwrite base game methods, and the VERY hacky Patcher to inject at all. Since then HarmonyLib has
come around and offered much more elegant ways to patch methods, and Unity Mod Manager has given us a more
robust modloader. I am attempting to update JPF's mods to the cleaner methods. Here is the conversion
progress:

 - [ ] Auto Alerts
 - [x] Auto Connections
 - [ ] Auto Rotate Buildings
 - [ ] Building Aligner
 - [ ] Camera Overhaul
   - [x] Basic integration (works but with issues)
   - [x] Fix skybox
   - [ ] Bugfixing
   - [ ] Remove debug code
 - [ ] CharacterCam
 - [ ] Free Building
 - [ ] Free Furnishing
 - [ ] Landing Control
 - [ ] Power Saver
 - [x] SkipIntro
   - [x] MVP
   - [x] Bugfixes
   - [ ] Polish
 
Patcher and Redirector will explicitly not be updated, as the entire point of this project is to remove
their need.
