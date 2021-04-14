using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class AlternateServiceAccountCredentialSchema : SimpleProviderObjectSchema
	{
		public static SimpleProviderPropertyDefinition Domain = new SimpleProviderPropertyDefinition("Domain", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.Mandatory, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition UserName = new SimpleProviderPropertyDefinition("UserName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.Mandatory, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition WhenAddedUTC = new SimpleProviderPropertyDefinition("WhenAddedUTC", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition WhenAdded = new SimpleProviderPropertyDefinition("WhenAdded", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated | PropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new SimpleProviderPropertyDefinition[]
		{
			AlternateServiceAccountCredentialSchema.WhenAddedUTC
		}, null, new GetterDelegate(AlternateServiceAccountCredential.WhenAddedGetter), null);
	}
}
