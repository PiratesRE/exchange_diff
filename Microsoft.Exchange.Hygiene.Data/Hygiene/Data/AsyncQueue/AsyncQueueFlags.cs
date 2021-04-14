using System;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	[Flags]
	internal enum AsyncQueueFlags
	{
		None = 0,
		HARequest = 1,
		ContinueOnFailure = 2,
		Finalizer = 4,
		SkipIfAssemblyMissing = 8,
		Continuous = 16,
		ContinueOnDependantRequestSuccess = 32,
		MarkSkipOnMaxRetries = 64
	}
}
