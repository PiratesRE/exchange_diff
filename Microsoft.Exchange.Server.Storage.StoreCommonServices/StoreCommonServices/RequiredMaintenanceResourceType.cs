using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public enum RequiredMaintenanceResourceType
	{
		Store,
		DirectoryServiceAndStore,
		StoreUrgent,
		StoreOnlineIntegrityCheck,
		StoreScheduledIntegrityCheck,
		Size
	}
}
