using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class RegionEmailFilterSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition FilterStatus = new HygienePropertyDefinition("Enabled", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
