using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class StampGroupSchema : ADConfigurationObjectSchema
	{
		public new static readonly ADPropertyDefinition Name = new ADPropertyDefinition("Name", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchMDBAvailabilityGroupName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ADObjectNameStringLengthConstraint(1, 15),
			ComputerNameCharacterConstraint.DefaultConstraint
		}, null, null);

		public static readonly ADPropertyDefinition Servers = new ADPropertyDefinition("Servers", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchMDBAvailabilityGroupBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
