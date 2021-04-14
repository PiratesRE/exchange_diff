using System;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class DefaultConnectionFilterGlobalSettingsSchema : ADConfigurationObjectSchema
	{
		public DefaultConnectionFilterGlobalSettingsSchema()
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>(base.AllProperties);
			hashSet.Remove(ADObjectSchema.ExchangeVersion);
			base.AllProperties = new ReadOnlyCollection<PropertyDefinition>(hashSet.ToArray());
			base.InitializePropertyCollections();
			base.InitializeADObjectSchemaProperties();
		}
	}
}
