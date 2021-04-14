using System;
using System.Configuration;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class JournalConfigSchema : ConfigSchemaBase
	{
		public override string Name
		{
			get
			{
				return "Journal";
			}
		}

		[ConfigurationProperty("LegalInterceptTenantName", DefaultValue = "")]
		public string LegalInterceptTenantName
		{
			get
			{
				return (string)base["LegalInterceptTenantName"];
			}
			set
			{
				base["LegalInterceptTenantName"] = value;
			}
		}

		internal static IConfigProvider Configuration
		{
			get
			{
				return JournalConfigSchema.configProvider.Value;
			}
		}

		private static Lazy<IConfigProvider> configProvider = new Lazy<IConfigProvider>(delegate()
		{
			IConfigProvider configProvider = ConfigProvider.CreateProvider(new JournalConfigSchema());
			configProvider.Initialize();
			return configProvider;
		}, true);

		public static class Setting
		{
			public const string LegalInterceptTenantName = "LegalInterceptTenantName";
		}
	}
}
