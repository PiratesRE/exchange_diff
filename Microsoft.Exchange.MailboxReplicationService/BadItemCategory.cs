using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class BadItemCategory
	{
		public BadItemCategory(string name, string configSettingName)
		{
			this.Name = name;
			this.ConfigSettingName = configSettingName;
		}

		public string Name { get; private set; }

		private protected string ConfigSettingName { protected get; private set; }

		public virtual int GetLimit()
		{
			return ConfigBase<MRSConfigSchema>.GetConfig<int>(this.ConfigSettingName);
		}

		public abstract bool IsMatch(BadMessageRec message, TestIntegration testIntegration);
	}
}
