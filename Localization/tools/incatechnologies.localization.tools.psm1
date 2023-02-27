<#
.SYNOPSIS
    Creates incaloc files.

.DESCRIPTION
    This tool can be used to create files .incaloc that will be readed whit an IIncalocReader to localize projects.
    
#>
function Invoke-Localization{
    [CmdletBinding(PositionalBinding = $false)]
    param(
        
        [switch]
        # Development utility feature.
        $Diagnostic,
        [string]
        # Project folder. Default value is Default project folder.
        $Input,     
        [string] 
        # Where the localization file will be stored. Defaut value .\Localization folder.
        $Output,
        [string]
        #Define a custom generator for the creation of the files.
        $Generator,
        [string] 
        # All the culture codes separated by commas (ex: -c "en-EN, fr-FR, es-ES")
        $Cultures)

    $params = 'dotnet', 'inca-loc'

    if($VerbosePreference){
        $params += '-v'
    }

    if($Diagnostic){
        $params += '--diagnostic'
    }

    if($Input){
        $params += '-i', $Input
    }
    else{
        $params += '-i', ((Get-Project).FullName | Split-Path)
    }

    if($Output){
        $params += '-o', $Output
    }
    else{
         $params += '-o', (((Get-Project).FullName | Split-Path) + '\Localization')
    }

    if($Generator){
        $params += '-g', $Generator
    }

    if($Cultures){
        $params += '-c', $Cultures
    }


    dotnet inca-loc $params
}
Export-ModuleMember Invoke-Localization
