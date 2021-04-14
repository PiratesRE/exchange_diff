using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public static class UserAgentSerializer
	{
		static UserAgentSerializer()
		{
			string arg = "NA";
			try
			{
				arg = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
			}
			catch
			{
			}
			UserAgentSerializer.header = string.Format("Exchange\\{0}\\EDiscovery\\EWS\\", arg);
		}

		public static string ToUserAgent(IEnumerable<KeyValuePair<string, string>> values)
		{
			string arg = string.Empty;
			StringBuilder stringBuilder = new StringBuilder(UserAgentSerializer.header);
			if (values != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in values)
				{
					stringBuilder.AppendFormat("{0}{1}={2}", arg, keyValuePair.Key, Uri.EscapeDataString(keyValuePair.Value));
					arg = "&";
				}
			}
			return stringBuilder.ToString();
		}

		public static IEnumerable<KeyValuePair<string, string>> FromUserAgent(string userAgent)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(userAgent))
			{
				string[] array = userAgent.Split(UserAgentSerializer.separator, 5);
				if (array.Length > 1)
				{
					string query = array[array.Length - 1];
					NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(query);
					for (int i = 0; i < nameValueCollection.Count; i++)
					{
						if (nameValueCollection.AllKeys[i] != null)
						{
							dictionary[nameValueCollection.AllKeys[i]] = nameValueCollection[i];
						}
					}
				}
			}
			return dictionary;
		}

		private const string HeaderFormat = "Exchange\\{0}\\EDiscovery\\EWS\\";

		private const string Ampersand = "&";

		private const int PartCount = 5;

		private static readonly string header;

		private static char[] separator = new char[]
		{
			'\\'
		};
	}
}
