# BetterINI
A C#/.NET Standard INI file parser and serializer.

## Getting Started
Throw `using BetterINI;` somewhere.

### IniFile
To get a new `IniFile`, call `IniFile.Parse()`. Give it a stream or string, and you will get a useful representation of the key=value pairs found in the data.

### Serialization
If you want to deserialize an `IniFile` into an object, Call `IniSerializer.Deserialize<T>()` with the type you want.

Ensure that fields in the type are marked with the `[IniParam]` attribute.

## License
MIT License.