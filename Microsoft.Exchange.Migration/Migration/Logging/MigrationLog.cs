using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.SyncMigrationServicelet;

namespace Microsoft.Exchange.Migration.Logging
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationLog : ADConfigurationLoader<Server, Server>
	{
		public MigrationLog()
		{
			this.configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 120, ".ctor", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Migration\\DataAccessLayer\\MigrationLog.cs");
			base.ReadConfiguration();
		}

		public static string DefaultLogPath
		{
			get
			{
				if (MigrationLog.defaultLogPath == null)
				{
					string text = ExchangeSetupContext.InstallPath;
					if (text == null)
					{
						text = Assembly.GetExecutingAssembly().Location;
						text = Path.GetDirectoryName(text);
					}
					MigrationLog.defaultLogPath = Path.Combine(text, "logging\\MigrationLogs");
				}
				return MigrationLog.defaultLogPath;
			}
			internal set
			{
				MigrationLog.defaultLogPath = value;
			}
		}

		public void Close()
		{
			lock (MigrationLog.lockObj)
			{
				if (this.log != null)
				{
					this.log.Close();
					this.log = null;
				}
			}
		}

		public void Log(string source, MigrationEventType eventType, object context, string format, params object[] args)
		{
			foreach (KeyValuePair<MigrationEventType, MigrationLog.Tracer> keyValuePair in MigrationLog.Tracers)
			{
				if (keyValuePair.Key == eventType)
				{
					keyValuePair.Value((long)context.GetHashCode(), string.Join(",", new string[]
					{
						source,
						context.ToString(),
						format
					}), args);
					break;
				}
			}
			if (this.log == null || !this.IsLoggingLevelEnabled(eventType))
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(MigrationLog.Schema);
			logRowFormatter[1] = Thread.CurrentThread.ManagedThreadId;
			logRowFormatter[3] = eventType;
			logRowFormatter[2] = source;
			logRowFormatter[4] = context.ToString();
			if (args != null)
			{
				logRowFormatter[5] = string.Format(CultureInfo.InvariantCulture, format, args);
			}
			else
			{
				logRowFormatter[5] = format;
			}
			lock (MigrationLog.lockObj)
			{
				this.log.Append(logRowFormatter, 0);
			}
		}

		public void Flush()
		{
			lock (MigrationLog.lockObj)
			{
				this.log.Flush();
			}
		}

		internal bool IsLoggingLevelEnabled(MigrationEventType level)
		{
			return this.loggingLevel >= level;
		}

		protected override void LogFailure(ADConfigurationLoader<Server, Server>.FailureLocation failureLocation, Exception exception)
		{
			this.Log("ConfigurationLoader", MigrationEventType.Error, MigrationLogContext.Current, "Failed to perform {0} due to exception {1}", new object[]
			{
				failureLocation,
				exception
			});
		}

		protected override void PreAdOperation(ref Server server)
		{
		}

		protected override void AdOperation(ref Server server)
		{
			server = this.configurationSession.FindLocalServer();
		}

		protected override void PostAdOperation(Server server, bool wasSuccessful)
		{
			if (wasSuccessful)
			{
				lock (MigrationLog.lockObj)
				{
					this.loggingLevel = server.MigrationLogLoggingLevel;
					string b = (null == server.MigrationLogFilePath) ? string.Empty : server.MigrationLogFilePath.ToString();
					if (this.log == null)
					{
						this.log = new Log("Migration_", new LogHeaderFormatter(MigrationLog.Schema), "Migration");
					}
					else if (string.Equals(this.logFilePath, b, StringComparison.OrdinalIgnoreCase) && this.maxAge == server.MigrationLogMaxAge && this.maxDirectorySize == server.MigrationLogMaxDirectorySize && this.maxFileSize == server.MigrationLogMaxFileSize)
					{
						return;
					}
					this.logFilePath = b;
					this.maxAge = server.MigrationLogMaxAge;
					this.maxDirectorySize = server.MigrationLogMaxDirectorySize;
					this.maxFileSize = server.MigrationLogMaxFileSize;
					this.log.Configure(string.IsNullOrEmpty(this.logFilePath) ? MigrationLog.DefaultLogPath : this.logFilePath, this.maxAge, (long)((double)this.maxDirectorySize), (long)((double)this.maxFileSize), false);
				}
			}
		}

		protected override void OnServerChangeCallback(ADNotificationEventArgs args)
		{
			base.ReadConfiguration();
		}

		private static readonly string[] Fields = new string[]
		{
			"timestamp",
			"thread-id",
			"source",
			"event-type",
			"context",
			"data"
		};

		private static readonly LogSchema Schema = new LogSchema("Microsoft Exchange Server", "15.00.1497.015", "MigrationLog", MigrationLog.Fields);

		private static readonly KeyValuePair<MigrationEventType, MigrationLog.Tracer>[] Tracers = new KeyValuePair<MigrationEventType, MigrationLog.Tracer>[]
		{
			new KeyValuePair<MigrationEventType, MigrationLog.Tracer>(MigrationEventType.Verbose, new MigrationLog.Tracer(ExTraceGlobals.ServiceletTracer.TraceDebug)),
			new KeyValuePair<MigrationEventType, MigrationLog.Tracer>(MigrationEventType.Information, new MigrationLog.Tracer(ExTraceGlobals.ServiceletTracer.Information)),
			new KeyValuePair<MigrationEventType, MigrationLog.Tracer>(MigrationEventType.Warning, new MigrationLog.Tracer(ExTraceGlobals.ServiceletTracer.TraceWarning)),
			new KeyValuePair<MigrationEventType, MigrationLog.Tracer>(MigrationEventType.Error, new MigrationLog.Tracer(ExTraceGlobals.ServiceletTracer.TraceError))
		};

		private static string defaultLogPath;

		private static object lockObj = new object();

		private readonly ITopologyConfigurationSession configurationSession;

		private Log log;

		private string logFilePath;

		private TimeSpan maxAge;

		private ByteQuantifiedSize maxDirectorySize;

		private ByteQuantifiedSize maxFileSize;

		private MigrationEventType loggingLevel;

		private delegate void Tracer(long id, string formatString, params object[] args);

		private enum Field
		{
			TimeStamp,
			ThreadID,
			Source,
			EventType,
			Context,
			Data
		}
	}
}
