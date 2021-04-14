using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.SyncMigrationServicelet;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AnchorLog
	{
		public AnchorLog(string applicationName, AnchorConfig config) : this(applicationName, config, ExTraceGlobals.ServiceletTracer)
		{
		}

		public AnchorLog(string applicationName, AnchorConfig config, Trace tracer)
		{
			this.tracers = new KeyValuePair<MigrationEventType, AnchorLog.Tracer>[]
			{
				new KeyValuePair<MigrationEventType, AnchorLog.Tracer>(MigrationEventType.Verbose, new AnchorLog.Tracer(tracer.TraceDebug)),
				new KeyValuePair<MigrationEventType, AnchorLog.Tracer>(MigrationEventType.Information, new AnchorLog.Tracer(tracer.Information)),
				new KeyValuePair<MigrationEventType, AnchorLog.Tracer>(MigrationEventType.Warning, new AnchorLog.Tracer(tracer.TraceWarning)),
				new KeyValuePair<MigrationEventType, AnchorLog.Tracer>(MigrationEventType.Error, new AnchorLog.Tracer(tracer.TraceError))
			};
			this.ApplicationName = applicationName;
			this.Config = config;
			this.Update();
		}

		private MigrationEventType LoggingLevel { get; set; }

		private string ApplicationName { get; set; }

		private AnchorConfig Config { get; set; }

		private string DefaultLogPath
		{
			get
			{
				if (this.defaultLogPath == null)
				{
					string text = ExchangeSetupContext.InstallPath;
					if (text == null)
					{
						text = Assembly.GetExecutingAssembly().Location;
						text = Path.GetDirectoryName(text);
					}
					this.defaultLogPath = Path.Combine(text, string.Format("logging\\{0}Logs", this.ApplicationName));
				}
				return this.defaultLogPath;
			}
		}

		private string LogFilePath { get; set; }

		private TimeSpan MaxAge { get; set; }

		private ByteQuantifiedSize MaxDirectorySize { get; set; }

		private ByteQuantifiedSize MaxFileSize { get; set; }

		public void Close()
		{
			lock (this.lockObj)
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
			foreach (KeyValuePair<MigrationEventType, AnchorLog.Tracer> keyValuePair in this.tracers)
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
			if (this.log == null || this.LoggingLevel < eventType)
			{
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(AnchorLog.Schema);
			logRowFormatter[1] = Environment.CurrentManagedThreadId;
			logRowFormatter[3] = eventType;
			logRowFormatter[2] = source;
			logRowFormatter[4] = context.ToString();
			if (args != null && args.Length > 0)
			{
				logRowFormatter[5] = string.Format(CultureInfo.InvariantCulture, format, args);
			}
			else
			{
				logRowFormatter[5] = format;
			}
			lock (this.lockObj)
			{
				this.log.Append(logRowFormatter, 0);
			}
		}

		protected virtual void Update()
		{
			MigrationEventType config = this.Config.GetConfig<MigrationEventType>("LoggingLevel");
			string config2 = this.Config.GetConfig<string>("LogFilePath");
			TimeSpan config3 = this.Config.GetConfig<TimeSpan>("LogMaxAge");
			ByteQuantifiedSize maxDirectorySize = new ByteQuantifiedSize(this.Config.GetConfig<ulong>("LogMaxDirectorySize"));
			ByteQuantifiedSize maxFileSize = new ByteQuantifiedSize(this.Config.GetConfig<ulong>("LogMaxFileSize"));
			this.Update(config, config2, config3, maxDirectorySize, maxFileSize);
		}

		protected void Update(MigrationEventType loggingLevel, string logFilePath, TimeSpan maxAge, ByteQuantifiedSize maxDirectorySize, ByteQuantifiedSize maxFileSize)
		{
			lock (this.lockObj)
			{
				this.LoggingLevel = loggingLevel;
				if (this.log == null)
				{
					this.log = new Log(string.Format("{0}_", this.ApplicationName), new LogHeaderFormatter(AnchorLog.Schema), this.ApplicationName);
				}
				else if (string.Equals(this.LogFilePath, logFilePath, StringComparison.OrdinalIgnoreCase) && this.MaxAge == maxAge && this.MaxDirectorySize == maxDirectorySize && this.MaxFileSize == maxFileSize)
				{
					return;
				}
				this.LogFilePath = logFilePath;
				this.MaxAge = maxAge;
				this.MaxDirectorySize = maxDirectorySize;
				this.MaxFileSize = maxFileSize;
				this.log.Configure(string.IsNullOrEmpty(this.LogFilePath) ? this.DefaultLogPath : this.LogFilePath, this.MaxAge, (long)((double)this.MaxDirectorySize), (long)((double)this.MaxFileSize), false);
			}
		}

		internal const string DefaultSeparator = ",";

		private static readonly string[] Fields = new string[]
		{
			"timestamp",
			"thread-id",
			"source",
			"event-type",
			"context",
			"data"
		};

		private static readonly LogSchema Schema = new LogSchema("Microsoft Exchange Server", "15.00.1497.015", "AnchorLog", AnchorLog.Fields);

		private readonly KeyValuePair<MigrationEventType, AnchorLog.Tracer>[] tracers;

		private string defaultLogPath;

		private object lockObj = new object();

		private Log log;

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
