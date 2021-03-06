properties {
	$pwd = Split-Path $psake.build_script_file	
	$build_directory  = "$pwd\output\condep-dsl-operations-aws"
	$configuration = "Release"
	$preString = "-beta"
	$releaseNotes = ""
	$nuget = "$pwd\..\tools\nuget.exe"
}
 
include .\..\tools\psake_ext.ps1

function GetNugetAssemblyVersion($assemblyPath) {
	$versionInfo = Get-Item $assemblyPath | % versioninfo

	return "$($versionInfo.FileMajorPart).$($versionInfo.FileMinorPart).$($versionInfo.FileBuildPart)$preString"
}

task default -depends Build-All, Pack-All
task ci -depends Build-All, Pack-All

task Build-All -depends Clean, Build, Create-BuildSpec-ConDep-Dsl-Operations-Aws
task Pack-All -depends Pack-ConDep-Dsl-Operations-Aws

task Build {
	Exec { msbuild "$pwd\..\src\condep-dsl-operations-aws.sln" /t:Build /p:Configuration=$configuration /p:OutDir=$build_directory /p:GenerateProjectSpecificOutputFolder=true}
}

task Clean {
	Write-Host "Cleaning Build output"  -ForegroundColor Green
	Remove-Item $build_directory -Force -Recurse -ErrorAction SilentlyContinue
}

task Create-BuildSpec-ConDep-Dsl-Operations-Aws {
	Generate-Nuspec-File `
		-file "$build_directory\condep.dsl.operations.aws.nuspec" `
		-version $(GetNugetAssemblyVersion $build_directory\ConDep.Dsl.Operations.Aws\ConDep.Dsl.Operations.Aws.dll) `
		-id "ConDep.Dsl.Operations.Aws" `
		-title "ConDep.Dsl.Operations.Aws" `
		-licenseUrl "http://www.con-dep.net/license/" `
		-projectUrl "http://www.con-dep.net/" `
		-description "ConDep is a highly extendable Domain Specific Language for Continuous Deployment, Continuous Delivery and Infrastructure as Code on Windows. This package contians operations for interacting with Amazon AWS, like bootstrapping Windows servers." `
		-iconUrl "https://raw.github.com/condep/ConDep/master/images/ConDepNugetLogo.png" `
		-releaseNotes "$releaseNotes" `
		-tags "Amazon AWS VPC Bootstrap Bootstrapping Continuous Deployment Delivery Infrastructure WebDeploy Deploy msdeploy IIS automation powershell remote" `
		-dependencies @(
			@{ Name="ConDep.Execution"; Version="[5.0.0-beta,6)"},
			@{ Name="ConDep.Dsl.Operations"; Version="[5.0.0-beta,6)"},
			@{ Name="AWSSDK"; Version="[2.3.50.1]"}
		) `
		-files @(
			@{ Path="ConDep.Dsl.Operations.Aws\ConDep.Dsl.Operations.Aws.dll"; Target="lib/net40"}, 
			@{ Path="ConDep.Dsl.Operations.Aws\ConDep.Dsl.Operations.Aws.xml"; Target="lib/net40"}
		)
}

task Pack-ConDep-Dsl-Operations-Aws {
	Exec { & $nuget pack "$build_directory\condep.dsl.operations.aws.nuspec" -OutputDirectory "$build_directory" }
}