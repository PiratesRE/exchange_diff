using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class ConfigurationSettingStatusBatchSchema
	{
		public static readonly PropertyDefinition TenantIdProp = ADObjectSchema.OrganizationalUnitRoot;

		public static readonly HygienePropertyDefinition BatchProp = new HygienePropertyDefinition("Batch", typeof(IConfigurable), null, ADPropertyDefinitionFlags.MultiValued);
	}
}
