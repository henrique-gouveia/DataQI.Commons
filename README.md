# DataQI Commons [![NuGet](https://img.shields.io/nuget/v/DataQI.Commons.svg)](https://www.nuget.org/packages/DataQI.Commons/)

Data Query Interface Commons is built around essential features of the C# 6 / VB that provides shared infrastructure containing technology neutral repository interfaces underpinning every DataQI module.

## Getting Started

DataQI Commons is a library heavily inspired by Pivotal's Spring Data Commons library, and it turns your Data Repositories a live interface, making possible a custom Query Methods definitions:

```csharp
public interface IPersonRepository : ICrudRepository<Person, int>
{
    IEnumerable<Person> FindByFullName(string fullName);
  
    IEnumerable<Person> FindByEmailLike(string email);
  
    IEnumerable<Person> FindByDateRegisterBetween(DateTime startDate, DateTime endDate);
}
```

The `RepositoryFactory` class can extended to define abstract method `GetCustomImplementation` and then generates an implementation of `IPersonRepository` that uses a Data Base Connection to make its calls:

```csharp
var repositoryFactory = new CustomRepositoryFactory(Connection);
PersonRepository = repositoryFactory.GetRepository<IPersonRepository>();
```

### Using Default Methods:

```csharp
personRepository.Insert(personExpected);
await personRepository.InsertAsync(personExpected);

personRepository.Save(person);
await personRepository.SaveAsync(person);

personRepository.Delete(person);
await personRepository.DeleteAsync(person);

var exists = personRepository.Exists(person);
exists = await personRepository.ExistsAsync(person);

var allPersons = personRepository.FindAll();
allPersons = await personRepository.FindAllAsync();

var onePerson = personRepository.FindOne(new Person{ Id = 1 });
onePerson = await personRepository.FindOneAsync(new Person{ Id = 1 });
```

### Using Criteria Definitions:

```csharp
Func<ICriteria, ICriteria> criteriaBuilder = criteria =>
    criteria
        .Add(Restrictions.Equal("FirstName", "@firstName"))
        .Add(Restrictions
            .Disjuction()
            .Add(Restrictions.Between("DateRegister", "@dateRegisterStart", "@dateRegisterEnd"))
            .Add(Restrictions.IsNotNull("Email")));
        
var personsByCriteria = personRepository.Find(criteriaBuilder);
personsByCriteria = await personRepository.FindAsync(criteriaBuilder);
```

### Using Query Methods:

```csharp
var personsByFullName = personRepository.FindByFullName("My Name");
var personsByEmailLike = personRepository.FindByEmailLike("mail@mail.%");
var personsByDateRegisterBetween = personRepository.FindByDateRegisterBetween(Convert.ToDateTime("2020-01-01"), Convert.ToDateTime("2020-01-01"));
```

**Supported keywords inside method names**

| **Keyword** | **Sample** | **Fragment**
|-------------|------------|-------------
| **Between** | FindByDateOfBirth**Between** | where DateOfBirth **between** @dateOfBirthStart and @dateOfBirthEnd
| **NotBetween** | FindByDateOfBirth**NotBetween** | where DateOfBirth **not between** @dateOfBirthStart **and** @dateOfBirthEnd
| **Equals** | FindByName, FindByName**Equals** | where Name **=** @name
| **NotEquals** | FindByName, FindByName**NotEquals** | where Name **<>** @name
| **GreaterThan** | FindByDateOfBirth**GreaterThan** | where DateOfBirth **>** @dateOfBirth
| **GreaterThanEqual** | FindByDateOfBirth**GreaterThanEqual** | where DateOfBirth **>=** @dateOfBirth
| **In** | FindByAddressType**In** | where AddressType **in** (@addressType)
| **NotIn** | FindByAddressType**NotIn** | where AddressType **not in** (@addressType)
| **IsNull** | FindByEmail**IsNull** | where Email **is null**
| **LessThan** | FindByDateOfBirth**LessThan** | where DateOfBirth **<** @dateOfBirth
| **LessThanEqual** | FindByDateOfBirth**LessThanEqual** | where DateOfBirth **<=** @dateOfBirth
| **Like** | FindByName**Like** | where Name **like** @name
| **NotLike** | FindByName**NotLike** | where Name **not like** @name
| **And** | FindByName**And**Email | where Name = @name **and** Email = @email
| **Or** | FindByName**Or**Email | where Name = @name **or** Email = @email

## Limitations and caveats

DataQI is in the experimental phase. It does attempt to solve some problems, but is expected that some improvements turn possible to arrive a stable version.

## Contributors

* Henrique Gouveia
* Brenno Brandão                                          

A [recent list of contributors](https://github.com/henrique-gouveia/DataQI.Commons/graphs/contributors) can always be obtained on GitHub.

## License

DataQI Commons is released under the [MIT License](https://opensource.org/licenses/MIT).
