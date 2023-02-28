# IncaTechnologies.LocalizationFramework

## What it does?

I created this piece of software to manage localization more easily then using resource file.
The general idea is to have xml files (*.incaloc) that contanis the localized text and a C# class that retrive the text when needed.

## How to use it?

The main two classes of the framework are `IncaLocReader` and `IncLocGenerator`. The first one is used to retrive the localized text, the second to generate .incaloc files that will contain the localized text.

### How to use IncaReader?

`IncaLocReader` is the default concrete implementation of `IIncaLocReader`. Of it, can be created a custom version that works in a different way or it can be persolanlized by overrinding its methods and properties.

To use the default implementation your project must embed .incaloc files as a resource that reflects the properties that has to be localized. This file must be an xml file that is named as [NameSpace].[Class].incaloc, the content must match the following pattern:

```xml
<?xml version="1.0" encoding="utf-8"?>
<IncaTehcnologies NameSpace="MyNameSpace" Class="MyClass">
  <Localize Property="FirstProperty">
    <en-EN>Text</en-EN>
    <fr-FR>Texte</fr-FR>
    <es-ES>Texto</es-ES>
  </Localize>
  <Localize Property="SecondProperty">
    <en-EN>...</en-EN>
    <fr-FR>...</fr-FR>
    <es-ES>...</es-ES>
  </Localize>
</IncaTehcnologies>
```

So the root element must be *IncaTchnologies* and has two attributes *NameSpace* and *Class*. Inside this element are contained as many *Localize* elements as are need.
Every *Localize* element has a *Property* attribute that points to the property that has to be localized and contains elements named after the culture codes for wich the localization is avaliable.
In the snippet are included codes for english, spanish and franch but any culture code can be included.

`IncaLocReader` has been created to work with MVVM design pattern and it might be incorporeted in your code like this:

```csharp
public abstract class _ViewModelBase
{
    protected virtual string GetText([CallerMemberName] string? propertyName = null)
    {
        return IncaLocService.IncaLocReader.GetText(new IncaLocParameters(
            nameSpace: this.GetType().Namespace,
            classIdentifier: this.GetType().Name,
            propertyIdentifier: propertyName));
    }
}

public class MyViewModel : _ViewModelBase
{
    public string? FirstProperty => GetText();

    public string? SecondProperty => GetText();
}
```

The `GetText(IncaLocParameters param)` method of `IncaLocReader` will automatically look for an .incaloc file embedded in the project whose name matches the invoker class and retrive the text correspondent to the culture specified in the property `IncaLocReader.CurrentCulture`.

In the snippet is used `IncaLocService` that is just an utility class that returns a singleton instance of `IncaLocReader`. It is not required to use this class to get an instance of `IncaLocReader`.  

### How to use IncaLocGenerator?

So far it looks like that use `IncaLocReader` requires quite a work since .incaloc files must be generated end embedded to the project.
Here is were `IncaLocGenerator` come into place. As for the reader it can be customized and personalized.
It has a method that generates .incaloc files but use it directly is not particularly useful since the method must know all the properties (including namespacees and classes) for the files to be generated.

This object is ment to be used in a tool combined with the `IncaLocalizeAttribute`.
The tool has aready been created and it is installed (locally) automatically when this framework is installed via Nuget package. 
If for some reason the installation fails it is possible to install the tool directly from dotnet client.
Once installed it possible call the tool form the dotnet client (by default is installed locally in the directory of the solution and only from that directory or subdirecotories can be invoked):
```
dotnet inca-loc [-Diagnostic] [-Input <String>] [-Output <String>] [-Generator <String>] [-Cultures <String>]
```

Otherwise if Visual Studio is used for development it can be invoked using inside the Package Manager Console:
```
Invoke-Localization [-Diagnostic] [-Input <String>] [-Output <String>] [-Generator <String>] [-Cultures <String>] [<CommonParameters>]
```

In both cases the tool will look inside all the .cs files in a project folder and automatically generates the .incaloc file for every property that is decorated with `IncaLocalizeAttribute`.
It also automatically embbed the file to the project as a resource.

Example:
```csharp
public class MyViewModel : _ViewModelBase
{
    [IncaLocalize]
    public string? FirstProperty => GetText();

    [IncaLocalize]
    public string? SecondProperty => GetText();
}
```

Call `Invoke-Localization` and the followiong file will be create in the folder .\MyProject\Localization .

```xml
<?xml version="1.0" encoding="utf-8"?>
<IncaTehcnologies NameSpace="MyNameSpace" Class="MyClass">
  <Localize Property="FirstProperty">
    <en-EN></en-EN>
  </Localize>
  <Localize Property="SecondProperty">
    <en-EN></en-EN>
  </Localize>
</IncaTehcnologies>
```

If cultures where specified by invoking for example:

```
Invoke-Localization -Cultures "en-US, it-IT"
```

The .incaloc file would be:
```xml
<?xml version="1.0" encoding="utf-8"?>
<IncaTehcnologies NameSpace="MyNameSpace" Class="MyClass">
  <Localize Property="FirstProperty">
    <en-US></en-US>
    <it-IT></it-IT>
  </Localize>
  <Localize Property="SecondProperty">
    <en-US></en-US>
    <it-IT></it-IT>
  </Localize>
</IncaTehcnologies>
```

The same results can be achieved invoking the tool form the dotnet client like this:
```
PS: C:\MyCoolProject\MyProject> dotnet inca-loc -Cultures "en-US, it-IT"
```

## Author Remarks

This project is tailor made on my needings but I thought that maybe sameone else might be interested in a solution like this for for the localization, or better for the support of multiple leguages in their projects.
I also mede this project to study more in depth how dotnet tools works, how Nuget packages works, how to write commandlet, how to write analyzers and how code generation works.
My ultimate goal was not only crete a framework that I needed but I was also intrested in find out if I was able to crete a similar experience to the one provided by the Microsoft.EntityFrameworkCore.Tools and CommunityToolkit.Mvvm.

In the next update if ever there will be one, I would like to use code generators in order to create programmatically the properties that has to be localized. This is the result I would like to get:
```csharp
public partial class MyViewModel : _ViewModelBase
{
    [IncaAutoLocalize]
    string? FirstProperty;

    [IncaAutoLocalize]
    string? SecondProperty;
}

//Generated
partial class MyViewModel
{
    protected virtual string GetText([CallerMemberName] string? propertyName = null)
    {
        return IncaLocService.IncaLocReader.GetText(new IncaLocParameters(
            nameSpace: this.GetType().Namespace,
            classIdentifier: this.GetType().Name,
            propertyIdentifier: propertyName));
    }

    public string? FirstProperty => GetText();

    public string? SecondProperty => GetText();

}
```
Or something like this at least.

I actually explored a lot of possible solution to get around this project and I ended up with a different approaches than the ones used in the framework that inspired me. 
None the less I am pretty happy of the results that I got.

I anyone wants to contribute, give avise or point out a better strategy to achive what this framework does, you are welcome sir.
You can do it through GitHub at any time.
