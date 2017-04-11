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

task default -depends Deploy

task Deploy -depends Clean {
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