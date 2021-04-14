using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal static class DiagnosticsItemFactory
	{
		public static DiagnosticsItemBase Create(string header)
		{
			DiagnosticsItemBase diagnosticsItemBase = new DiagnosticsItemUnknown();
			if (!string.IsNullOrEmpty(header))
			{
				Match match = DiagnosticsItemFactory.msDiagnosticsPattern.Match(header);
				if (match.Success)
				{
					int num;
					if (int.TryParse(match.Groups["errorid"].Value, out num))
					{
						if (DiagnosticsItemCallRedirect.IsExpectedErrorId(num))
						{
							diagnosticsItemBase = new DiagnosticsItemCallRedirect();
						}
						else if (DiagnosticsItemCallReceived.IsExpectedErrorId(num))
						{
							diagnosticsItemBase = new DiagnosticsItemCallReceived();
						}
						else if (DiagnosticsItemLyncServer.IsExpectedErrorId(num))
						{
							diagnosticsItemBase = new DiagnosticsItemLyncServer();
						}
						else if (DiagnosticsItemExchangeServer.IsExpectedErrorId(num))
						{
							diagnosticsItemBase = new DiagnosticsItemExchangeServer();
						}
						else if (DiagnosticsItemTrace.IsExpectedErrorId(num))
						{
							diagnosticsItemBase = new DiagnosticsItemTrace();
						}
						else if (DiagnosticsItemCallStart.IsExpectedErrorId(num))
						{
							diagnosticsItemBase = new DiagnosticsItemCallStart();
						}
						else if (DiagnosticsItemCallEstablishing.IsExpectedErrorId(num))
						{
							diagnosticsItemBase = new DiagnosticsItemCallEstablishing();
						}
						else if (DiagnosticsItemCallEstablished.IsExpectedErrorId(num))
						{
							diagnosticsItemBase = new DiagnosticsItemCallEstablished();
						}
						else if (DiagnosticsItemCallEstablishFailed.IsExpectedErrorId(num))
						{
							diagnosticsItemBase = new DiagnosticsItemCallEstablishFailed();
						}
						else if (DiagnosticsItemCallDisconnected.IsExpectedErrorId(num))
						{
							diagnosticsItemBase = new DiagnosticsItemCallDisconnected();
						}
						diagnosticsItemBase.ErrorId = num;
					}
					int count = match.Groups["key"].Captures.Count;
					for (int i = 0; i < count; i++)
					{
						diagnosticsItemBase.Add(match.Groups["key"].Captures[i].Value, match.Groups["value"].Captures[i].Value);
					}
				}
			}
			return diagnosticsItemBase;
		}

		public static string FormatDiagnostics(int errorid, string reason, params string[] additional)
		{
			string value = string.Format("{0};source=\"{1}\";reason=\"{2}\";service=\"{3}/{4}\"", new object[]
			{
				errorid,
				DiagnosticsItemFactory.GetLocalHostFqdn(),
				reason,
				DiagnosticsItemFactory.GetAssemblyName(),
				DiagnosticsItemFactory.GetAssemblyVersion()
			});
			StringBuilder stringBuilder = new StringBuilder(value);
			if (additional != null)
			{
				foreach (string value2 in additional)
				{
					stringBuilder.Append(";");
					stringBuilder.Append(value2);
				}
			}
			return stringBuilder.ToString();
		}

		private static string GetAssemblyVersion()
		{
			return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
		}

		private static string GetAssemblyName()
		{
			return Assembly.GetExecutingAssembly().GetName().Name;
		}

		private static string GetLocalHostFqdn()
		{
			IPGlobalProperties ipglobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			if (!string.IsNullOrEmpty(ipglobalProperties.DomainName))
			{
				return string.Format("{0}.{1}", ipglobalProperties.HostName, ipglobalProperties.DomainName);
			}
			return ipglobalProperties.HostName;
		}

		private static Regex msDiagnosticsPattern = new Regex("(?<errorid>\\d+)(;(?<key>\\w+)=\"(?<value>[^\"\\\\]*(?:\\\\.[^\"\\\\]*)*)\")*");
	}
}
