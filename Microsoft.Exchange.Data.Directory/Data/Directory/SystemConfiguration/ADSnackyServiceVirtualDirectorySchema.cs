using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADSnackyServiceVirtualDirectorySchema : ExchangeVirtualDirectorySchema
	{
		public static readonly ADPropertyDefinition LiveIdAuthentication = new ADPropertyDefinition("LiveIdAuthentication", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADVirtualDirectorySchema.InternalAuthenticationMethodFlags
		}, null, new GetterDelegate(ADSnackyServiceVirtualDirectory.LiveIdAuthenticationGetter), new SetterDelegate(ADSnackyServiceVirtualDirectory.LiveIdAuthenticationSetter), null, null);
	}
}
