using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal delegate TenantRelocationState GetTenantRelocationStateDelegate(ADObjectId tenantOUId, out bool isSourceTenant, bool readThrough = false);
}
