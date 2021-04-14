using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ADMapiVirtualDirectorySchema : ExchangeVirtualDirectorySchema
	{
		public static readonly ADPropertyDefinition IISAuthenticationMethods = new ADPropertyDefinition("IISAuthenticationMethods", ExchangeObjectVersion.Exchange2007, typeof(AuthenticationMethod), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ValueDefinedConstraint<AuthenticationMethod>(new AuthenticationMethod[]
			{
				AuthenticationMethod.Basic,
				AuthenticationMethod.Ntlm,
				AuthenticationMethod.Kerberos,
				AuthenticationMethod.Negotiate,
				AuthenticationMethod.LiveIdBasic,
				AuthenticationMethod.OAuth
			})
		}, new ProviderPropertyDefinition[]
		{
			ADVirtualDirectorySchema.InternalAuthenticationMethodFlags
		}, null, new GetterDelegate(ADMapiVirtualDirectory.GetIISAuthenticationMethods), new SetterDelegate(ADMapiVirtualDirectory.SetIISAuthenticationMethods), null, null);
	}
}
