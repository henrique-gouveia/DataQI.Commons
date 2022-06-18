# DataQI Commons

Data Query Interface Commons is written in C# and built around essential features of the .NET Standard that provides shared infrastructure containing technology-neutral repository interfaces underpinning every DataQI Providers.

[![Build](https://github.com/henrique-gouveia/DataQI.Commons/actions/workflows/dotnet.yml/badge.svg)](https://github.com/henrique-gouveia/DataQI.Commons/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/henrique-gouveia/DataQI.Commons/branch/main/graph/badge.svg)](https://codecov.io/gh/henrique-gouveia/DataQI.Commons)
[![NuGet](https://img.shields.io/nuget/v/DataQI.Commons.svg)](https://www.nuget.org/packages/DataQI.Commons/) 
<!-- [![License](https://img.shields.io/github/license/henrique-gouveia/DataQI.Commons.svg)](https://github.com/henrique-gouveia/DataQI.Commons/blob/main/LICENSE.txt) -->

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

**v1.3.0 - 2022/06**

* Change! Upgraded package references

**v1.2.0 - 2022/01**

* New! Method _GetRepository_ on Repository Factory can receive a lambda expression to create a repository instance
* Change! Method _GetRepository_ on Repository Factory can receive arguments for the repository constructor
* Change! TEntity requirements on generic interface _ICrudRepository_

**v1.1.0 - 2020/09**

* New! Added a new Criteria Query API Core
* New! Added a new Query Parser
* New! Added support for the keywords **_Containing_**, **_StartingWith_** and **_EndingWith_**
* Change! Keyword **_Equals_** to **_Equal_**
* Change! Keyword **_IsNull_** to **_Null_**
* Fix! [issue3](https://github.com/henrique-gouveia/DataQI.Dapper.FastCrud/issues/3)

**v1.0.0 - 2020/03**

* Provided initial core base

## Providers

* [DataQI.Dapper.FastCrud](https://github.com/henrique-gouveia/DataQI.Dapper.FastCrud)
* [DataQI.EntityFrameworkCore](https://github.com/henrique-gouveia/DataQI.EntityFrameworkCore)

## License

DataQI Commons is released under the [MIT License](https://opensource.org/licenses/MIT).
