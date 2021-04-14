using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Microsoft.Exchange.Transport.Agent.PhishingDetection
{
	internal static class PhishingDetection
	{
		static PhishingDetection()
		{
			PhishingDetection.LoadConfiguration();
		}

		public static bool TenantHasPhishingEnabled(Guid tenantId)
		{
			return tenantId == Guid.Empty || PhishingDetection.TenantIds.Any((Guid id) => id.Equals(tenantId));
		}

		public static List<KeyValuePair<string, string>> ExtractPhishingUrlsFromContent(string mailBody)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Match match = Regex.Match(mailBody, "(?is)<a.*?href\\s*=\\s*((?:\".*?\")|(?:'.*?')|[^>\\s]+).*?>(.*?)<\\/a>");
			while (match.Success)
			{
				int num = 3072 - stringBuilder.Length;
				if (match.Groups.Count >= 3)
				{
					string link = HttpUtility.UrlDecode(match.Groups[1].Value);
					string text = HttpUtility.UrlDecode(match.Groups[2].Value);
					if (link.Length + text.Length <= num && link.Length <= 2083 && text.Length <= 2083)
					{
						int num2 = text.IndexOf("<", StringComparison.InvariantCultureIgnoreCase);
						while (num2 != -1)
						{
							int num3 = text.IndexOf(">", num2, StringComparison.InvariantCultureIgnoreCase);
							if (num3 != -1)
							{
								text = text.Remove(num2, num3 - num2 + 1);
								num2 = text.IndexOf("<", StringComparison.InvariantCultureIgnoreCase);
							}
							else
							{
								IL_10D:
								while (link.StartsWith("u=", StringComparison.OrdinalIgnoreCase))
								{
									link = link.Substring(2);
								}
								if (link.StartsWith("file:///", StringComparison.OrdinalIgnoreCase))
								{
									link = link.Substring(8);
								}
								if (link.EndsWith("/", StringComparison.OrdinalIgnoreCase) || link.EndsWith(";", StringComparison.OrdinalIgnoreCase))
								{
									link = link.Substring(0, link.Length - 1);
								}
								if (text.EndsWith("/", StringComparison.OrdinalIgnoreCase) || text.EndsWith(";", StringComparison.OrdinalIgnoreCase))
								{
									text = text.Substring(0, text.Length - 1);
								}
								link = link.Replace("\"", string.Empty).Replace("'", string.Empty);
								text = text.Replace("\"", string.Empty).Replace("'", string.Empty);
								if (string.IsNullOrWhiteSpace(link) || string.IsNullOrWhiteSpace(text))
								{
									goto IL_336;
								}
								link = link.Replace("\r\n", string.Empty);
								text = text.Replace("\r\n", string.Empty);
								link = link.Replace(" ", string.Empty);
								text = text.Replace(" ", string.Empty);
								List<string> source = new List<string>
								{
									"mailto:",
									"tel:",
									"sip:",
									"mid:"
								};
								if (!source.Any((string startValue) => link.StartsWith(startValue, StringComparison.OrdinalIgnoreCase)) && !link.Equals(text, StringComparison.OrdinalIgnoreCase) && !text.StartsWith("<img", StringComparison.OrdinalIgnoreCase) && (text.IndexOf(".", StringComparison.InvariantCultureIgnoreCase) >= 0 || text.IndexOf("://", StringComparison.InvariantCultureIgnoreCase) >= 0))
								{
									stringBuilder.Append(link + "|" + text + "; ");
									goto IL_336;
								}
								goto IL_336;
							}
						}
						goto IL_10D;
					}
				}
				IL_336:
				match = match.NextMatch();
			}
			if (stringBuilder.Length == 0)
			{
				return null;
			}
			string text2 = stringBuilder.ToString();
			int num4 = PhishingDetection.KeyNames.LogKeyLength + text2.Length;
			if (num4 > 3072)
			{
				text2 = text2.Substring(0, 3072 - "...".Length) + "...";
			}
			return new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("u", text2)
			};
		}

		public static void LogWarning(string warning)
		{
		}

		private static void LoadConfiguration()
		{
			string location = Assembly.GetExecutingAssembly().Location;
			string text = null;
			try
			{
				Configuration configuration = ConfigurationManager.OpenExeConfiguration(location);
				text = ((configuration.AppSettings.Settings["PhishingDetectionEnabledTenantIds"] == null) ? null : configuration.AppSettings.Settings["PhishingDetectionEnabledTenantIds"].Value);
			}
			catch (ConfigurationErrorsException)
			{
				PhishingDetection.LogWarning(string.Format(CultureInfo.InvariantCulture, "No special Tenant configuration found. Defaulting to :'{0}'.", new object[]
				{
					"5660bb4b-f6c5-47e5-af49-c13b55185dff;4171b533-24dd-48c3-9388-7e4df49fd947"
				}));
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "5660bb4b-f6c5-47e5-af49-c13b55185dff;4171b533-24dd-48c3-9388-7e4df49fd947";
			}
			string[] array = text.Split(new char[]
			{
				';'
			});
			foreach (string text2 in array)
			{
				Guid item;
				if (Guid.TryParse(text2, out item))
				{
					PhishingDetection.TenantIds.Add(item);
				}
				else
				{
					PhishingDetection.LogWarning(string.Format(CultureInfo.InvariantCulture, "Error parsing GUID:'{0}'. Please fix it on configuration file.", new object[]
					{
						text2
					}));
				}
			}
		}

		private const string RegexPattern = "(?is)<a.*?href\\s*=\\s*((?:\".*?\")|(?:'.*?')|[^>\\s]+).*?>(.*?)<\\/a>";

		private const string PhishingDetectionEnabledTenantIdsConfig = "PhishingDetectionEnabledTenantIds";

		private const string DefaultPhishingDetectionEnabledTenantIds = "5660bb4b-f6c5-47e5-af49-c13b55185dff;4171b533-24dd-48c3-9388-7e4df49fd947";

		private const int MaxFormattedLength = 3072;

		private const int MaxLinkLength = 2083;

		private static readonly IList<Guid> TenantIds = new List<Guid>();

		private static class KeyNames
		{
			public static int LogKeyLength
			{
				get
				{
					return "u".Length;
				}
			}

			public const string Url = "u";
		}
	}
}
