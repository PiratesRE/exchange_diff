using System;

namespace System.Threading.Tasks
{
	[Flags]
	[Serializable]
	internal enum InternalTaskOptions
	{
		None = 0,
		InternalOptionsMask = 65280,
		ChildReplica = 256,
		ContinuationTask = 512,
		PromiseTask = 1024,
		SelfReplicating = 2048,
		LazyCancellation = 4096,
		QueuedByRuntime = 8192,
		DoNotDispose = 16384
	}
}
