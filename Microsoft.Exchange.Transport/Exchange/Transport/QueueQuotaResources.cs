using System;

namespace Microsoft.Exchange.Transport
{
	[Flags]
	internal enum QueueQuotaResources : byte
	{
		SubmissionQueueSize = 1,
		TotalQueueSize = 2,
		All = 255
	}
}
