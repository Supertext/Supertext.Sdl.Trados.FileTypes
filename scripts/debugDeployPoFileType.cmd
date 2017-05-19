@echo off

call ".\psake\psake.cmd" "deploy.ps1 -properties @{\"buildConfig\"=\"Debug\"}" -parameters @{\"projectNameParam\"=\"PoFile\"}"

pause