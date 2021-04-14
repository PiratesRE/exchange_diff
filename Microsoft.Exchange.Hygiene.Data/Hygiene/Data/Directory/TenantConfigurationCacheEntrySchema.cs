using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class TenantConfigurationCacheEntrySchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition Reason = new HygienePropertyDefinition("Reason", typeof(TenantConfigurationCacheEntryReason), TenantConfigurationCacheEntryReason.Pinned, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
