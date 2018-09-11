Include ".\utils.ps1"

properties {
  $buildConfig = "Release"

  $projectName = "Supertext.Sdl.Trados.FileType.$projectNameParam"
  $pluginName1 = "$projectName.sdlplugin"
  $pluginFolder1 = "$projectName"

  $baseDir = Resolve-Path ../..
  $slnFile = "$baseDir\$projectName.sln"
  $sourceDirs = "$baseDir*"
  $publishDir = "$baseDir\plugin"
  $publishDirPlugin1 = "$publishDir\$pluginName1"
  
  $appDataRoaming = $env:APPDATA
  $appDataLocal = $env:LOCALAPPDATA
  $deployDir = "$appDataLocal\SDL\SDL Trados Studio\$versionParam\Plugins\Packages"
  $pluginFolders = @(
	"$appDataLocal\SDL\SDL Trados Studio\$versionParam\Plugins\Packages\$pluginName1",
	"$appDataLocal\SDL\SDL Trados Studio\$versionParam\Plugins\Unpacked\$pluginFolder1",
    "$appDataRoaming\SDL\SDL Trados Studio\$versionParam\Plugins\Packages\$pluginName1",
    "$appDataRoaming\SDL\SDL Trados Studio\$versionParam\Plugins\Unpacked\$pluginFolder1"
  )

}

task default -depends Deploy

task Deploy -depends Clean {
  $pluginPath1 =  "$baseDir\$projectName\bin\$buildConfig\$pluginName1"
	
  if (!(Test-Path -path $publishDir)){ 
	Write-Host "creating publish dir" $publishDir 
  	New-Item $publishDir -Type Directory | out-null
  }
	
  Write-Host "copying " $pluginPath1 "to" $publishDir
  Copy-Item -Force $pluginPath1 $publishDir
	
  Write-Host "copying " $pluginPath1 "to" $deployDir
  Copy-Item -Force $pluginPath1 $deployDir
}

task Clean { 
  removePlugin $publishDirPlugin1 $pluginFolders
}
