using System;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public enum JobPriority : short
	{
		[MapToManagement(null, false)]
		High,
		[MapToManagement(null, false)]
		Normal,
		[MapToManagement(null, false)]
		Low
	}
}
