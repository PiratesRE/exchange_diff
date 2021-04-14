using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.E4E
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EncryptionConfigurationSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Image = new SimpleProviderPropertyDefinition("Image", ExchangeObjectVersion.Exchange2012, typeof(byte[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition EmailText = new SimpleProviderPropertyDefinition("EmailText", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition PortalText = new SimpleProviderPropertyDefinition("PortalText", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DisclaimerText = new SimpleProviderPropertyDefinition("DisclaimerText", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition OTPEnabled = new SimpleProviderPropertyDefinition("OTPEnabled", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2012, typeof(OMEConfigurationId), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			EncryptionConfigurationSchema.Image,
			EncryptionConfigurationSchema.EmailText,
			EncryptionConfigurationSchema.PortalText
		}, null, new GetterDelegate(EncryptionConfiguration.IdentityGetter), null);
	}
}
