using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Diagnostics.Internal
{
	internal class LoggingFormatter
	{
		public static bool IsSeparator(char c)
		{
			return c == ';' || c == ':' || c == '=' || c == '|';
		}

		public static List<KeyValuePair<string, object>> GetAgentInfoString(List<List<KeyValuePair<string, string>>> data)
		{
			if (data == null)
			{
				return null;
			}
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
			foreach (List<KeyValuePair<string, string>> list2 in data)
			{
				if (list2.Count != 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					KeyValuePair<string, string> keyValuePair = list2[0];
					string key = LoggingFormatter.EncodeAgentName(keyValuePair.Key);
					stringBuilder.AppendFormat("{0}|", keyValuePair.Value);
					for (int i = 1; i < list2.Count; i++)
					{
						stringBuilder.AppendFormat("{0}={1}|", list2[i].Key, list2[i].Value);
					}
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
					list.Add(new KeyValuePair<string, object>(key, stringBuilder.ToString()));
				}
			}
			return list;
		}

		public static string EncodeAgentName(string name)
		{
			StringBuilder stringBuilder = null;
			for (int i = 0; i < name.Length; i++)
			{
				char c = name[i];
				string text = null;
				if (LoggingFormatter.IsSeparator(c) || c == '-')
				{
					text = "_";
				}
				else if (Convert.ToInt32(c) < 32)
				{
					text = "?";
				}
				else if (Convert.ToInt32(c) > 127)
				{
					string str = "\\u";
					int num = (int)c;
					text = str + num.ToString("x4");
				}
				if (text != null)
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(name.Length);
						stringBuilder.Append(name, 0, i);
					}
					stringBuilder.Append(text);
				}
				else if (stringBuilder != null)
				{
					stringBuilder.Append(c);
				}
			}
			if (stringBuilder != null)
			{
				return stringBuilder.ToString();
			}
			return name;
		}

		public const char AgentNameSeparator = '-';

		public const char ServerSeparator = ';';

		public const char ServerFqdnSeparator = ':';

		public const char ComponentValueSeparator = '=';

		public const char LatencyRecordSeparator = '|';
	}
}
