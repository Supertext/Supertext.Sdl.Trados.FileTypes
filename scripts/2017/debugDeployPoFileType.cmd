@echo off

call "..\psake\psake.cmd" "deploy.ps1 -properties @{\"buildConfig\"=\"Debug\"}" -parameters @{\"projectNameParam\"=\"PoFile\";\"versionParam\"=\"14\"}"

pause