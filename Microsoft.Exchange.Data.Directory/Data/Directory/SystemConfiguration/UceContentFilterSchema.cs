using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class UceContentFilterSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition SCLJunkThreshold = new ADPropertyDefinition("SCLJunkThreshold", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchUceStoreActionThreshold", ADPropertyDefinitionFlags.PersistDefaultValue, 4, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 9)
		}, PropertyDefinitionConstraint.None, null, null);
	}
}
