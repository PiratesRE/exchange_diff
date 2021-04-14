using System;
using System.Linq;
using System.Security;
using System.Text;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	public class AirSyncInMemoryTraceHandler : ExchangeDiagnosableWrapper<AirSyncTraces>
	{
		protected override string UsageText
		{
			get
			{
				return "The In-Memory tracing handler is a diagnostics handler that returns the currently collected in-memory traces. Below are examples for using this diagnostics handler: ";
			}
		}

		protected override string UsageSample
		{
			get
			{
				return " Example 1: Returns health for all devices in cache\r\n                            Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component AirSyncInMemoryTrace";
			}
		}

		public static AirSyncInMemoryTraceHandler GetInstance()
		{
			if (AirSyncInMemoryTraceHandler.instance == null)
			{
				lock (AirSyncInMemoryTraceHandler.lockObject)
				{
					if (AirSyncInMemoryTraceHandler.instance == null)
					{
						AirSyncInMemoryTraceHandler.instance = new AirSyncInMemoryTraceHandler();
					}
				}
			}
			return AirSyncInMemoryTraceHandler.instance;
		}

		private AirSyncInMemoryTraceHandler()
		{
		}

		protected override string ComponentName
		{
			get
			{
				return "AirSyncInMemoryTrace";
			}
		}

		internal override AirSyncTraces GetExchangeDiagnosticsInfoData(DiagnosableParameters arguments)
		{
			AirSyncTraces airSyncTraces = new AirSyncTraces();
			StringBuilder tracesBuilder = new StringBuilder("TimeStamp,TraceTag,FormatString,NativeThreadId,ComponentGuid,TraceTag,StartIndex,Id\r\n");
			if (AirSyncDiagnostics.IsInMemoryTracingEnabled() && AirSyncDiagnostics.TroubleshootingContext.MemoryTraceBuilder != null)
			{
				AirSyncDiagnostics.TroubleshootingContext.MemoryTraceBuilder.GetTraces().ToList<TraceEntry>().ForEach(delegate(TraceEntry traceLine)
				{
					tracesBuilder.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", new object[]
					{
						traceLine.Timestamp,
						traceLine.TraceType,
						traceLine.FormatString,
						traceLine.NativeThreadId,
						traceLine.ComponentGuid,
						traceLine.TraceTag,
						traceLine.StartIndex,
						traceLine.Id
					}));
				});
				airSyncTraces.TraceData = SecurityElement.Escape(tracesBuilder.ToString());
				airSyncTraces.TracingEnabled = new bool?(true);
			}
			else
			{
				airSyncTraces.TracingEnabled = new bool?(false);
			}
			return airSyncTraces;
		}

		private const string TraceHeader = "TimeStamp,TraceTag,FormatString,NativeThreadId,ComponentGuid,TraceTag,StartIndex,Id\r\n";

		private const string TraceFormatString = "{0},{1},{2},{3},{4},{5},{6},{7}";

		private static AirSyncInMemoryTraceHandler instance;

		private static object lockObject = new object();
	}
}
