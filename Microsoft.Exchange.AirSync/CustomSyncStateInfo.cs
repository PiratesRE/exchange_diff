using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal abstract class CustomSyncStateInfo : SyncStateInfo
	{
		internal CustomSyncStateInfo()
		{
			AirSyncSyncStateTypeFactory.EnsureSyncStateTypesRegistered();
		}
	}
}
