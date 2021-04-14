using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ADVirtualDirectorySchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition InternalUrl = new ADPropertyDefinition("InternalUrl", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchInternalHostName", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition InternalAuthenticationMethodFlags = new ADPropertyDefinition("AuthenticationMethodFlags", ExchangeObjectVersion.Exchange2007, typeof(AuthenticationMethodFlags), "msExchInternalAuthenticationMethods", ADPropertyDefinitionFlags.None, AuthenticationMethodFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalUrl = new ADPropertyDefinition("ExternalUrl", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchExternalHostName", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition ExternalAuthenticationMethodFlags = new ADPropertyDefinition("AuthenticationMethodFlags", ExchangeObjectVersion.Exchange2007, typeof(AuthenticationMethodFlags), "msExchExternalAuthenticationMethods", ADPropertyDefinitionFlags.None, AuthenticationMethodFlags.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Server = new ADPropertyDefinition("Server", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADVirtualDirectory.ServerGetter), null, null, null);

		public static readonly ADPropertyDefinition ServerName = new ADPropertyDefinition("ServerName", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADVirtualDirectory.ServerNameGetter), null, null, null);

		public static readonly ADPropertyDefinition AdminDisplayVersion = new ADPropertyDefinition("AdminDisplayVersion", ExchangeObjectVersion.Exchange2003, typeof(ServerVersion), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition InternalAuthenticationMethods = new ADPropertyDefinition("InternalAuthenticationMethods", ExchangeObjectVersion.Exchange2007, typeof(AuthenticationMethod), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<AuthenticationMethod>(AuthenticationMethod.Basic, AuthenticationMethod.LiveIdNegotiate)
		}, new ProviderPropertyDefinition[]
		{
			ADVirtualDirectorySchema.InternalAuthenticationMethodFlags
		}, null, new GetterDelegate(ADVirtualDirectory.InternalAuthenticationMethodsGetter), new SetterDelegate(ADVirtualDirectory.InternalAuthenticationMethodsSetter), null, null);

		public static readonly ADPropertyDefinition ExternalAuthenticationMethods = new ADPropertyDefinition("ExternalAuthenticationMethods", ExchangeObjectVersion.Exchange2007, typeof(AuthenticationMethod), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<AuthenticationMethod>(AuthenticationMethod.Basic, AuthenticationMethod.LiveIdNegotiate)
		}, new ProviderPropertyDefinition[]
		{
			ADVirtualDirectorySchema.ExternalAuthenticationMethodFlags
		}, null, new GetterDelegate(ADVirtualDirectory.ExternalAuthenticationMethodsGetter), new SetterDelegate(ADVirtualDirectory.ExternalAuthenticationMethodsSetter), null, null);
	}
}
