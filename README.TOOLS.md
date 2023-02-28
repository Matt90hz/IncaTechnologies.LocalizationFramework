# IncaTechnologies.LocalizationFramework.Tools

## Important note

This tool is complementary with the use of [IncaTechnologies.LocalizeFramework](https://www.nuget.org/packages/IncaTechnologies).
It is also automatically installed when the  LocalizeFramework is installed.
You might want to install directly this tool only to control version or in case of erros during the installation process.

## What does it do?

It generates .incaloc file based on the properties decorated with `IncaLocalizeAttribute`.

```PowerShell
dotnet inca-loc [-Diagnostic] [-Input <String>] [-Output <String>] [-Generator <String>] [-Cultures <String>] [<CommonParameters>]
```

The *-Diagnostic* parameter is for development pourposes only, not really useful in any other circumstances. 

The *-Generator* parameter is in development and if specified it might not work or cause erros, **do not use it**.




