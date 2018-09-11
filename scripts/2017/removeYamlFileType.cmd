@echo off

call "..\psake\psake.cmd" "clean.ps1 -parameters @{\"projectNameParam\"=\"YamlFile\";\"versionParam\"=\"14\"}"

pause