using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.HttpProxy
{
	internal class DiagnosticsConfiguration
	{
		internal static readonly StringAppSettingsEntry LogFolderPath = new StringAppSettingsEntry("RequestLogger.LogFolderPath", Path.Combine(ExchangeSetupContext.InstallPath, "Logging\\HttpProxy", HttpProxyGlobals.ProtocolType.ToString()), ExTraceGlobals.VerboseTracer);

		internal static readonly StringAppSettingsEntry LogType = new StringAppSettingsEntry("RequestLogger.LogType", "HttpProxy Logs", ExTraceGlobals.VerboseTracer);

		internal static readonly StringAppSettingsEntry LogFileNamePrefix = new StringAppSettingsEntry("RequestLogger.LogFileNamePrefix", "HttpProxy2_", ExTraceGlobals.VerboseTracer);

		internal static readonly IntAppSettingsEntry MaxLogRetentionInDays = new IntAppSettingsEntry("RequestLogger.MaxLogRetentionInDays", 30, ExTraceGlobals.VerboseTracer);

		internal static readonly IntAppSettingsEntry MaxLogDirectorySizeInGB = new IntAppSettingsEntry("RequestLogger.MaxLogDirectorySizeInGB", 1, ExTraceGlobals.VerboseTracer);

		internal static readonly IntAppSettingsEntry MaxLogFileSizeInMB = new IntAppSettingsEntry("RequestLogger.MaxLogFileSizeInMB", 10, ExTraceGlobals.VerboseTracer);

		internal static readonly BoolAppSettingsEntry AddDebugHeadersToTheResponse = new BoolAppSettingsEntry("RequestLogger.AddDebugHeadersToTheResponse", true, ExTraceGlobals.VerboseTracer);

		internal static readonly BoolAppSettingsEntry DetailedLatencyTracingEnabled = new BoolAppSettingsEntry("LatencyTracker.DetailedLatencyTracing", false, ExTraceGlobals.VerboseTracer);
	}
}
