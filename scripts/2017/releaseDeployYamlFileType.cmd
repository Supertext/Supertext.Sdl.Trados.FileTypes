@echo off

call "..\psake\psake.cmd" "deploy.ps1 -parameters @{\"projectNameParam\"=\"YamlFile\";\"versionParam\"=\"14\"}"

pause