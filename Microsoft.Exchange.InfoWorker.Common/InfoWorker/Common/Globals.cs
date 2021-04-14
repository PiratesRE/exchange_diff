using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal static class Globals
	{
		public const string ComponentGuidString = "3A8BB7C6-6298-45eb-BE95-1A3AF02F7FFA";

		public const string AvailabilityEventSource = "MSExchange Availability";

		public const string OOFEventSource = "MSExchangeMailboxAssistants";

		public const string ELCEventSource = "MSExchangeMailboxAssistants";

		public const string ServicesNamespaceBase = "http://schemas.microsoft.com/exchange/services/2006";

		public const string ServicesTypeNamespace = "http://schemas.microsoft.com/exchange/services/2006/types";

		public const string ServicesErrorNamespace = "http://schemas.microsoft.com/exchange/services/2006/errors";

		public static readonly Delayed<string> ProcessId = new Delayed<string>(delegate()
		{
			string text = "unknown";
			try
			{
				text = AppDomain.CurrentDomain.FriendlyName;
			}
			catch (SystemException)
			{
			}
			string text2 = "unknown";
			string text3 = "unknown";
			try
			{
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					text2 = currentProcess.Id.ToString();
					text3 = currentProcess.MainModule.ModuleName;
				}
			}
			catch (Win32Exception)
			{
			}
			return string.Concat(new string[]
			{
				text2,
				"[",
				text3,
				":",
				text,
				"]"
			});
		});

		public static readonly Guid ComponentGuid = new Guid("3A8BB7C6-6298-45eb-BE95-1A3AF02F7FFA");

		public static ExEventLog AvailabilityLogger = new ExEventLog(ExTraceGlobals.SingleInstanceItemTracer.Category, "MSExchange Availability");

		public static ExEventLog OOFLogger = new ExEventLog(ExTraceGlobals.SingleInstanceItemTracer.Category, "MSExchangeMailboxAssistants");

		public static ExEventLog ELCLogger = new ExEventLog(ExTraceGlobals.ELCTracer.Category, "MSExchangeMailboxAssistants");

		public static readonly string CertificateValidationComponentId = "AvailabilityService";

		public static readonly int E14Version = new ServerVersion(14, 0, 0, 0).ToInt();

		public static readonly int E14SP1Version = new ServerVersion(14, 1, 0, 0).ToInt();

		public static readonly int E14SP2Version = new ServerVersion(14, 2, 0, 0).ToInt();

		public static readonly int E14SP3Version = new ServerVersion(14, 3, 0, 0).ToInt();

		public static readonly int E15Version = new ServerVersion(15, 0, 0, 0).ToInt();
	}
}
