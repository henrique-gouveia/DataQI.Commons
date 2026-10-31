# DataQI Commons

DataQI.Commons defines repository contracts, query criteria, and proxy dispatch shared by the DataQI providers. Declare queries as interface methods or build criteria explicitly; the provider translates them to its data access API.

[![Build](https://github.com/henrique-gouveia/DataQI.Commons/actions/workflows/dotnet.yml/badge.svg)](https://github.com/henrique-gouveia/DataQI.Commons/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/henrique-gouveia/DataQI.Commons/branch/main/graph/badge.svg)](https://codecov.io/gh/henrique-gouveia/DataQI.Commons)
[![NuGet](https://img.shields.io/nuget/v/DataQI.Commons.svg)](https://www.nuget.org/packages/DataQI.Commons/)
[![License](https://img.shields.io/github/license/henrique-gouveia/DataQI.Commons.svg)](https://github.com/henrique-gouveia/DataQI.Commons/blob/main/LICENSE)

## Features

- Shared CRUD contracts with synchronous and asynchronous operations.
- Query methods returning collections or a single entity, dispatched by return type.
- Criteria with comparisons, text matches, logical groups, and ordering.
- Proxy factories for provider implementations and custom repository methods.
- Optional trailing `CancellationToken` on asynchronous queries; cancellation support depends on the provider.

## Getting Started

### Installing

Install Commons in the project containing your repository contracts:

```bash
dotnet add package DataQI.Commons
```

Select a provider to create a repository backed by a database. Commons itself supplies no database implementation. The following example uses integer identifiers, as in an EF Core repository. For Dapper, use the provider's `IDapperRepository<Person>` base interface: its identifier type is `Person` itself. See each provider README for factory and dependency injection setup.

```csharp
public interface IPersonRepository : ICrudRepository<Person, int>
{
    IEnumerable<Person> FindByLastName(string lastName);
    Person FindByEmail(string email);
    Task<Person> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Person>> FindByLastNameOrderByFirstNameAscAsync(
        string lastName, CancellationToken cancellationToken = default);
    IEnumerable<Person> FindByIdIn(object[] ids);
}
```

Build criteria directly on an existing repository instance:

```csharp
var people = await repository.FindAsync(criteria => criteria
    .Add(Restrictions.GreaterThanEqual(nameof(Person.Age), 18))
    .AddOrder(Order.Asc(nameof(Person.LastName)))
    .AddOrder(Order.Asc(nameof(Person.FirstName))), cancellationToken);
```

Read the [query-method grammar](#query-methods) for the full keyword and alias table, argument types, precedence, return types, and naming limits. In particular, pass numeric membership values as `new object[] { 1, 2 }`, rather than `int[]`.

### Query-method completion

Commons 4.0 includes [DataQI.Analyzers](https://github.com/henrique-gouveia/DataQI.Analyzers) (0.1 or later) as a transitive dependency. Restoring Commons also brings the analyzer to the consuming project, enabling `FindBy...` completion in a compatible IDE without a separate analyzer package reference.

Completion follows the grammar in [Query Methods](#query-methods) and was tested against Commons 4.0 only; it does not check which Commons version a project references. Suggestions cover a conservative subset of names. See the [DataQI.Analyzers repository](https://github.com/henrique-gouveia/DataQI.Analyzers) for IDE configuration and the completion flow.

## Repository Contract

`ICrudRepository<TEntity, TId>` extends `IDisposable`. The provider determines what `TId` represents, when changes are persisted, and whether cancellation is observed. Disposing a repository disposes its underlying context or connection; follow the provider's lifetime guidance.

| Operation | Synchronous | Asynchronous | Result |
|---|---|---|---|
| Delete by identifier | `Delete(TId)` | `DeleteAsync(TId, CancellationToken)` | No result |
| Check identifier | `Exists(TId)` | `ExistsAsync(TId, CancellationToken)` | `bool` |
| Find all | `FindAll()` | `FindAllAsync(CancellationToken)` | `IEnumerable<TEntity>` |
| Find by criteria | `Find(Func<ICriteria, ICriteria>)` | `FindAsync(Func<ICriteria, ICriteria>, CancellationToken)` | `IEnumerable<TEntity>` |
| Find by identifier | `FindOne(TId)` | `FindOneAsync(TId, CancellationToken)` | Entity or `null` |
| Find one by criteria | `FindOne(Func<ICriteria, ICriteria>)` | `FindOneAsync(Func<ICriteria, ICriteria>, CancellationToken)` | Entity or `null`; multiple matches throw |
| Find a slice (`IPagedRepository`) | `FindSlice(criteriaBuilder, page)` | `FindSliceAsync(criteriaBuilder, page, token)` | `ISlice<TEntity>` |
| Find a page (`IPagedRepository`) | `FindPage(criteriaBuilder, page)` | `FindPageAsync(criteriaBuilder, page, token)` | `IPage<TEntity>` |
| Insert | `Insert(TEntity)` | `InsertAsync(TEntity, CancellationToken)` | No result |
| Insert or update | `Save(TEntity)` | `SaveAsync(TEntity, CancellationToken)` | No result |

`ICrudRepository<TEntity, TId>` and `IPagedRepository<TEntity, TId>` are independent contracts extending the empty `IRepository<TEntity, TId>` marker. CRUD retains `IDisposable`; paging declares only its four query methods. Provider interfaces combine both contracts. A non-generic repository must identify exactly one entity/id pair through this marker. Paging interfaces live in `DataQI.Commons.Repository.Paging`; the default implementations (`PageRequest`, `Slice<T>` and `Page<T>`) live in `DataQI.Commons.Repository.Paging.Support`.

Asynchronous operations return `Task` or `Task<TResult>` around the result shown. Collection queries return an empty sequence when nothing matches. Criteria-based single-entity queries throw `InvalidOperationException` when more than one row matches; exceptions from synchronous proxy calls can be wrapped in `TargetInvocationException`.

## Query Methods

This reference describes the name parser and proxy contracts used by the Commons 4.0 release line. [EF Core](https://github.com/henrique-gouveia/DataQI.EntityFrameworkCore/blob/main/README.md) and [Dapper.FastCrud](https://github.com/henrique-gouveia/DataQI.Dapper.FastCrud/blob/main/README.md) translate the resulting criteria differently; their READMEs describe text patterns, null handling, conversion, and persistence.

[DataQI.Analyzers](https://github.com/henrique-gouveia/DataQI.Analyzers) completes the conventional `FindBy` form. Completion is a convenience, not validation of every name, argument, or provider operation.

### Declaration and Return Types

Use this conventional shape:

```text
FindBy<Property>[<Operator>][And|Or<Property>[<Operator>]]...
    [OrderBy<Property>[Asc|Desc][<Property>[Asc|Desc]]...][Async]
```

The predicate must precede `OrderBy`. For an ordered query without conditions, use the criteria API with `Find`/`FindAsync` and `AddOrder`, rather than inventing an empty `FindBy` predicate.

The return type decides which provider method receives the parsed criteria:

| Return type | Provider dispatch | No matches | Multiple matches |
|---|---|---|---|
| `IEnumerable<TEntity>` | `Find(criteriaBuilder)` | Empty sequence | All matches |
| `Task<IEnumerable<TEntity>>` | `FindAsync(criteriaBuilder, token)` | Empty sequence | All matches |
| `ISlice<TEntity>` | `FindSlice(criteriaBuilder, page)` | Empty items | One window of matches |
| `Task<ISlice<TEntity>>` | `FindSliceAsync(criteriaBuilder, page, token)` | Empty items | One window of matches |
| `IPage<TEntity>` | `FindPage(criteriaBuilder, page)` | Empty items with real total | One window with total |
| `Task<IPage<TEntity>>` | `FindPageAsync(criteriaBuilder, page, token)` | Empty items with real total | One window with total |
| `TEntity` | `FindOne(criteriaBuilder)` | `null` | `InvalidOperationException` |
| `Task<TEntity>` | `FindOneAsync(criteriaBuilder, token)` | `null` | `InvalidOperationException` |

Use those exact shapes for parsed queries. `List<TEntity>`, arrays, `IQueryable<TEntity>`, `ValueTask<T>`, counts, and `bool` projections are not additional supported query contracts. A custom implementation method with an exact matching public signature can support other return types because it bypasses parsing.

Synchronous calls pass through reflection; provider exceptions can be wrapped in `TargetInvocationException`. Asynchronous operation failures are normally observed when awaiting the returned task.

### Paging

Import `DataQI.Commons.Repository.Paging.Support` to use `PageRequest.Of(pageIndex, pageSize, orders)` with a zero-based `PageIndex`, a positive `PageSize` less than `int.MaxValue`, and optional `Order.Asc`/`Order.Desc` entries. `Offset` uses `long`; providers reject offsets above `int.MaxValue`. `PageRequest`, `Slice<T>` and `Page<T>` copy their input collections into read-only snapshots, including ordering values.

```csharp
public interface IPersonRepository : ICrudRepository<Person, int>, IPagedRepository<Person, int>
{
    ISlice<Person> FindByLastName(string lastName, IPageRequest page);
    Task<IPage<Person>> FindByLastNameAsync(
        string lastName, IPageRequest page, CancellationToken cancellationToken = default);
}

var slice = repository.FindSlice(c => c, PageRequest.Of(0, 20));
```

`ISlice<T>` exposes `Items`, `PageIndex`, `PageSize`, `HasNext` and `HasPrevious`. `IPage<T>` also exposes `long TotalCount` and `long TotalPages`. Prefer a slice when a total is unnecessary: it reads one extra row in one query. A page runs a separate count only when the items cannot reveal the total; an empty page beyond the end retains the real total.

Parsed paging query signatures are validated when the proxy is created:

- Return exactly `ISlice<TEntity>`, `IPage<TEntity>` or their `Task` forms, with the repository's entity type.
- Include exactly one parameter declared as `IPageRequest`, last before any cancellation token. Passing `null` fails with `ArgumentException`.
- An optional `CancellationToken` must occur only once and last; asynchronous queries forward it to the provider.
- Do not use `IPageRequest` with a non-paged query return type, or a paged return without it. These invalid declarations throw `InvalidOperationException` at proxy creation.
- Exact matching implementation methods retain precedence and bypass parsed-query validation.

Final ordering follows method-name `OrderBy`, criteria `AddOrder`, then `page.Orders`. The first occurrence of each property wins; missing primary-key properties are appended ascending to break ties. An entity with neither a primary key nor an explicit ordering cannot be paged.

### Keywords, Aliases, and Arguments

Matching is case-sensitive. `Property` means the public entity property name, including its original casing. Values are consumed from left to right, following the clauses in the name; parameter names do not determine binding.

| Canonical keyword | Other accepted forms | Arguments | Value type | Example |
|---|---|---:|---|---|
| No operator / `Equal` | `Equals`, `IsEqual`, `IsEquals` | 1 | Value compatible with the property | `FindByName`, `FindByNameEqual` |
| `Between` | `IsBetween` | 2 | Inclusive lower and upper bounds | `FindByAgeBetween` |
| `GreaterThan` | `IsGreaterThan` | 1 | Comparable value supported by the provider | `FindByAgeGreaterThan` |
| `GreaterThanEqual` | `IsGreaterThanEqual` | 1 | Comparable value supported by the provider | `FindByAgeGreaterThanEqual` |
| `LessThan` | `IsLessThan` | 1 | Comparable value supported by the provider | `FindByAgeLessThan` |
| `LessThanEqual` | `IsLessThanEqual` | 1 | Comparable value supported by the provider | `FindByAgeLessThanEqual` |
| `In` | `IsIn` | 1 | `object[]`, including covariant reference arrays such as `string[]` | `FindByIdIn` |
| `Null` | `IsNull` | 0 | No value parameter | `FindByEmailNull` |
| `StartingWith` | `StartsWith`, `IsStartingWith` | 1 | `string` | `FindByNameStartingWith` |
| `EndingWith` | `EndsWith`, `IsEndingWith` | 1 | `string` | `FindByNameEndingWith` |
| `Containing` | `Contains`, `IsContaining` | 1 | `string` | `FindByNameContaining` |
| `Like` | `IsLike` | 1 | `string`; matching semantics depend on the provider | `FindByNameLike` |
| `Not` with a property/operator | Negation marker, not a separate argument | Same as the underlying operator | Same as the underlying operator | `FindByNameNot`, `FindByAgeNotBetween`, `FindByEmailNotNull` |
| `And`, `Or` | Logical separators | 0 | No additional parameter | `FindByNameAndAgeOrEmailNull` |
| `OrderBy`, `Asc`, `Desc` | Ordering suffix | 0 | No value parameter | `FindByNameOrderByAgeDescIdAsc` |
| Trailing `Async` | Optional name suffix | 0 | The return type still decides async dispatch | `FindByNameAsync` |

`IsContains`, `IsStartsWith`, and `IsEndsWith` are not aliases. The parser can fall back to treating such an entire segment as a property name, so avoid them rather than expecting a syntax error.

Text keywords reject non-string values with `ArgumentException`. The parser accepts a null text value, but that does not guarantee a meaningful or translatable query in every provider. Use `Null`/`NotNull` for null checks.

#### Membership and Value Arrays

The current parser casts the argument of `In` to `object[]`. A reference array such as `string[]` satisfies that cast. Value-type arrays (`int[]`, enum arrays), `List<T>`, and other `IEnumerable<T>` values do not; passing them throws `InvalidCastException` when the predicate is built.

Declare a membership query with the appropriate array argument:

```csharp
public interface ICityRepository : ICrudRepository<City, int>
{
    IEnumerable<City> FindByIdIn(object[] ids);
    IEnumerable<City> FindByNameIn(string[] names);
}
```

```csharp
var cities = repository.FindByIdIn(new object[] { 1, 2, 3 });
```

This restriction also applies to `Restrictions.In(property, object[] values)`. Numeric values can be boxed in `object[]`; EF coerces those entries to the property type when translating, while Dapper binds them to SQL parameters.

#### Negation

Prefer `PropertyNot`, `PropertyNotEqual`, `PropertyNotBetween`, `PropertyNotIn`, and `PropertyNotNull`. `Not` wraps the criterion and consumes no additional value. For example, `NameNotEqual` still consumes one string, while `EmailNotNull` consumes none.

The implementation recognizes `Not` whenever it is not followed by a lowercase letter and removes its occurrences from the segment before resolving the operator. This is a parser limitation to account for when choosing property names, not a reason to place `Not` arbitrarily or repeat it. Repeated markers do not express a reliable double-negation language.

### Logical Precedence

`Or` splits the predicate into groups; `And` combines the clauses within each group. Therefore:

```text
FindByNameAndAgeOrEmailNull
(Name = value0 AND Age = value1) OR Email IS NULL
```

The method takes only two value parameters. `Null`, the logical separators, and ordering consume none.

Method names have no parentheses syntax. Use `Restrictions.Conjunction()` and `Restrictions.Disjunction()` when you need other grouping, and add at least one member to each junction. Multiple calls to `criteria.Add(...)` are combined with AND.

### Ordering

A direction terminates an ordered property. The final property defaults to ascending when its direction is omitted:

| Name | Ordering |
|---|---|
| `FindByNameOrderByAge` | Age ascending |
| `FindByNameOrderByAgeDesc` | Age descending |
| `FindByNameOrderByAgeDescId` | Age descending, then Id ascending |
| `FindByNameOrderByAgeDescIdAsc` | Age descending, then Id ascending |

Every non-final property needs an explicit `Asc` or `Desc`. `OrderByAgeId` is interpreted as one property named `AgeId`, not two ascending properties.

Ordering does not turn a single-entity query into a first-match operation. It still fails when more than one entity matches. Use a collection query when multiple matches are intended.

### Asynchronous Queries and Cancellation

The `Async` suffix is conventional but optional: the return type determines asynchronous dispatch. A synchronous return type with an `Async` name still dispatches synchronously.

Place an optional `CancellationToken` last on asynchronous query methods. The proxy removes that final token from predicate values and forwards it to the provider's async criteria overload. With no token parameter, it forwards the default token.

```csharp
public interface IProductRepository : ICrudRepository<Product, int>
{
    Task<IEnumerable<Product>> FindByPriceGreaterThanOrderByPriceDescAsync(
        decimal price, CancellationToken cancellationToken = default);
}
```

```csharp
var products = await repository.FindByPriceGreaterThanOrderByPriceDescAsync(100m, cancellationToken);
```

EF forwards the token to its asynchronous database operations. The current Dapper.FastCrud provider accepts the token for contract compatibility but does not observe it. Do not add a token in the middle of predicate values or rely on cancellation for synchronous parsed queries.

### Naming Limits and Permissive Parsing

Use public, mapped entity properties with PascalCase names. The parser works on name patterns without validating the model first; a name that looks like a keyword can be split or changed before the provider resolves the property.

Examples of problematic names include `InStock`, `LinkedIn`, `LikeCount`, `NotApplicable`, and `TermsAndConditions`, as well as names ending in `Is`, `Not`, `And`, `Or`, or `Async`. A boundary between a property and an operator can also change parsing: `PeriodEnd` + `EndingWith` and `LineStart` + `StartingWith` are current examples. Use explicit criteria or a custom implementation for these properties.

Ordinary substrings are not always separators: `OrderDate` and `AndroidVersion` work because `Or`/`And` are followed by lowercase letters. The list above is not an exhaustive safe-name validator. Completion filters some unsafe names, but currently does not catch every property/operator boundary.

For compatibility, the runtime prefix pattern is `^\w+?By(?=[A-Z])`, so other prefixes ending in `By` before an ASCII uppercase property can be parsed. Completion deliberately offers only `FindBy`.

The parser does not reject every missing prefix: an unrecognized name such as `Foo` can become an equality comparison on a property named `Foo`. Use the conventional shape instead of relying on this fallback. Missing values cause `ArgumentException`; extra values are not a way to add clauses.

### Exact Implementation Methods

Inherited CRUD methods and public custom implementation methods with an exact matching name, return type, and parameter types are forwarded unchanged before any query parsing. Such methods can implement operations outside this grammar, including counts or application-specific checks. Refer to each provider's custom repository example for factory setup.

## Criteria AST (4.0 and later)

`ICriteria.Add` and `AddOrder` return the same criteria instance for chaining. `Criterions` and `Orders` expose the entries in insertion order. Providers combine top-level criteria with AND; use junctions for explicit groups.

All methods below belong to `DataQI.Commons.Query.Support.Restrictions`:

| Method | Criterion |
|---|---|
| `Equal(property, value)` | Equality |
| `GreaterThan(property, value)`, `GreaterThanEqual(property, value)` | Lower bound |
| `LessThan(property, value)`, `LessThanEqual(property, value)` | Upper bound |
| `Between(property, starts, ends)` | Inclusive range |
| `In(property, object[] values)` | Membership |
| `Null(property)` | Null check |
| `StartingWith(property, string value)` | Text match at the start |
| `EndingWith(property, string value)` | Text match at the end |
| `Containing(property, string value)` | Text match within the value |
| `Like(property, string value)` | Provider-specific text match |
| `Not(criterion)` | Negation |
| `Conjunction()`, `Disjunction()` | Junction of kind AND or OR; add members with `.Add(criterion)` |

Property names are strings; `nameof(Person.LastName)` avoids spelling mistakes. Add at least one member to a junction before using it. Add sorting with `Order.Asc(property)` or `Order.Desc(property)`.

Text matching, null semantics, value conversion, supported property types, and persistence differ between providers. Consult the provider README before treating the same criterion as identical SQL or LINQ behavior.

## Release Notes

**v4.0.0 - upcoming release**

- Add paging through `IPagedRepository<TEntity, TId>` with immutable requests and slice/page results. Query methods support synchronous and asynchronous paging, validate their signatures, use stable ordering, and count matching rows only when the total cannot be deduced.
- **Breaking Change!** `ICrudRepository<TEntity, TId>` and `IPagedRepository<TEntity, TId>` independently extend the new `IRepository<TEntity, TId>` marker. Provider interfaces combine both contracts; custom provider implementations must add `FindSlice`, `FindSliceAsync`, `FindPage` and `FindPageAsync`.
- **Breaking Change!** `MethodDescriptor.Create` now accepts a `Func<string, Type[], MethodInfo>` to find implementation methods instead of four separate `MethodInfo` arguments. Update custom dispatch integrations that call it directly.
- **Breaking Change!** Replace the criteria expression hierarchy with an AST and visitor contract, expose read-only criteria and order collections and properties, and require `string` values for `Containing` and `StartingWith`. See the migration table below for the affected APIs.
- Fix parsing of property names containing ordinary `Or`/`And` substrings, such as `OrderDate` and `AndroidVersion`, without treating every substring as a junction.
- Accept `Equals` as an equality alias alongside `Equal`.
- Document public APIs with XML comments for IDE help.
- Include query-method completion through a transitive DataQI.Analyzers dependency.

**Migrating from 3.x**

Criteria are an AST visited through `ICriterionVisitor<T>`. The `Restrictions` factory and `Func<ICriteria, ICriteria>` builder style remain available.

| Previous API | Commons 4.0 |
|---|---|
| `ICriterion.GetPropertyName()` / `GetWhereOperator()` | `ICriterion.Accept<T>(ICriterionVisitor<T>)` and node properties |
| `WhereOperator`, `SimpleExpression`, `BetweenExpression`, `InExpression`, `NullExpression`, `NotExpression` | `Comparison`, `Between`, `In`, `IsNull`, `TextMatch`, `Not` in `DataQI.Commons.Query.Ast` |
| `Conjunction`, `Disjunction`, `IJunction` types | `Junction` with `LogicalKind.And` or `LogicalKind.Or` |
| `IOrderCriterion.GetPropertyName()` / `GetDirection()` | Read-only `PropertyName` / `Direction` properties |
| `Restrictions.Containing` / `StartingWith` with `object` values | Pass `string` values |
| Custom `ICriteria` implementations | Expose read-only `Criterions` and `Orders` collections |

The query interfaces (`ICriteria`, `ICriterion`, `ICriterionVisitor<T>`, `IOrderCriterion`) remain in `DataQI.Commons.Query`. Builders and ordering helpers remain in `DataQI.Commons.Query.Support`. Update custom providers to visit the new AST instead of inspecting `WhereOperator` or the removed expression classes.

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

### Writing a Provider

Implement `ICrudRepository<TEntity, TId>` and, when paging is supported, `IPagedRepository<TEntity, TId>` with the database behavior you need, then derive a factory from `RepositoryFactory`. Its `GetRepositoryInstance(Type, params object[])` creates that implementation; consumers call `GetRepository<TRepository>(...)` or pass a custom implementation factory.

The dispatch path is:

1. `RepositoryFactory` creates a `RepositoryProxy<TRepository>` around the implementation.
2. `RepositoryMetadata` identifies the entity and identifier types. A `MethodDescriptor` caches the dispatch decision; query plans are parsed lazily.
3. An exact public implementation signature takes precedence over name parsing, including custom methods.
4. Otherwise, the return type selects criteria-based `Find`, `FindAsync`, `FindOne`, or `FindOneAsync`. `QueryPlan.ApplyTo` adds the parsed predicate and ordering to `ICriteria`.
5. Paging dispatch selects `FindSlice`, `FindPage` or their async counterparts and removes the page request and token from predicate arguments. Implement `IPagedRepository<TEntity, TId>` to expose these targets.
6. The provider translates `ICriterion.Accept(visitor)` nodes and applies the orders. A method with no available dispatch target is `Unresolvable` and invocation fails.

The dispatch kinds are `ExactMatch`, `SyncCollection`, `AsyncCollection`, `SyncSingle`, `AsyncSingle`, `SyncSlice`, `AsyncSlice`, `SyncPage`, `AsyncPage`, and `Unresolvable`. Expose the criteria overloads with the contract signatures, including a trailing `CancellationToken` on asynchronous overloads. Keep the supported query return shapes listed in the grammar; the dispatcher does not validate arbitrary return types for you.

## License

DataQI Commons is released under the [MIT License](https://github.com/henrique-gouveia/DataQI.Commons/blob/main/LICENSE).
