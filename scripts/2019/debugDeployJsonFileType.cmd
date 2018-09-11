@echo off

call "..\psake\psake.cmd" "deploy.ps1 -properties @{\"buildConfig\"=\"Debug\"}" -parameters @{\"projectNameParam\"=\"JsonFile\";\"versionParam\"=\"15\"}"

pause