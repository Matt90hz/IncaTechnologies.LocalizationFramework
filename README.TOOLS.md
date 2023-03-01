# IncaTechnologies.LocalizationFramework.Tools

## Important note

This tool is complementary of [IncaTechnologies.LocalizeFramework](https://www.nuget.org/packages/IncaTechnologies).
It is also automatically installed when the  LocalizeFramework is installed.
You might want to install it directly, in case of versioning controls or because of unexpected erros during the installation process.

## What does it do?

It generates .incaloc file based on the properties decorated with `IncaLocalizeAttribute`.

```PowerShell
dotnet inca-loc [-Diagnostic] [-Input <String>] [-Output <String>] [-Generator <String>] [-Cultures <String>] [<CommonParameters>]
```

The *-Diagnostic* parameter is for development purposes only, not really useful in any other circumstances. 

The *-Generator* parameter is in development and, if specified, it might not work or it might cause errors, **do not use it**.




