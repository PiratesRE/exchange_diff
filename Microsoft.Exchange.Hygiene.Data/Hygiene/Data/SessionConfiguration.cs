using System;
using System.Configuration;
using System.Reflection;

namespace Microsoft.Exchange.Hygiene.Data
{
	public class SessionConfiguration : ConfigurationSection
	{
		[ConfigurationProperty("PerMessageRecipientSaveThreshold", IsRequired = false, DefaultValue = "100")]
		public int PerMessageRecipientSaveThreshold
		{
			get
			{
				return (int)base["PerMessageRecipientSaveThreshold"];
			}
			internal set
			{
				base["PerMessageRecipientSaveThreshold"] = value;
			}
		}

		[ConfigurationProperty("PerMessageRecipientSplitSaveThreshold", IsRequired = false, DefaultValue = "1000")]
		public int PerMessageRecipientSplitSaveThreshold
		{
			get
			{
				return (int)base["PerMessageRecipientSplitSaveThreshold"];
			}
			internal set
			{
				base["PerMessageRecipientSplitSaveThreshold"] = value;
			}
		}

		public static SessionConfiguration Instance
		{
			get
			{
				return SessionConfiguration.instance;
			}
		}

		private static SessionConfiguration GetInstance()
		{
			SessionConfiguration sessionConfiguration = (SessionConfiguration)ConfigurationManager.GetSection("DALSessionConfiguration");
			if (sessionConfiguration == null)
			{
				string exeConfigFilename = Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path) + ".config";
				ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
				{
					ExeConfigFilename = exeConfigFilename
				};
				Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
				sessionConfiguration = (SessionConfiguration)configuration.GetSection("DALSessionConfiguration");
			}
			if (sessionConfiguration == null)
			{
				sessionConfiguration = new SessionConfiguration();
			}
			return sessionConfiguration;
		}

		private const string PerMessageRecipientSaveThresholdKey = "PerMessageRecipientSaveThreshold";

		private const string PerMessageRecipientSplitSaveThresholdKey = "PerMessageRecipientSplitSaveThreshold";

		private const string SectionName = "DALSessionConfiguration";

		private static SessionConfiguration instance = SessionConfiguration.GetInstance();
	}
}
