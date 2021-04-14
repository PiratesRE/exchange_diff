using System;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal enum AmDbCompletionReason
	{
		None,
		Finished,
		Timedout,
		Cancelled
	}
}
