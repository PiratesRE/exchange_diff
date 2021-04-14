using System;
using System.Collections.Concurrent;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class ThemeManagerFactory
	{
		public static ThemeManager GetInstance(string owaVersion)
		{
			if (!ThemeManagerFactory.themeManagerCollection.ContainsKey(owaVersion))
			{
				ThemeManager value = new ThemeManager(owaVersion);
				ThemeManagerFactory.themeManagerCollection.TryAdd(owaVersion, value);
			}
			return ThemeManagerFactory.themeManagerCollection[owaVersion];
		}

		private static ConcurrentDictionary<string, ThemeManager> themeManagerCollection = new ConcurrentDictionary<string, ThemeManager>();
	}
}
