using System;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public enum JobSource : short
	{
		[MapToManagement(null, false)]
		OnDemand,
		[MapToManagement(null, false)]
		Maintenance
	}
}
