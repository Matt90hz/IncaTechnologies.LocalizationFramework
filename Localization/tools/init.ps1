param($installPath, $toolsPath, $package)

#Check for compatibility with the power shell installed
if ($PSVersionTable.PSVersion -lt '3.0')
{
    Write-Host "PowerShell Version not compatible with incatechnologies tools."
	Write-Host "Update to 3.0 or superior."
    return
}

#Install or update the latest version of the tool
dotnet new tool-manifest --verbosity quiet
dotnet tool update IncaTechnologies.LocalizationFramework.Tools --local


#Imports the module to be used in the Package Manager Console
Import-Module (Join-Path $toolsPath incatechnologies.localization.tools.psm1)




