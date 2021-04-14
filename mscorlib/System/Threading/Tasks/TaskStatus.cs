using System;

namespace System.Threading.Tasks
{
	[__DynamicallyInvokable]
	public enum TaskStatus
	{
		[__DynamicallyInvokable]
		Created,
		[__DynamicallyInvokable]
		WaitingForActivation,
		[__DynamicallyInvokable]
		WaitingToRun,
		[__DynamicallyInvokable]
		Running,
		[__DynamicallyInvokable]
		WaitingForChildrenToComplete,
		[__DynamicallyInvokable]
		RanToCompletion,
		[__DynamicallyInvokable]
		Canceled,
		[__DynamicallyInvokable]
		Faulted
	}
}
