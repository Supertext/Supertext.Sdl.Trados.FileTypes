function removePlugin(){
[CmdletBinding()]
Param(
  [Parameter(Mandatory=$True)]
  [string]$publishDirPlugin,
  
  [Parameter(Mandatory=$True)]
  [string[]]$pluginFolders
)

if(Test-Path  $publishDirPlugin){
    Write-Host "deleting" $publishDirPlugin 
    Remove-Item $publishDirPlugin -Force -Recurse
  }
  
  $pluginFolders | ForEach-Object{
    $pluginFolder = $_
	
	if(Test-Path  $pluginFolder){
      Write-Host "deleting" $pluginFolder
      Remove-Item $pluginFolder -Force -Recurse
    }
  }
}