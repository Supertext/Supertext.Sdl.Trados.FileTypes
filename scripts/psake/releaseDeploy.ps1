properties {
  $buildConfig = "Release"

  $projectName = "Supertext.Sdl.Trados.FileType.$projectNameParam"
  $pluginName1 = "$projectName.sdlplugin"
  $pluginName2 = "$projectName.WinUI.sdlplugin"
  $pluginFolder1 = "$projectName"
  $pluginFolder2 = "$projectName.WinUI"

  $baseDir = Resolve-Path ../..
  $slnFile = "$baseDir\$projectName.sln"
  $sourceDirs = "$baseDir*"
  $publishDir = "$baseDir\plugin"
  $pluginPath1 =  "$baseDir\$projectName\bin\$buildConfig\$pluginName1"
  $pluginPath2 =  "$baseDir\$projectName.WinUI\bin\$buildConfig\$pluginName2"
  $appDataRoaming = $env:APPDATA
  $appDataLocal = $env:LOCALAPPDATA
  $deployDir = "$appDataLocal\SDL\SDL Trados Studio\12\Plugins\Packages"
  $pluginFolders = @(
    "$deployDir\$pluginName1",
    "$deployDir\$pluginName2",
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

task default -depends Deploy

task Deploy -depends Clean {
  if (!(Test-Path -path $publishDir)){ 
	Write-Host "creating publish dir" $publishDir 
  	New-Item $publishDir -Type Directory | out-null
  }
	
  Write-Host "copying " $pluginPath1 "to" $publishDir
  Copy-Item $pluginPath1 $publishDir
  Write-Host "copying " $pluginPath2 "to" $publishDir
  Copy-Item $pluginPath2 $publishDir
	
  Write-Host "copying " $pluginPath1 "to" $deployDir
  Copy-Item $pluginPath1 $deployDir
  Write-Host "copying " $pluginPath2 "to" $deployDir
  Copy-Item $pluginPath2 $deployDir
}

task Clean { 
  Write-Host "deleting" $publishDir 
  Remove-Item $publishDir -Force -Recurse -ErrorAction SilentlyContinue

  $pluginFolders | ForEach-Object{
    $pluginFolder = $_

    Write-Host "deleting" $pluginFolder
    Remove-Item $pluginFolder -Force -Recurse -ErrorAction SilentlyContinue 
  }
}