# IncaTechnologies.LocalizationFramework

## What does it do?

I created this piece of software to manage localization more easily then using resource file.
The general idea is to have xml files (*.incaloc) that contains the localized text and a C# class that retrieve the text when needed.

## How to use it?

The main two classes of the framework are `IncaLocReader` and `IncLocGenerator`. The first one is used to retrieve the localized text, the second one to generate .incaloc files that will contain the localized text.

### How to use IncaReader?

`IncaLocReader` is a concrete implementation of `IIncaLocReader`. It can be customized by overriding its methods and properties if needed. To use this implementation your project must embed .incaloc files as a resource that reflects the properties that shall be localized. This file must be an xml file whose name match the pattern **[NameSpace].[Class].incaloc**, the content of the file instead must match the following pattern:

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

Thus, the root element must be *IncaTchnologies* and it shall have two attributes *NameSpace* and *Class*. Inside this root element shall be contained as many *Localize* elements as are needed.
Every *Localize* element shall have a *Property* attribute that points to the property that shall be localized. It contains elements named after the culture codes for which the localization is avaliable.
In the snippet are included codes for English, Spanish and French but any culture code can be included.

`IncaLocReader.GetText(IncaLocParameters parameters)` can be used to retrive the translated text. For example, the following code can be used in a ViewModel:

```csharp
public abstract class ViewModelBase
{
    protected virtual string GetText([CallerMemberName] string? propertyName = null)
    {
        return IncaLocReader.Default.GetText(new IncaLocParameters(
            nameSpace: this.GetType().Namespace,
            classIdentifier: this.GetType().Name,
            propertyIdentifier: propertyName));
    }
}

public class MyViewModel : ViewModelBase
{
    public string FirstProperty => GetText();

    public string SecondProperty => GetText();
}
```

The interface `IIncaLocReader` comes with some handy extension methods that can be used to retrieve the localized text in a more concise way. For example, the previous code can be rewritten as:

```csharp
public class MyClass
{
    public string FirstProperty => this.GetText();

    public string SecondProperty => this.GetText();
}
```

The `GetText()` method in the first example will look for an .incaloc file embedded in the project/library that called the method. In the second example the assembly containing the object passed as parameter is used. Then the most relevant translation is retrived from the file. The most relevant translation is the one that matches the `IIncaLocReader.CurrentCulture` property. If no match is fonud the `IIncaLocReader.DefaultCulture` is used. If no match is found again, the first translation found is used. If no translation is found at all, the method returns an empty string.

### How to use IncaLocGenerator?

So far it looks like that use `IncaLocReader` requires quite a work since .incaloc files must be generated end embedded to the project.
Here is where `IncaLocGenerator` comes into place. As for the reader, it can be customized and personalized.
It has a method that generates .incaloc files but its direct usage is not particularly useful since the method must know all the properties (including namespaces and classes) for the files to be generated.

This object is ment to be used mainly in a tool combined with the `IncaLocalizeAttribute`.
The tool has already been created and it is installed (locally) automatically when this framework is installed via Nuget package. 
If for some reason the installation fails, it is possible to install the tool directly from dotnet client.
Once installed, it is possible to call the tool from the dotnet client (by default is installed locally in the directory of the solution and it can be invoked only from that directory or subdirectories):
```
dotnet inca-loc [-Diagnostic] [-Input <String>] [-Output <String>] [-Generator <String>] [-Cultures <String>]
```

Otherwise, if Visual Studio is used for development, it can be invoked inside the Package Manager Console via:
```
Invoke-Localization [-Diagnostic] [-Input <String>] [-Output <String>] [-Generator <String>] [-Cultures <String>] [<CommonParameters>]
```

In both cases the tool will look inside all the .cs files in a project folder and automatically generates the .incaloc file for every property that is decorated with `IncaLocalizeAttribute`.
It also automatically embeds the file to the project as a resource.

Example:
```csharp
public class MyClass
{
    [IncaLocalize]
    public string FirstProperty => this.GetText();

    [IncaLocalize]
    public string SecondProperty => this.GetText();
}
```

Call `Invoke-Localization` and the followiong file will be created in the folder .\MyProject\Localization .

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

If cultures where specified by invoking, for example:

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

The same results can be achieved invoking the tool form the dotnet client as:
```
PS: C:\MyCoolProject\MyProject> dotnet inca-loc -Cultures "en-US, it-IT"
```

## Author Remarks

This project is tailor made on my needings but I thought that maybe someone else might be interested in a similar solution for the localization, or better for the support of multiple languages in their own projects.
I also used this project to study more in depth how dotnet tools works, how Nuget packages works, how to write commandlet, how to write analyzers and how code generation works.
My ultimate goal was not only to create a framework that I needed but I was also interested in finding out if I was able to create a similar experience to the one provided by the Microsoft.EntityFrameworkCore.Tools and CommunityToolkit.Mvvm.

In the next update (if ever there will be one), I would like to use code generators in order to create programmatically the properties that has to be localized. This is the result I would like to get:
```csharp
public partial class MyViewModel : ViewModelBase
{
    [IncaAutoLocalize]
    string FirstProperty;

    [IncaAutoLocalize]
    string SecondProperty;
}

//Generated
partial class MyViewModel
{
    public string FirstProperty => this.GetText();

    public string SecondProperty => this.GetText();

}
```
Or something like this at least.

I actually explored a lot of possible solution to get around this project and I ended up with different approaches than the ones used in the framework that inspired me the most. 
Nonetheless I am pretty happy with the results I got.

If anyone wants to contribute, give advise or point out a better strategy to achieve what this framework does, you are very welcome sir.
You can do it through GitHub at any time.
