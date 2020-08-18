MonoDevelop 8.1 introduces a new model for implementing and consuming services. In previous versions MonoDevelop had a mix of approaches for implementing services. Some were implemented as instance classes, some as static classes, and there was no standard way of initializing them.

## The New Service Model

The new service model has three main goals:

* Homogenize how services are implemented, instantiated, initialized and accessed.
* Support asynchronous and on-demand initialization of services, to reduce and parallelize the of work that needs to be done at IDE startup.
* Improve support for unit testing by allowing discrete service initialization and using mock test services.

The model is very simple:

* The new ServiceProvider class provides methods for registering and getting references to services. 
* Services implement the interface `IService`, either directly or through the abstract class `Service`. The interface defines the async method `Task Initialize(ServiceProvider)` which is invoked the first time a service is requested.

In this new model all services are instances (no more static classes, with the exception of some special cases), and it is possible to get any service using the main service provider available in `Runtime.ServiceProvider`. The `Runtime` class also provides some shortcut methods for getting services. For example:

``` csharp
var fontService = await Runtime.GetService<FontService> ();
...
```

The `GetService()` method can be used to get a service. Services are created on demand, so if the service has not been requested before, it will be created and initialized at this time.

## Getting Services

There are several ways of getting a reference to a service:

### The ServiceProvider class

The `ServiceProvider` class provides several methods for getting a service:

* `Task<T> GetService<T> ()`: this async method can be used to get a service. Services are created on demand, so if the service has not been requested before, it will be created and initialized at this time.

* `T PeekService<T> ()`: it returns the service of the specified type, but only if the service has been initialized. It returns `null` otherwise.

* `IDisposable WhenServiceInitialized<T> (Action<T> action)`: Executes the given action when (and if) the service is initialized. This method is useful when the dependency on a service is optional, and no action has to be taken until that service is initialized.

### The IdeServices class

Getting a service is an asynchronous operation (since it may involve initialization). This may not always be convenient, since services also need to be used from methods that are not asynchronous. To make it easier to use services, MonoDevelop.Ide defines the class `IdeServices`, which has references to all services that are initialized at startup. Referencing services from `IdeServices` is safe once the Ide has completed the initialization (that is when the IdeApp.Initialized event is raised and IdeApp.IsInitialized is set to true).

In some scenarios (such as in unit tests or when implementing a command line command), IdeServices still can be used, but will only have references to those services that have been initialized.

## Service Initialization in Unit Tests

A benefit of the new service model is that unit tests don't need to initialize the whole IDE to test some functionality. Services can be initialized and obtained independently, and in general they will take care of ensuring that the services they depend on are also initialized.

However, even though a service knows which other services it needs to do its work and can initialize those when needed, it doesn't know what extensions may need. For example, the DocumentManager service doesn't need the Type System service to work, but some document controller implementations may depend on it, and assume that it will be available in IdeServices.

When writing unit tests those cases are easy to detect, because the test will fail with a "Service not initialized" exception, so the solution is to explicitly initialize the service at startup. If the unit test is subclassing the `TestBase` class defined by the `UnitTests` assembly, service initialization can be easily requested using the `[RequireService]` attribute, for example:

```csharp
[RequireService (typeof(TypeSystemService))]
public class SomeTests : TestBase
{
	...
}
```

