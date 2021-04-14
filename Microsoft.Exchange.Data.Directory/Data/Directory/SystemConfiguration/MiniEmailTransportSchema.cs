using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class MiniEmailTransportSchema : ADObjectSchema
	{
		public static readonly ADPropertyDefinition Server = ADEmailTransportSchema.Server;

		public static readonly ADPropertyDefinition InternalConnectionSettings = PopImapAdConfigurationSchema.InternalConnectionSettings;

		public static readonly ADPropertyDefinition ExternalConnectionSettings = PopImapAdConfigurationSchema.ExternalConnectionSettings;

		public static readonly ADPropertyDefinition UnencryptedOrTLSBindings = PopImapAdConfigurationSchema.UnencryptedOrTLSBindings;

		public static readonly ADPropertyDefinition SSLBindings = PopImapAdConfigurationSchema.SSLBindings;

		public static readonly ADPropertyDefinition LoginType = PopImapAdConfigurationSchema.LoginType;
	}
}
