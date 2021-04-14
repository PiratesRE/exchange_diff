using System;
using System.Net;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Logging.WorkloadManagement
{
	internal class TransportWlmLog
	{
		public static LogSchema Schema
		{
			get
			{
				if (TransportWlmLog.schema == null)
				{
					LogSchema value = new LogSchema("Microsoft Exchange Server", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Transport Workload Management Log", TransportWlmLog.Row.Headers);
					Interlocked.CompareExchange<LogSchema>(ref TransportWlmLog.schema, value, null);
				}
				return TransportWlmLog.schema;
			}
		}

		public static void Start()
		{
			TransportWlmLog.CreateLog();
			TransportWlmLog.Configure(Components.Configuration.LocalServer);
			TransportWlmLog.RegisterConfigurationChangeHandlers();
			TransportWlmLog.processRole = Components.Configuration.ProcessTransportRole;
			TransportWlmLog.hostName = Dns.GetHostName();
		}

		public static void Stop()
		{
			TransportWlmLog.UnregisterConfigurationChangeHandlers();
			if (TransportWlmLog.log != null)
			{
				TransportWlmLog.log.Close();
			}
		}

		public static void FlushBuffer()
		{
			if (TransportWlmLog.log != null)
			{
				TransportWlmLog.log.Flush();
			}
		}

		public static void StartTest(string directory)
		{
			TransportWlmLog.CreateLog();
			TransportWlmLog.log.Configure(directory, TimeSpan.FromDays(1.0), 0L, 0L, 0, TimeSpan.MaxValue);
		}

		public static void LogActivity(string messageId, Guid tenantId, string source, IActivityScope scope)
		{
			if (scope == null)
			{
				throw new ArgumentNullException("scope");
			}
			if (!TransportWlmLog.enabled)
			{
				return;
			}
			TransportWlmLog.Row row = new TransportWlmLog.Row
			{
				MessageId = messageId,
				TenantId = tenantId,
				Source = source,
				Process = TransportWlmLog.processRole,
				HostName = TransportWlmLog.hostName,
				Description = string.Format("[{0}]", LogRowFormatter.FormatCollection(WorkloadManagementLogger.FormatWlmActivity(scope, true)))
			};
			row.Log(source);
		}

		private static void CreateLog()
		{
			TransportWlmLog.log = new Log("TRANSPORTWLMLOG", new LogHeaderFormatter(TransportWlmLog.Schema), "TransportWorkloadManagementLog");
		}

		private static void RegisterConfigurationChangeHandlers()
		{
			Components.Configuration.LocalServerChanged += TransportWlmLog.Configure;
		}

		private static void UnregisterConfigurationChangeHandlers()
		{
			Components.Configuration.LocalServerChanged -= TransportWlmLog.Configure;
		}

		private static void Configure(TransportServerConfiguration server)
		{
			Server transportServer = server.TransportServer;
			if (transportServer.WlmLogPath == null || string.IsNullOrEmpty(transportServer.WlmLogPath.PathName))
			{
				TransportWlmLog.enabled = false;
				ExTraceGlobals.GeneralTracer.TraceDebug(0L, "Transport Wlm Log path was set to null, Transport Wlm Log is disabled");
				Components.EventLogger.LogEvent(TransportEventLogConstants.Tuple_TransportWlmLogPathIsNull, null, new object[0]);
				return;
			}
			TransportWlmLog.enabled = true;
			TransportWlmLog.log.Configure(transportServer.WlmLogPath.PathName, transportServer.WlmLogMaxAge, (long)(transportServer.WlmLogMaxDirectorySize.IsUnlimited ? 0UL : transportServer.WlmLogMaxDirectorySize.Value.ToBytes()), (long)(transportServer.WlmLogMaxFileSize.IsUnlimited ? 0UL : transportServer.WlmLogMaxFileSize.Value.ToBytes()), Components.TransportAppConfig.Logging.TransportWlmLogBufferSize, Components.TransportAppConfig.Logging.TransportWlmLogFlushInterval);
		}

		private const string LogComponentName = "TransportWorkloadManagementLog";

		private const string LogFileName = "TRANSPORTWLMLOG";

		private static LogSchema schema;

		private static Log log;

		private static bool enabled;

		private static ProcessTransportRole processRole;

		private static string hostName;

		public struct Source
		{
			public const string Categorizer = "CAT";
		}

		private sealed class Row : LogRowFormatter
		{
			public Row() : base(TransportWlmLog.Schema)
			{
			}

			public static string[] Headers
			{
				get
				{
					if (TransportWlmLog.Row.headers == null)
					{
						string[] array = new string[Enum.GetValues(typeof(TransportWlmLog.Row.Field)).Length];
						array[0] = "date-time";
						array[1] = "message-id";
						array[2] = "tenant-id";
						array[3] = "hostname";
						array[4] = "process";
						array[5] = "source";
						array[6] = "description";
						Interlocked.CompareExchange<string[]>(ref TransportWlmLog.Row.headers, array, null);
					}
					return TransportWlmLog.Row.headers;
				}
			}

			public string MessageId
			{
				set
				{
					base[1] = value;
				}
			}

			public Guid TenantId
			{
				set
				{
					base[2] = value;
				}
			}

			public string HostName
			{
				set
				{
					base[3] = value;
				}
			}

			public ProcessTransportRole Process
			{
				set
				{
					base[4] = value;
				}
			}

			public string Source
			{
				set
				{
					base[5] = value;
				}
			}

			public string Description
			{
				set
				{
					base[6] = value;
				}
			}

			public void Log(string component)
			{
				try
				{
					TransportWlmLog.log.Append(this, 0);
				}
				catch (ObjectDisposedException)
				{
					ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Appending to transport WLM log for component {0} failed with ObjectDisposedException", component);
				}
			}

			private static string[] headers;

			public enum Field
			{
				Time,
				MessageId,
				TenantId,
				HostName,
				Process,
				Source,
				Description
			}
		}
	}
}
