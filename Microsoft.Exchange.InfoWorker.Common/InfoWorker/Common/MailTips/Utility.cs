using System;
using System.IO;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.MailTips;
using Microsoft.Win32;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	public static class Utility
	{
		internal static TimeSpan RegistryCheckInterval
		{
			get
			{
				return Utility.registryCheckInterval;
			}
			set
			{
				Utility.nextRegistryCheck = DateTime.MinValue;
				Utility.registryCheckInterval = value;
			}
		}

		internal static bool RenderingDisabled
		{
			get
			{
				Utility.CheckDisableSwitch();
				return Utility.renderingDisabled;
			}
		}

		internal static void CheckDisableSwitch()
		{
			DateTime utcNow = DateTime.UtcNow;
			if (utcNow > Utility.nextRegistryCheck)
			{
				Utility.Tracer.TraceDebug(0L, "Checking disable-rendering switch");
				Utility.nextRegistryCheck = utcNow.Add(Utility.registryCheckInterval);
				Utility.renderingDisabled = Utility.IsDisabled("DisableGroupMetricsRendering");
			}
		}

		private static bool IsDisabled(string name)
		{
			bool flag = false;
			try
			{
				object value = Registry.GetValue(Utility.RegistryKeyName, name, 0);
				Utility.Tracer.TraceDebug<string, object>(0L, "IsDisabled {0} raw value {1}", name, value);
				if (value is int && (int)value > 0)
				{
					flag = true;
				}
			}
			catch (IOException arg)
			{
				Utility.Tracer.TraceDebug<string, IOException>(0L, "IsDisabled {0} exception {1}", name, arg);
			}
			catch (SecurityException arg2)
			{
				Utility.Tracer.TraceDebug<string, SecurityException>(0L, "IsDisabled {0} exception {1}", name, arg2);
			}
			Utility.Tracer.TraceDebug<string, bool>(0L, "IsDisabled {0} result {1}", name, flag);
			return flag;
		}

		internal const int RetryADLimit = 3;

		internal const string DisableGroupMetricsRendering = "DisableGroupMetricsRendering";

		internal static readonly Trace Tracer = ExTraceGlobals.GroupMetricsTracer;

		internal static readonly ExEventLog Logger = new ExEventLog(Utility.Tracer.Category, "MSExchange MailTips");

		internal static string RegistryKeyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		private static TimeSpan registryCheckInterval = TimeSpan.FromMinutes(10.0);

		private static DateTime nextRegistryCheck = DateTime.MinValue;

		private static bool renderingDisabled = false;
	}
}
