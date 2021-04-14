using System;

namespace System
{
	[__DynamicallyInvokable]
	public interface IServiceProvider
	{
		[__DynamicallyInvokable]
		object GetService(Type serviceType);
	}
}
