using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Configuration.Core
{
	internal static class Constants
	{
		internal static ExEventLog CoreEventLogger
		{
			get
			{
				return Constants.coreEventLogger.Value;
			}
		}

		internal static int ProcessId
		{
			get
			{
				if (Constants.processId == null)
				{
					using (Process currentProcess = Process.GetCurrentProcess())
					{
						Constants.processId = new int?(currentProcess.Id);
					}
				}
				return Constants.processId.Value;
			}
		}

		internal static string ProcessName
		{
			get
			{
				return ExWatson.RealAppName;
			}
		}

		internal static bool IsPowerShellWebService
		{
			get
			{
				return EventLogConstants.IsPowerShellWebService;
			}
		}

		internal static bool IsRemotePS
		{
			get
			{
				return !Constants.IsPowerShellWebService;
			}
		}

		public const string HttpContextUserSidItemKey = "X-EX-UserSid";

		private static readonly Lazy<ExEventLog> coreEventLogger = new Lazy<ExEventLog>(() => new ExEventLog(ExTraceGlobals.InstrumentationTracer.Category, "MSExchange Configuration Core"));

		private static int? processId;
	}
}
