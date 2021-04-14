using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public enum OperationStatus : uint
	{
		None,
		Pending,
		Searching,
		RetrySearching,
		SearchCompleted,
		Stopping,
		Processing,
		PartiallyProcessed,
		Processed,
		Rollbacking
	}
}
