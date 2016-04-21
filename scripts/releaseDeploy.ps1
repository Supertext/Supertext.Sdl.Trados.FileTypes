properties {
  $buildConfig = "Release"

  $projectName = "Supertext.Sdl.Trados.FileType.PoFile"
  $pluginName1 = "$projectName.sdlplugin"
  $pluginName2 = "$projectName.WinUI.sdlplugin"
  $pluginFolder1 = "$projectName"
  $pluginFolder2 = "$projectName.WinUI"

  $baseDir = Resolve-Path ..
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
    "$appDataRoaming\SDL\SDL Trados Studio\12\Plugins\Packages\$pluginName1",
    "$appDataRoaming\SDL\SDL Trados Studio\12\Plugins\Packages\$pluginName2",
    "$appDataLocal\SDL\SDL Trados Studio\12\Plugins\Unpacked\$pluginFolder1",
    "$appDataLocal\SDL\SDL Trados Studio\12\Plugins\Unpacked\$pluginFolder2",
    "$appDataRoaming\SDL\SDL Trados Studio\12\Plugins\Unpacked\$pluginFolder1",
    "$appDataRoaming\SDL\SDL Trados Studio\12\Plugins\Unpacked\$pluginFolder2"
  )

}

task default -depends Deploy

task Deploy -depends Test, Compile, Clean {
  if (!(Test-Path -path $publishDir)){ 
  	New-Item $publishDir -Type Directory 
  }

  Copy-Item $pluginPath1 $publishDir
  Copy-Item $pluginPath2 $publishDir

  Copy-Item $pluginPath1 $deployDir
  Copy-Item $pluginPath2 $deployDir
}

task Test -depends Compile, Clean { 
  
}

task Compile -depends Clean { 
  #Write-Host "building" $slnFile "with" $msbuildVersion
      
  #Write-Host "building using msbuild" $slnFile "/p:Configuration=$buildConfig" "/verbosity:minimal" "/fileLogger" "/fileLoggerParameters:LogFile=$baseDir/msbuild.log" $parallelBuildParam 
  
  #Exec { msbuild $slnFile "/p:Configuration=$buildConfig" "/verbosity:minimal" "/fileLogger" "/fileLoggerParameters:LogFile=$baseDir/msbuild.log"} 
}

task Clean { 

  #Get-Childitem $sourceDirs -Include bin, obj -Recurse |
  #Where {$_.psIsContainer -eq $true} |  
  #Foreach-Object {  
  #    Write-Host "deleting" $_.fullname 
  #    Remove-Item $_.fullname -Force -Recurse -ErrorAction SilentlyContinue 
  #} 
    
  Write-Host "deleting" $publishDir 
  Remove-Item $publishDir -Force -Recurse -ErrorAction SilentlyContinue

  $pluginFolders | ForEach-Object{
    $pluginFolder = $_

    Write-Host "deleting" $pluginFolder
    Remove-Item $pluginFolder -Force -Recurse -ErrorAction SilentlyContinue 
  }
  
}