@echo off

call "..\psake\psake.cmd" "deploy.ps1 -parameters @{\"projectNameParam\"=\"JsonFile\";\"versionParam\"=\"14\"}"

pause