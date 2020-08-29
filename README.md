# DataQI Commons

Data Query Interface Commons is written in C# and built around essential features of the .NET Standard that provides shared infrastructure containing technology-neutral repository interfaces underpinning every DataQI Providers.

[![NuGet](https://img.shields.io/nuget/v/DataQI.Commons.svg)](https://www.nuget.org/packages/DataQI.Commons/) [![Build status](https://ci.appveyor.com/api/projects/status/rl2ja69994rt3ei6?svg=true)](https://ci.appveyor.com/project/henrique-gouveia/dataqi-commons) [![codecov](https://codecov.io/gh/henrique-gouveia/DataQI.Commons/branch/master/graph/badge.svg)](https://codecov.io/gh/henrique-gouveia/DataQI.Commons)

## Features

* Repository Interface providing standard methods
* Repository Base Factory
* Dynamic query generation from query method names
* Simple Criteria API

## Getting Started

### Installing

This library can add in to the project by way:

    dotnet add package DataQI.Commons

See [Nuget](https://www.nuget.org/packages/DataQI.Commons) for other options.

## News

**v1.1.0 - 2020/09**

* New! Added a new Criteria Query API Core
* New! Added a new Query Parser
* New! Added support for the keywords **_Containing_**, **_StartingWith_** and **_EndingWith_**
* Changed! Keyword **_Equals_** to **_Equal_**
* Changed! Keyword **_IsNull_** to **_Null_**
* Fixed! [issue3](https://github.com/henrique-gouveia/DataQI.Dapper.FastCrud/issues/3)

**v1.0.0 - 2020/03**

* Provided initial core base

## Providers

* [DataQI.Dapper.FastCrud](https://github.com/henrique-gouveia/DataQI.Dapper.FastCrud)
* [DataQI.EntityFrameworkCore](https://github.com/henrique-gouveia/DataQI.EntityFrameworkCore)

## License

DataQI Commons is released under the [MIT License](https://opensource.org/licenses/MIT).
