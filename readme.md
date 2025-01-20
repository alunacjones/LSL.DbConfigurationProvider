[![Build status](https://img.shields.io/appveyor/ci/alunacjones/lsl-dbconfigurationprovider.svg)](https://ci.appveyor.com/project/alunacjones/lsl-dbconfigurationprovider)
[![Coveralls branch](https://img.shields.io/coverallsCoverage/github/alunacjones/LSL.DbConfigurationProvider)](https://coveralls.io/github/alunacjones/LSL.DbConfigurationProvider)
[![NuGet](https://img.shields.io/nuget/v/LSL.DbConfigurationProvider.svg)](https://www.nuget.org/packages/LSL.DbConfigurationProvider/)

# LSL.DbConfigurationProvider

Provides a `ConfigurationProvider` that reads settings from a database table.

## Configuring with the default settings

The following example sets up a DB configuration provider that fetches the settings from a table called `Settings` with a key field called `Key` and a value field called `Value`.

```csharp
var builder = new ConfigurationBuilder();
builder.AddDbConfiguration(
    () => new SqlConnection("my-connection-string"));
```

## Configuring with the custom settings

The following example sets up a DB configuration provider that fetches the settings from a table called `OtherSettings` with a key field called `OtherKey` and a value field called `OtherValue`.

```csharp
var builder = new ConfigurationBuilder();
builder.AddDbConfiguration(
    () => new SqlConnection("my-connection-string"), 
    "OtherSettings", 
    "OtherKey", 
    "OtherValue");
```

## Configuring with a key prefix

The following example sets up a DB configuration provider that only fetches settings whose keys are prefixed with the value of the `keyPrefix` parameter.

```csharp
var builder = new ConfigurationBuilder();
builder.AddDbConfiguration(
    () => new SqlConnection("my-connection-string"), 
    keyPrefix: "my-application:");
```

All returned settings will have the prefix automatically removed from the key name.

## Dealing with errors on load

An optional paramter `onLoadError` allows for the passing of a delegate to decide what to do when an error occurs on loading settings from the DB.

```csharp
var builder = new ConfigurationBuilder();
builder.AddDbConfiguration(
    () => new SqlConnection("my-connection-string"), 
    onLoadError: context =>
    {
        Console.WriteLine(context.Exception.ToString());

        // NOTE: The following is not required really
        // as RethrowException defaults to false.
        // It is set here only to show that this is an option.
        context.RethrowException = false;
    });
```

### onLoadError Context

The `onLoadError` delegate is passed a context that has the following members available:

#### Exception 

This readonly property provides the exception that was thrown so that an error handler can
act appropriately based on what was thrown.

#### RethrowException

This property defaults to `false` but can be set to `true` in order to throw an exception
of type `DbConfigurationProviderLoadException` so that a consuming application can react to 
the problem and inspect the `InnerException` and act accordingly.

> **NOTE**: The `InnerException` is the instance of the originally thrown exception.