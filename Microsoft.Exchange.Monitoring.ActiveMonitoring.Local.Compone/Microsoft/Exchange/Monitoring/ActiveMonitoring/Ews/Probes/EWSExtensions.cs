using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Ews.Probes
{
	public static class EWSExtensions
	{
		public static void AddUnique(this Dictionary<string, string> dictionary, string key, string val)
		{
			lock (EWSExtensions.lockObj)
			{
				string key2 = string.Format("{0}{1}{2}", EWSExtensions.uniqueId, EWSExtensions.logSeparator, key);
				dictionary.Add(key2, val);
				EWSExtensions.uniqueId++;
			}
		}

		public static bool TryConvertRegularLogKey(string unique, out string normal)
		{
			normal = unique;
			try
			{
				if (unique.Contains(EWSExtensions.logSeparator))
				{
					int num = unique.IndexOf(EWSExtensions.logSeparator) + EWSExtensions.logSeparator.Length;
					normal = unique.Substring(num, unique.Length - num);
					return true;
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		private static int uniqueId = 1;

		private static object lockObj = new object();

		private static string logSeparator = "{$#!";
	}
}
