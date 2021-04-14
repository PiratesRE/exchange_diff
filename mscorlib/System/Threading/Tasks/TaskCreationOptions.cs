using System;

namespace System.Threading.Tasks
{
	[Flags]
	[__DynamicallyInvokable]
	[Serializable]
	public enum TaskCreationOptions
	{
		[__DynamicallyInvokable]
		None = 0,
		[__DynamicallyInvokable]
		PreferFairness = 1,
		[__DynamicallyInvokable]
		LongRunning = 2,
		[__DynamicallyInvokable]
		AttachedToParent = 4,
		[__DynamicallyInvokable]
		DenyChildAttach = 8,
		[__DynamicallyInvokable]
		HideScheduler = 16,
		[__DynamicallyInvokable]
		RunContinuationsAsynchronously = 64
	}
}
