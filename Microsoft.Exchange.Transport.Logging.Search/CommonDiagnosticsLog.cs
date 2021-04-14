using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	internal sealed class CommonDiagnosticsLog
	{
		private void Configure(HostId hostId)
		{
			if (!string.IsNullOrEmpty(CommonDiagnosticsLog.LogPath))
			{
				string fileNamePrefix = Names<HostId>.Map[(int)hostId] + "." + "COMMONDIAG";
				this.log = new Log(fileNamePrefix, new LogHeaderFormatter(CommonDiagnosticsLog.commonDiagnosticsLogSchema), "CommonDiagnostics");
				this.log.Configure(CommonDiagnosticsLog.LogPath, TimeSpan.FromDays(30.0), 10485760L, 1048576L);
				this.initialized = true;
			}
		}

		public static CommonDiagnosticsLog Instance
		{
			get
			{
				if (CommonDiagnosticsLog.instance == null)
				{
					lock (CommonDiagnosticsLog.initLock)
					{
						if (CommonDiagnosticsLog.instance == null)
						{
							if (CommonDiagnosticsLog.hostId == HostId.NotInitialized)
							{
								throw new InvalidOperationException("CommonDiagnosticsLog is not yet initialized");
							}
							CommonDiagnosticsLog commonDiagnosticsLog = new CommonDiagnosticsLog();
							commonDiagnosticsLog.Configure(CommonDiagnosticsLog.hostId);
							CommonDiagnosticsLog.instance = commonDiagnosticsLog;
						}
					}
				}
				return CommonDiagnosticsLog.instance;
			}
		}

		public static string LogPath
		{
			get
			{
				string path = null;
				try
				{
					path = ConfigurationContext.Setup.InstallPath;
				}
				catch (SecurityException)
				{
					return null;
				}
				catch (IOException)
				{
					return null;
				}
				string result;
				try
				{
					result = Path.Combine(path, "TransportRoles\\Logs\\CommonDiagnosticsLog");
				}
				catch (ArgumentException)
				{
					result = null;
				}
				return result;
			}
		}

		public static void Initialize(HostId hostId)
		{
			CommonDiagnosticsLog.hostId = hostId;
		}

		public void LogEvent(CommonDiagnosticsLog.Source source, IEnumerable<KeyValuePair<string, object>> eventData)
		{
			if (!this.initialized)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(CommonDiagnosticsLog.commonDiagnosticsLogSchema);
			logRowFormatter[1] = Names<CommonDiagnosticsLog.Source>.Map[(int)source];
			logRowFormatter[2] = eventData;
			this.log.Append(logRowFormatter, 0);
		}

		public const string LogName = "COMMONDIAG";

		public const string LogType = "Common Diagnostics Log";

		public const string LogComponent = "CommonDiagnostics";

		private static string[] fields = new string[]
		{
			"TimeStamp",
			"Source",
			"EventData"
		};

		private static object initLock = new object();

		private static HostId hostId;

		private static LogSchema commonDiagnosticsLogSchema = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Common Diagnostics Log", CommonDiagnosticsLog.fields);

		public static CsvTable CommonDiagnosticsLogTable = new CsvTable(new CsvField[]
		{
			new CsvField(CommonDiagnosticsLog.fields[0], typeof(DateTime)),
			new CsvField(CommonDiagnosticsLog.fields[1], typeof(string)),
			new CsvField(CommonDiagnosticsLog.fields[2], typeof(KeyValuePair<string, object>[]))
		});

		private static CommonDiagnosticsLog instance;

		private bool initialized;

		private Log log;

		internal enum Source
		{
			DeliveryReports,
			Trace,
			DeliveryReportsCache,
			MailSubmissionService,
			MailboxDeliveryService,
			MailboxTransportSubmissionService
		}
	}
}
