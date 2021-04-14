using System;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public enum JobState : short
	{
		[MapToManagement(null, false)]
		Initializing,
		[MapToManagement("Queued", false)]
		Pending,
		[MapToManagement(null, false)]
		Running,
		[MapToManagement("Succeeded", false)]
		Completed,
		[MapToManagement(null, false)]
		Failed
	}
}
