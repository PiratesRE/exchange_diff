using System;

namespace System.Threading.Tasks
{
	[Flags]
	[__DynamicallyInvokable]
	[Serializable]
	public enum TaskContinuationOptions
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
		LazyCancellation = 32,
		[__DynamicallyInvokable]
		RunContinuationsAsynchronously = 64,
		[__DynamicallyInvokable]
		NotOnRanToCompletion = 65536,
		[__DynamicallyInvokable]
		NotOnFaulted = 131072,
		[__DynamicallyInvokable]
		NotOnCanceled = 262144,
		[__DynamicallyInvokable]
		OnlyOnRanToCompletion = 393216,
		[__DynamicallyInvokable]
		OnlyOnFaulted = 327680,
		[__DynamicallyInvokable]
		OnlyOnCanceled = 196608,
		[__DynamicallyInvokable]
		ExecuteSynchronously = 524288
	}
}
