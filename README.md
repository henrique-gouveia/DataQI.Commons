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

## Criteria AST (4.0 and later)

Query criteria are now an AST walked through `ICriterionVisitor<T>`, replacing the `WhereOperator`-driven class hierarchy. Where the types live:

| Namespace | Types |
|---|---|
| `DataQI.Commons.Query` | `ICriteria`, `ICriterion`, `ICriterionVisitor<T>`, `IOrderCriterion` |
| `DataQI.Commons.Query.Ast` | `Comparison`, `Between`, `In`, `IsNull`, `TextMatch`, `Not`, `Junction`, `ComparisonKind`, `TextMatchKind`, `LogicalKind` |
| `DataQI.Commons.Query.Support` | `Criteria`, `Restrictions`, `Order`, `OrderCriterion`, `OrderDirection` |

`Restrictions` keeps its method names and `Func<ICriteria, ICriteria>` is still the way to build criteria, so code that only composes criteria with `Restrictions` keeps working.

Breaking changes:

- `ICriterion` no longer has `GetPropertyName()`/`GetWhereOperator()`; it has `T Accept<T>(ICriterionVisitor<T> visitor)`. `WhereOperator`, `SimpleExpression`, `BetweenExpression`, `InExpression`, `NullExpression`, `NotExpression`, `Conjunction`, `Disjunction` and `IJunction` were removed. `Restrictions.Conjunction()`/`Disjunction()` return the single `Junction` node, whose `Kind` is `And` or `Or`.
- `IOrderCriterion.GetPropertyName()` and `GetDirection()` were replaced by the read-only properties `PropertyName` and `Direction`. `Order.Asc`/`Order.Desc` are unchanged.
- `Restrictions.StartingWith` takes `string` instead of `object`, like `EndingWith` and `Like`.
- `ICriteria` now exposes `Criterions` and `Orders`.

Parser fixes: method names whose property contains `Or`/`And` as a substring (`FindByOrderDate`, `FindByAndroidVersion`) no longer throw, and `Equals` is accepted as a synonym for `Equal` (`FindByNameEquals`).

## Getting Started

### Installing

This library can add to the project by the way:

    dotnet add package DataQI.Commons

See [Nuget](https://www.nuget.org/packages/DataQI.Commons) for other options.

## News

**v3.0.0 - 2026/10**

* New! Added support for async query methods
* New! Added single-entity query methods and criteria-based `FindOne`/`FindOneAsync` methods
* New! Added ordering through criteria and the `OrderBy` suffix in query method names
* Change! Added `AddOrder` to `ICriteria` and criteria-based `FindOne`/`FindOneAsync` overloads to `ICrudRepository`; custom implementations must implement the new members
* Change! Cache repository method dispatch metadata
* Fix! Make `RepositoryProxy.Create` thread-safe with locking

**v2.0.0 - 2024/12**

* Change! Add cancellation token parameter for async methods
* Change! Add the use of collection methods instead of enumerator methods to the `QueryTree` extractor.
* New! Add a new way to generate a unique method name for caching

**v1.3.0 - 2022/06**

* Change! Upgraded versions of the package references

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
