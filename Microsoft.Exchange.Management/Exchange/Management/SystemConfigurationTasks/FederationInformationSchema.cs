using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FederationInformationSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition TargetApplicationUri = new SimpleProviderPropertyDefinition("TargetApplicationUri", ExchangeObjectVersion.Exchange2010, typeof(Uri), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TokenIssuerUris = new SimpleProviderPropertyDefinition("TokenIssuerUris", ExchangeObjectVersion.Exchange2010, typeof(Uri), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DomainNames = new SimpleProviderPropertyDefinition("DomainNames", ExchangeObjectVersion.Exchange2010, typeof(SmtpDomain), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TargetAutodiscoverEpr = new SimpleProviderPropertyDefinition("TargetAutodiscoverEpr", ExchangeObjectVersion.Exchange2010, typeof(Uri), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public new static readonly SimpleProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2010, typeof(SmtpDomain), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
