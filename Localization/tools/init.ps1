param($installPath, $toolsPath, $package)

#Check for compatibility with the power shell installed
if ($PSVersionTable.PSVersion -lt '3.0')
{
    Write-Host "PowerShell Version not compatible with incatechnologies tools."
	Write-Host "Update to 3.0 or superior."
    return
}
 #Try to retrive the module if is already installed
$module = Get-Module IncaTechnologies.Localization.Tools

#If no module is installed then is installed, else is updated just to be sure to have the latest version
if($module -eq $null){
	dotnet new tool-manifest --verbosity minimal
	dotnet tool install IncaTechnologies.Localization.Tools --verbosity minimal
}
else {
	dotnet tool update IncaTechnologies.Localization.Tools --local --verbosity minimal
}

#Imports the module to be used in the Package Manager Console
Import-Module (Join-Path $toolsPath incatechnologies.localization.tools.psm1)



