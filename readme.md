[![Build status](https://img.shields.io/appveyor/ci/alunacjones/lsl-dbconfigurationprovider.svg)](https://ci.appveyor.com/project/alunacjones/lsl-dbconfigurationprovider)
[![Coveralls branch](https://img.shields.io/coverallsCoverage/github/alunacjones/LSL.DbConfigurationProvider)](https://coveralls.io/github/alunacjones/LSL.DbConfigurationProvider)
[![NuGet](https://img.shields.io/nuget/v/LSL.DbConfigurationProvider.svg)](https://www.nuget.org/packages/LSL.DbConfigurationProvider/)

# LSL.DbConfigurationProvider

Provides a `ConfigurationProvider` that reads settings from a database table.

## Configuring with the default settings

The following example sets up a DB configuration provider that fetches the settings from a table called `Settings` with a key field called `Key` and a value field called `Value`.

```csharp
var ctx = new MyDbContext(); // Using an EF Core context here
var builder = new ConfigurationBuilder();
builder.AddDbConfiguration(() => ctx.Database.GetDbConnection());
```

## Configuring with the custom settings

The following example sets up a DB configuration provider that fetches the settings from a table called `OtherSettings` with a key field called `OtherKey` and a value field called `OtherValue`.

```csharp
var ctx = new MyDbContext(); // Using an EF Core context here
var builder = new ConfigurationBuilder();
builder.AddDbConfiguration(() => ctx.Database.GetDbConnection(), "OtherSettings", "OtherKey", "OtherValue");
```

