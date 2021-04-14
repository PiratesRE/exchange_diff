using System;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Network
{
	internal static class NetworkUtils
	{
		public static IPAddress GetNetworkAddress(IPAddress address, IPAddress subnetMask)
		{
			byte[] addressBytes = address.GetAddressBytes();
			byte[] addressBytes2 = subnetMask.GetAddressBytes();
			if (addressBytes.Length > 0 && addressBytes.Length != addressBytes2.Length)
			{
				return address;
			}
			byte[] array = new byte[addressBytes.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (addressBytes[i] & addressBytes2[i]);
			}
			return new IPAddress(array);
		}

		public static void LogWorkItemMessage(TracingContext context, WorkItemResult result, string message, params object[] messageParams)
		{
			if (message.Length > 0)
			{
				message = string.Format(message, messageParams);
			}
			if (context != null)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ServiceTracer, context, message, null, "LogWorkItemMessage", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Network\\NetworkUtils.cs", 74);
			}
			if (result != null)
			{
				result.StateAttribute1 = (result.StateAttribute1 ?? string.Empty);
				result.StateAttribute1 = result.StateAttribute1 + message.Replace("\n", "<br />\r\n") + "<br />\r\n";
			}
		}

		public static string ResolveHostARecord(string hostName, string dnsServer = null)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = Environment.SystemDirectory + "\\nslookup.exe",
				Arguments = string.Format("-type=A {0} {1}", hostName, dnsServer),
				UseShellExecute = false,
				RedirectStandardOutput = true
			};
			string input = string.Empty;
			using (Process process = Process.Start(startInfo))
			{
				input = process.StandardOutput.ReadToEnd().Trim();
			}
			Match match = Regex.Match(input, "Address:\\s+(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})$");
			if (!match.Success)
			{
				return null;
			}
			return match.Groups[1].Value;
		}

		public const ulong OneGiga = 1000000000UL;

		public const ulong TenGiga = 10000000000UL;
	}
}
