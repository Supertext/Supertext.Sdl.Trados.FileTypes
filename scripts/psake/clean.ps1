Include ".\utils.ps1"

properties {

  $projectName = "Supertext.Sdl.Trados.FileType.$projectNameParam"
  $pluginName1 = "$projectName.sdlplugin"
  $pluginFolder1 = "$projectName"

  $baseDir = Resolve-Path ../..
  $publishDir = "$baseDir\plugin"
  $publishDirPlugin1 = "$publishDir\$pluginName1"
  $appDataRoaming = $env:APPDATA
  $appDataLocal = $env:LOCALAPPDATA
  $deployDir = "$appDataLocal\SDL\SDL Trados Studio\14\Plugins\Packages"
  $pluginFolders = @(
	"$appDataLocal\SDL\SDL Trados Studio\14\Plugins\Packages\$pluginName1",
	"$appDataLocal\SDL\SDL Trados Studio\14\Plugins\Unpacked\$pluginFolder1",
    "$appDataRoaming\SDL\SDL Trados Studio\14\Plugins\Packages\$pluginName1",
    "$appDataRoaming\SDL\SDL Trados Studio\14\Plugins\Unpacked\$pluginFolder1"
  )

}

task default -depends Clean

task Clean { 
  removePlugin $publishDirPlugin1 $pluginFolders
}