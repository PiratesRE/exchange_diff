using System;
using System.Configuration;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal sealed class AppConfigAdapter : IConfigAdapter
	{
		internal static AppConfigAdapter Instance
		{
			get
			{
				return AppConfigAdapter.instance;
			}
		}

		public string GetSetting(string key)
		{
			return ConfigurationManager.AppSettings[key];
		}

		private static AppConfigAdapter instance = new AppConfigAdapter();
	}
}
