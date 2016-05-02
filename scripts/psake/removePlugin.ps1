properties {
  $buildConfig = "Release"

  $projectName = "Supertext.Sdl.Trados.FileType.$projectNameParam"
  $pluginName1 = "$projectName.sdlplugin"
  $pluginName2 = "$projectName.WinUI.sdlplugin"
  $pluginFolder1 = "$projectName"
  $pluginFolder2 = "$projectName.WinUI"

  $baseDir = Resolve-Path ../..
  $publishDir = "$baseDir\plugin"
  $publishDirPlugin1 = "$publishDir\$pluginName1"
  $publishDirPlugin2 = "$publishDir\$pluginName2"
  $pluginPath1 =  "$baseDir\$projectName\bin\$buildConfig\$pluginName1"
  $pluginPath2 =  "$baseDir\$projectName.WinUI\bin\$buildConfig\$pluginName2"
  $appDataRoaming = $env:APPDATA
  $appDataLocal = $env:LOCALAPPDATA
  $deployDir = "$appDataLocal\SDL\SDL Trados Studio\12\Plugins\Packages"
  $pluginFolders = @(
	"$appDataLocal\SDL\SDL Trados Studio\12\Plugins\Packages\$pluginName1",
    "$appDataLocal\SDL\SDL Trados Studio\12\Plugins\Packages\$pluginName2",
	"$appDataLocal\SDL\SDL Trados Studio\12\Plugins\Unpacked\$pluginFolder1",
    "$appDataLocal\SDL\SDL Trados Studio\12\Plugins\Unpacked\$pluginFolder2",
    "$appDataRoaming\SDL\SDL Trados Studio\12\Plugins\Packages\$pluginName1",
    "$appDataRoaming\SDL\SDL Trados Studio\12\Plugins\Packages\$pluginName2",
    "$appDataRoaming\SDL\SDL Trados Studio\12\Plugins\Unpacked\$pluginFolder1",
    "$appDataRoaming\SDL\SDL Trados Studio\12\Plugins\Unpacked\$pluginFolder2"
  )

}

task default -depends Clean

task Clean { 
  if(Test-Path  $publishDirPlugin1){
    Write-Host "deleting" $publishDirPlugin1 
    Remove-Item $publishDirPlugin1 -Force -Recurse
  }
  
  if(Test-Path  $publishDirPlugin2){
    Write-Host "deleting" $publishDirPlugin2 
    Remove-Item $publishDirPlugin2 -Force -Recurse
  }
  
  $pluginFolders | ForEach-Object{
    $pluginFolder = $_
	
	if(Test-Path  $pluginFolder){
      Write-Host "deleting" $pluginFolder
      Remove-Item $pluginFolder -Force -Recurse
    }
  }
}