@echo off

call "..\psake\psake.cmd" "deploy.ps1 -parameters @{\"projectNameParam\"=\"PoFile\";\"versionParam\"=\"15\"}"

pause