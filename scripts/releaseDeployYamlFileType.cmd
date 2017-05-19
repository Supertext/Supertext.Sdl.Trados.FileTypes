@echo off

call ".\psake\psake.cmd" "deploy.ps1 -parameters @{\"projectNameParam\"=\"YamlFile\"}"

pause