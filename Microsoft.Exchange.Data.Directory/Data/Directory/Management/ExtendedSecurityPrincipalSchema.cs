using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal sealed class ExtendedSecurityPrincipalSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition SID = IADSecurityPrincipalSchema.Sid;

		public static readonly ADPropertyDefinition SecurityPrincipalTypes = new ADPropertyDefinition("SecurityPrincipalTypes", ExchangeObjectVersion.Exchange2003, typeof(SecurityPrincipalType), null, ADPropertyDefinitionFlags.Calculated, SecurityPrincipalType.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.ObjectClass,
			ADObjectSchema.ObjectCategory
		}, null, new GetterDelegate(ExtendedSecurityPrincipal.SecurityPrincipalTypeDetailsGetter), new SetterDelegate(ExtendedSecurityPrincipal.SecurityPrincipalTypeDetailsSetter), null, null);

		public static readonly ADPropertyDefinition DisplayName = ADRecipientSchema.DisplayName;

		public static readonly ADPropertyDefinition RecipientTypeDetails = ADRecipientSchema.RecipientTypeDetails;
	}
}
