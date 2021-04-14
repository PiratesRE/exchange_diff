using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Win32;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	internal class DiagnosticWatson
	{
		private static KeyType TryReadRegistryKey<KeyType>(string value, KeyType defaultValue)
		{
			Exception ex = null;
			try
			{
				object value2 = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\DeliveryReports", value, defaultValue);
				if (value2 == null || !(value2 is KeyType))
				{
					return defaultValue;
				}
				return (KeyType)((object)value2);
			}
			catch (SecurityException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.MessageTrackingTracer.TraceError<string, string, Exception>(0L, "Failed to read registry key: {0}\\{1}, {2}", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\DeliveryReports", value, ex);
			}
			return defaultValue;
		}

		public static void SendWatsonWithoutCrash(Exception e, string key, TimeSpan interval)
		{
			DiagnosticWatson.SendWatsonWithoutCrash(e, key, interval, null);
		}

		public static void SendWatsonWithoutCrash(Exception e, string key, TimeSpan interval, string extraData)
		{
			if (DiagnosticWatson.reportNonFatalBugs == 0)
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			lock (DiagnosticWatson.dumpLock)
			{
				DateTime dateTime;
				if (!DiagnosticWatson.watsonHistory.TryGetValue(key, out dateTime) || !(utcNow < dateTime))
				{
					ExWatson.SendReport(e, ReportOptions.None, extraData);
					dateTime = DateTime.UtcNow + interval;
					DiagnosticWatson.watsonHistory[key] = dateTime;
				}
			}
		}

		private const string DeliveryReportsRegkey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\DeliveryReports";

		private static object dumpLock = new object();

		private static Dictionary<string, DateTime> watsonHistory = new Dictionary<string, DateTime>(5);

		private static int reportNonFatalBugs = DiagnosticWatson.TryReadRegistryKey<int>("ReportNonFatalBugs", 0);
	}
}
