# <img align="center" src="https://raw.githubusercontent.com/dalrankov/TransactHangfire/master/icon.png"/> TransactHangfire

TransactionScope filter wrapper for Hangfire jobs

<a href="https://www.nuget.org/packages/TransactHangfire"><img alt="NuGet Version" src="https://img.shields.io/nuget/v/TransactHangfire"></a>
<a href="https://www.nuget.org/packages/TransactHangfire"><img alt="NuGet Downloads" src="https://img.shields.io/nuget/dt/TransactHangfire"></a>

## How it works?

Registered filter will wrap jobs executed by Hangfire inside TransactionScope. Operations within a job are persisted only if there are no exceptions being thrown.

## Usage

Global level:

All jobs will be wrapped inside individual transaction scopes.
````csharp
services.AddHangfire((provider, config) =>
{
    config
        /* Your Hangfire configuration */
        .UseTransactionScope();
});

//or

GlobalConfiguration.Configuration.UseTransactionScope();
````

Class level:

All methods within class will be wrapped inside individual transaction scopes.
````csharp
[UseTransactionScope]
public class TestService
{
    public void Run1()
    {
        //Database writes, nested transactions etc.
    }

    public void Run2()
    {
        //Database writes, nested transactions etc.
    }
}
````

Method level:

Only method with `[UseTransactionScope]` attribute will be wrapped inside transaction scope.
````csharp
public class TestService
{
    [UseTransactionScope]
    public void Run1()
    {
        //Database writes, nested transactions etc.
    }

    public void Run2()
    {
        //Some operations that maybe do not require transaction...
    }
}
````

Default TransactionScope factory has following specs:
- TransactionScopeOption: Required
- IsolationLevel: ReadCommited
- Timeout: 1 minute
- TransactionScopeAsyncFlowOption: Enabled

## Custom TransactionScope factory

You can implement `ITransactionScopeFactory` to build your own TransactionScope objects.

````csharp
public class CustomTransactionScopeFactory
    : ITransactionScopeFactory
{
    public TransactionScope CreateScope()
    {
        return new TransactionScope();
    }
}
````

Register your factory on global level:

````csharp
services.AddHangfire((provider, config) =>
{
    config
        /* Your Hangfire configuration */
        .UseTransactionScope<CustomTransactionScopeFactory>();
});

//or

GlobalConfiguration.Configuration.UseTransactionScope<CustomTransactionScopeFactory>();
````

Register your factory per attribute:

````csharp
[UseTransactionScope(typeof(CustomTransactionScopeFactory))]
````