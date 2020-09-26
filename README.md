# BetterINI
A C#/.NET Standard INI file parser and serializer.

## Getting Started
Throw `using BetterINI;` somewhere.

### IniFile
To get a new `IniFile`, call `IniFile.Parse()`. Give it a stream or string, and you will get a useful representation of the key=value pairs found in the data.

### Serialization
If you want to deserialize an `IniFile` into an object, Call `IniSerializer.Deserialize<T>()` with the type you want.

Ensure that fields in the type are marked with the `[IniParam]` attribute.

### Examples
Assume the INI file contains this:
```ini
FName  = The
LName  = Terminator
Height = 6.2
Weight = 249
```

Here is the C# type we want to use:
```csharp
public class Human
{
    [IniParam("FName")] public string firstName;
    [IniParam("LName")] public string lastName;

    [IniParam("Height")] public float height;
    [IniParam("Weight")] public float weight;
}
```

And here's how you deserialize the INI file we defined above:
```csharp
Human h = IniSerializer.Deserialze<Human>(iniData);
```

## License
MIT License.