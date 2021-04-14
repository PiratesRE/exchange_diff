using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Clutter
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class InferenceDiagnosticsLog
	{
		static InferenceDiagnosticsLog()
		{
			InferenceDiagnosticsLog.Schema = new LogSchema("Microsoft Exchange", InferenceDiagnosticsLog.Version, "InferenceDiagnosticsLog", InferenceDiagnosticsLog.Columns);
			string fileNamePrefix = string.Format("{0}_{1}_{2}_", "InferenceDiagnosticsLog", InferenceDiagnosticsLog.ProcessName, InferenceDiagnosticsLog.ProcessId.ToString());
			InferenceDiagnosticsLog.Logger = new Log(fileNamePrefix, new LogHeaderFormatter(InferenceDiagnosticsLog.Schema), "InferenceDiagnosticsLog");
			InferenceDiagnosticsLog.Logger.Configure(InferenceDiagnosticsLog.LogPath, TimeSpan.FromDays(7.0), (long)ByteQuantifiedSize.FromGB(1UL).ToBytes(), (long)ByteQuantifiedSize.FromMB(10UL).ToBytes());
		}

		public static void Log(string source, object detail)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(InferenceDiagnosticsLog.Schema);
			logRowFormatter[0] = DateTime.UtcNow;
			logRowFormatter[1] = InferenceDiagnosticsLog.ProcessName;
			logRowFormatter[2] = source;
			logRowFormatter[3] = detail;
			InferenceDiagnosticsLog.Logger.Append(logRowFormatter, -1);
		}

		private const string SoftwareName = "Microsoft Exchange";

		private const string LogType = "InferenceDiagnosticsLog";

		private static readonly string LogPath = Path.Combine(ExchangeSetupContext.LoggingPath, "InferenceDiagnosticsLog");

		private static readonly Log Logger;

		private static readonly LogSchema Schema;

		private static readonly string Version = "15.00.1497.012";

		private static readonly string[] Columns = new string[]
		{
			"Timestamp",
			"ProcessName",
			"Source",
			"Detail"
		};

		private static readonly int ProcessId = Process.GetCurrentProcess().Id;

		private static readonly string ProcessName = Process.GetCurrentProcess().ProcessName;
	}
}
