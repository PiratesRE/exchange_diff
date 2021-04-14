using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public sealed class SmtpVirtualServerConfiguration : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return SmtpVirtualServerConfiguration.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return SmtpVirtualServerConfiguration.mostDerivedClass;
			}
		}

		public string SmtpFqdn
		{
			get
			{
				return (string)this[SmtpVirtualServerConfigurationSchema.SmtpFqdn];
			}
			internal set
			{
				this[SmtpVirtualServerConfigurationSchema.SmtpFqdn] = value;
			}
		}

		private static SmtpVirtualServerConfigurationSchema schema = ObjectSchema.GetInstance<SmtpVirtualServerConfigurationSchema>();

		private static string mostDerivedClass = "protocolCfgSMTPServer";
	}
}
