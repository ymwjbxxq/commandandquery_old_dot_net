# CommandAndQuery

## Overview

CommandAndQuery applies the CQS principle by using separate Query and Command objects to retrieve and modify data, respectively.

## Command configuration

### Basic Usage

First, install the [NuGet package](https://www.nuget.org/packages/CommandAndQuery/):

```Install-Package CommandAndQuery```

Add the appropriate namespace:

```
using CommandAndQuery;
```

Then create your concreate service locator implementation. For example with Castle.Windsor

```
public class WindsorServiceLocator : IServiceLocator
{
	private readonly IWindsorContainer _container;

	public WindsorServiceLocator(IWindsorContainer container)
	{
		_container = container;
	}

	public T Resolve<T>()
	{
		return _container.Resolve<T>();
	}

	public IEnumerable<T> ResolveAll<T>()
	{
		return _container.ResolveAll<T>();
	}
}
```

Then set up your dependency injection to register the appropriate interfaces. For example with Castle.Windsor:

```
public class TasksInstaller : IWindsorInstaller
{
	public void Install(IWindsorContainer container, IConfigurationStore store)
	{
		var serviceLocator = new WindsorServiceLocator(container);
		container.Register(Component.For<IServiceLocator>().Instance(serviceLocator));
		ServiceLocator.SetServiceLocator(serviceLocator);

		container.Register(
		  Classes.FromThisAssembly()
			   .BasedOn(typeof(ICommandHandler<>))
			   .WithService.FirstInterface());

		container.Register(
			Classes.FromThisAssembly()
				.BasedOn(typeof(ICommandHandler<,>))
				.WithService.FirstInterface());

		container.Register(
			Component.For(typeof(ICommandProcessor))
				.ImplementedBy(typeof(CommandProcessor))
				.Named("CommandProcessor"));
				
		container.Register(
			Classes.FromThisAssembly()
				.BasedOn(typeof(ICommandValidatorFor<>))
				.WithServiceAllInterfaces());
	}
}
```

Then create your command:

```
public class MyCommand : ICommand
{
	public MyCommand(string myProperty)
	{
		MyProperty = myProperty;
	}

	public string MyProperty { get; private set; }
}
```

Then create your command handler:

```
public class MyCommandHandler : ICommandHandler<MyCommand>
{
	public async Task Handle(MyCommand command)
	{
		//DoSomething()
		...	
	}
}
```

Or create your command handler with an output parameter:

```
public class YourDTO : CommandResult
{

}

public class MyCommandHandler : ICommandHandler<MyCommand, YourDTO>
{
	public async Task<YourDTO> Handle(MyCommand command)
	{
		//DoSomething()
		...			

		return new YourDTO();
	}
}
```

You could apply validators to your request to have some kind of business logic


```
public class MyValidator : ICommandValidatorFor<MyCommand>
{
	public ValidationResponse Validate(MyCommand command)
	{
		if (command.MyProperty == "1")
		{
			return new ValidationResponse
			{
				Errors = new[] {"Error"}
			};
		}

		return new ValidationResponse
		{
			Errors = new string[] {}
		};
	}
}
```

Then call your command:

```
public class MyController : ApiController
{
	private readonly ICommandProcessor _commandProcessor;

	public MyController(ICommandProcessor commandProcessor)
	{
		_commandProcessor = commandProcessor;
	}

	[HttpPost]
	public async Task<IHttpActionResult> DoSomething(YourModel model)
	{
		var command = new MyCommand(model.MyProperty);
		//command with an output result
		var result = await _commandProcessor.Process<MyCommand, YourDTO>(command);
		return Ok(result);

		//fire ad forget
		await _commandProcessor.Process<MyCommand>(command);
		return Ok();
	}
}
```

## Query configuration

### Basic Usage

First, install the [NuGet package](https://www.nuget.org/packages/CommandAndQuery/):

```Install-Package CommandAndQuery```

Add the appropriate namespace:

```
using CommandAndQuery;
```

Then set up your dependency injection to register the appropriate interfaces. For example with Castle.Windsor:

```
public class QueryInstaller : IWindsorInstaller
{
	public void Install(IWindsorContainer container, IConfigurationStore store)
	{
		container.Register(
			Classes.FromThisAssembly()
				.BasedOn(typeof(IQueryHandler<>))
				.LifestyleTransient()
				.WithService.AllInterfaces());
	}
}
```

Then create your query:

```
public interface IMyQuery
    {
        IQueryHandler<MyDto> Init();
    }
	
public class MyQuery : IMyQuery, IQueryHandler<MyDto>
{
	private int _id;

	public IQueryHandler<MyDto> Init(int id)
	{
		_id = id;
		return this;
	}

	public async Task<MyDto> Execute()
	{
		//DoSomething(_id)
		return new MyDto();
	}
}
```

Then call your query:

```
public class MyController : ApiController
{
	private readonly IMyQuery _myQuery;

	public MyController(IMyQuery myQuery)
	{
		_myQuery = myQuery;
	}

	[HttpGet]
	public async Task<IHttpActionResult> GetSomething(int id)
	{
		var result = await _myQuery.Init(id).Execute();
		return Ok(result);
	}
}
```
