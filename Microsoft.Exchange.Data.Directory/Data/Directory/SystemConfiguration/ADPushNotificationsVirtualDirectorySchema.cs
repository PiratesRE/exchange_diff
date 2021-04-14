using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADPushNotificationsVirtualDirectorySchema : ExchangeVirtualDirectorySchema
	{
		public static readonly ADPropertyDefinition LiveIdAuthentication = new ADPropertyDefinition("LiveIdAuthentication", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADVirtualDirectorySchema.InternalAuthenticationMethodFlags
		}, null, new GetterDelegate(ADPushNotificationsVirtualDirectory.LiveIdAuthenticationGetter), new SetterDelegate(ADPushNotificationsVirtualDirectory.LiveIdAuthenticationSetter), null, null);
	}
}
