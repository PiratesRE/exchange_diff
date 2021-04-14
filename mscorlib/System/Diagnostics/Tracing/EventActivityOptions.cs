using System;

namespace System.Diagnostics.Tracing
{
	[Flags]
	[__DynamicallyInvokable]
	public enum EventActivityOptions
	{
		[__DynamicallyInvokable]
		None = 0,
		[__DynamicallyInvokable]
		Disable = 2,
		[__DynamicallyInvokable]
		Recursive = 4,
		[__DynamicallyInvokable]
		Detachable = 8
	}
}
