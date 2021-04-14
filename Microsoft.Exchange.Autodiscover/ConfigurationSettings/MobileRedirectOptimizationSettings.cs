using System;
using System.Collections.Generic;
using System.Web.Configuration;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	internal class MobileRedirectOptimizationSettings
	{
		public MobileRedirectOptimizationSettings()
		{
			string text = WebConfigurationManager.AppSettings["MobileSyncRedirectBypassEnabled"];
			this.Enabled = (text != null && text.Equals("true"));
			this.enabledUserAgentPrefixes = new List<string>();
			string text2 = WebConfigurationManager.AppSettings["MobileSyncRedirectBypassClientPrefixes"];
			if (text2 != null)
			{
				string[] array = text2.Split(new char[]
				{
					MobileRedirectOptimizationSettings.clientPrefixListDelimeter
				});
				foreach (string item in array)
				{
					this.enabledUserAgentPrefixes.Add(item);
				}
			}
		}

		public bool UserAgentEnabled(string userAgent)
		{
			if (this.enabledUserAgentPrefixes != null && userAgent != null)
			{
				foreach (string value in this.enabledUserAgentPrefixes)
				{
					if (userAgent.StartsWith(value, StringComparison.CurrentCultureIgnoreCase))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public readonly bool Enabled;

		private static char clientPrefixListDelimeter = ',';

		private List<string> enabledUserAgentPrefixes;
	}
}
