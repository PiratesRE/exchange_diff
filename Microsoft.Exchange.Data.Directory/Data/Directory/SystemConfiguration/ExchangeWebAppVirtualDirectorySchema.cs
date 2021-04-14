using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ExchangeWebAppVirtualDirectorySchema : ExchangeVirtualDirectorySchema
	{
		public static readonly ADPropertyDefinition WindowsAuthentication = new ADPropertyDefinition("WindowsAuthentication", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DigestAuthentication = new ADPropertyDefinition("DigestAuthentication", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FormsAuthentication = new ADPropertyDefinition("FormsAuthentication", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BasicAuthentication = new ADPropertyDefinition("BasicAuthentication", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LiveIdAuthentication = new ADPropertyDefinition("LiveIdAuthentication", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADVirtualDirectorySchema.InternalAuthenticationMethodFlags
		}, null, new GetterDelegate(ExchangeWebAppVirtualDirectory.LiveIdAuthenticationGetter), new SetterDelegate(ExchangeWebAppVirtualDirectory.LiveIdAuthenticationSetter), null, null);

		public static readonly ADPropertyDefinition AdfsAuthentication = new ADPropertyDefinition("AdfsAuthentication", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADVirtualDirectorySchema.InternalAuthenticationMethodFlags
		}, null, new GetterDelegate(ExchangeWebAppVirtualDirectory.AdfsAuthenticationGetter), new SetterDelegate(ExchangeWebAppVirtualDirectory.AdfsAuthenticationSetter), null, null);

		public static readonly ADPropertyDefinition OAuthAuthentication = new ADPropertyDefinition("OAuthAuthentication", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADVirtualDirectorySchema.InternalAuthenticationMethodFlags
		}, null, new GetterDelegate(ExchangeWebAppVirtualDirectory.OAuthAuthenticationGetter), new SetterDelegate(ExchangeWebAppVirtualDirectory.OAuthAuthenticationSetter), null, null);

		public static readonly ADPropertyDefinition DefaultDomain = new ADPropertyDefinition("DefaultDomain", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ADGzipLevel = new ADPropertyDefinition("GzipLevel", ExchangeObjectVersion.Exchange2007, typeof(GzipLevel), null, ADPropertyDefinitionFlags.TaskPopulated, GzipLevel.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition WebSite = new ADPropertyDefinition("WebSite", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DisplayName = new ADPropertyDefinition("DisplayName", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
