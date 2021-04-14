using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net
{
	internal class DnsLog
	{
		public static void Start(string dnsLogPath, TimeSpan dnsLogmaxAge, long dnsLogMaxDirectorySize, long dnsLogMaxFileSize)
		{
			if (DnsLog.enabled)
			{
				throw new InvalidOperationException("DnsLogPath cannot be started twice");
			}
			ArgumentValidator.ThrowIfOutOfRange<long>("dnsLogMaxDirectorySize", dnsLogMaxDirectorySize, 0L, long.MaxValue);
			ArgumentValidator.ThrowIfOutOfRange<long>("dnsLogMaxFileSize", dnsLogMaxFileSize, 0L, long.MaxValue);
			if (string.IsNullOrEmpty(dnsLogPath))
			{
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				dnsLogPath = Path.Combine(Directory.GetParent(Path.GetDirectoryName(executingAssembly.Location)).FullName, "TransportRoles\\Logs\\Dns\\");
			}
			DnsLog.dnsLogSchema = new LogSchema("Microsoft Exchange Server", "15.00.1497.012", "DNS log", DnsLog.Fields);
			DnsLog.log = new AsyncLog("DnsLog", new LogHeaderFormatter(DnsLog.dnsLogSchema), "DnsLogs");
			DnsLog.log.Configure(dnsLogPath, dnsLogmaxAge, dnsLogMaxDirectorySize, dnsLogMaxFileSize, 5242880, TimeSpan.FromSeconds(60.0), TimeSpan.FromSeconds(15.0));
			DnsLog.enabled = true;
			DnsLog.LogServiceStart();
		}

		public static void LogServiceStart()
		{
			if (!DnsLog.enabled)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(DnsLog.dnsLogSchema);
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				logRowFormatter[3] = string.Format("DNS Logging for process {0} started.", currentProcess.ProcessName);
			}
			logRowFormatter[1] = DnsLogEventId.START;
			DnsLog.log.Append(logRowFormatter, 0);
		}

		public static void Stop()
		{
			if (DnsLog.log != null)
			{
				DnsLog.enabled = false;
				DnsLog.log.Close();
				DnsLog.log = null;
			}
		}

		private static string[] InitializeFields()
		{
			string[] array = new string[Enum.GetValues(typeof(DnsLog.DnsLogField)).Length];
			array[0] = "Timestamp";
			array[1] = "EventId";
			array[2] = "RequestId";
			array[3] = "Data";
			return array;
		}

		public static void Log(object request, string format, params object[] parameters)
		{
			if (!DnsLog.enabled)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(DnsLog.dnsLogSchema);
			logRowFormatter[3] = string.Format(format, parameters);
			logRowFormatter[2] = ((request == null) ? 0 : request.GetHashCode());
			DnsLog.Append(logRowFormatter);
		}

		private static void Append(LogRowFormatter row)
		{
			try
			{
				DnsLog.log.Append(row, 0);
			}
			catch (ObjectDisposedException)
			{
				ExTraceGlobals.DNSTracer.TraceError(0L, "DnsLog append failed with ObjectDisposedException");
			}
		}

		private const string LogComponentName = "DnsLogs";

		private static readonly string[] Fields = DnsLog.InitializeFields();

		private static LogSchema dnsLogSchema;

		private static AsyncLog log;

		private static bool enabled;

		internal enum DnsLogField
		{
			Time,
			EventId,
			RequestId,
			Data
		}
	}
}
