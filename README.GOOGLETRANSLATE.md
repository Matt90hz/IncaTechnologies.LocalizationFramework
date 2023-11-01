# IncaTechnologies.LocalizationFramework.Tools

## Important note

This tool is complementary of [IncaTechnologies.LocalizeFramework](https://www.nuget.org/packages/IncaTechnologies).

## What does it do?

It looks for all .incaloc files ino the specified folder and subfolders and translate the missig cultures using Google Translate.

```PowerShell
dotnet inca-loc-google [-Verbose] [-Input <String>] [-From <String>] [<CommonParameters>]
```

## Notes

This package uses [Google Cloud Translation API](https://cloud.google.com/translate/docs/). To avoid any complaint it will not be published in NuGet. You can download the source code and compile it yourself.


