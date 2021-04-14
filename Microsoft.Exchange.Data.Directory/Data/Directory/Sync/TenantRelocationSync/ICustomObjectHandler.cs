using System;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal interface ICustomObjectHandler
	{
		bool HandleObject(TenantRelocationSyncObject obj, ModifyRequest modRequest, UpdateData mData, TenantRelocationSyncData syncData, ITopologyConfigurationSession targetPartitionSession);
	}
}
