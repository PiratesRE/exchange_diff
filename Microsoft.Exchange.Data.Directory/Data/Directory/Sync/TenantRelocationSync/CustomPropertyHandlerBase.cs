using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal abstract class CustomPropertyHandlerBase
	{
		public abstract List<ADObjectId> EnumerateObjectDependenciesInSource(TenantRelocationSyncTranslator translator, DirectoryAttribute DirectoryAttributeModification);

		public abstract void UpdateModifyRequestForTarget(TenantRelocationSyncTranslator translator, DirectoryAttribute sourceValue, ref DirectoryAttributeModification mod);
	}
}
