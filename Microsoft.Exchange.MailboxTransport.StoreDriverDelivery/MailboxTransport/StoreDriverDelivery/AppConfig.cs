using System;
using System.Configuration;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal sealed class AppConfig : IAppConfiguration
	{
		internal AppConfig()
		{
		}

		public bool IsFolderPickupEnabled
		{
			get
			{
				return AppConfig.GetConfigBool(AppConfig.folderPickupEnabled, false);
			}
		}

		public int PoisonRegistryEntryMaxCount
		{
			get
			{
				return AppConfig.GetConfigInt(AppConfig.poisonRegistryEntryMaxCount, 1, int.MaxValue, 100);
			}
		}

		public void Load()
		{
		}

		private static bool GetConfigBool(string label, bool defaultValue)
		{
			bool result;
			try
			{
				result = TransportAppConfig.GetConfigBool(label, defaultValue);
			}
			catch (ConfigurationErrorsException)
			{
				result = defaultValue;
			}
			return result;
		}

		private static int GetConfigInt(string label, int minimumValue, int maximumValue, int defaultValue)
		{
			int result;
			try
			{
				result = TransportAppConfig.GetConfigInt(label, minimumValue, maximumValue, defaultValue);
			}
			catch (ConfigurationErrorsException)
			{
				result = defaultValue;
			}
			return result;
		}

		private static string folderPickupEnabled = "FolderPickupEnabled";

		private static string poisonRegistryEntryMaxCount = "PoisonRegistryEntryMaxCount";
	}
}
