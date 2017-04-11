properties {
  $buildConfig = "Release"

  $projectName = "Supertext.Sdl.Trados.FileType.$projectNameParam"
  $pluginName1 = "$projectName.sdlplugin"
  $pluginFolder1 = "$projectName"

  $baseDir = Resolve-Path ../..
  $publishDir = "$baseDir\plugin"
  $publishDirPlugin1 = "$publishDir\$pluginName1"
  $pluginPath1 =  "$baseDir\$projectName\bin\$buildConfig\$pluginName1"
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
  if(Test-Path  $publishDirPlugin1){
    Write-Host "deleting" $publishDirPlugin1 
    Remove-Item $publishDirPlugin1 -Force -Recurse
  }
  
  $pluginFolders | ForEach-Object{
    $pluginFolder = $_
	
	if(Test-Path  $pluginFolder){
      Write-Host "deleting" $pluginFolder
      Remove-Item $pluginFolder -Force -Recurse
    }
  }
}