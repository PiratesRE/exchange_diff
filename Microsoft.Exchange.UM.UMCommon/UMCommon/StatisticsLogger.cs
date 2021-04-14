using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class StatisticsLogger : DisposableBase
	{
		protected abstract StatisticsLogger.StatisticsLogSchema LogSchema { get; }

		public void Init()
		{
			this.Init(AppConfig.Instance.Service.StatisticsLoggingEnabled, "UnifiedMessaging\\log");
		}

		public void Init(bool statisticsLoggingEnabled, string logDirectoryPath)
		{
			this.Init(VariantConfiguration.InvariantNoFlightingSnapshot.UM.AnonymizeLogging.Enabled, statisticsLoggingEnabled, Path.Combine(Utils.GetExchangeDirectory(), logDirectoryPath), AppConfig.Instance.Service.StatisticsLoggingMaxDirectorySize, AppConfig.Instance.Service.StatisticsLoggingMaxFileSize);
		}

		public void Init(bool anonymise, bool statisticsLoggingEnabled, string logDirectoryPath, int maxDirectorySize, int maxFileSize)
		{
			this.anonymise = anonymise;
			this.statisticsLoggingEnabled = statisticsLoggingEnabled;
			if (this.statisticsLoggingEnabled)
			{
				this.log = new Log(this.LogSchema.LogType + this.LogSchema.Version, new LogHeaderFormatter(this.LogSchema, true), "UnifiedMessaging");
				TimeSpan timeSpan = TimeSpan.FromDays(30.0);
				long num = (long)ByteQuantifiedSize.FromGB((ulong)((long)maxDirectorySize)).ToBytes();
				long num2 = (long)ByteQuantifiedSize.FromMB((ulong)((long)maxFileSize)).ToBytes();
				this.log.Configure(logDirectoryPath, timeSpan, num, num2);
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "Initialized log file. Path: '{0}'; Max age: '{1}'; Max dir size: '{2}'; Max file size: '{3}';", new object[]
				{
					logDirectoryPath,
					timeSpan,
					num,
					num2
				});
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "Statistics logging is disabled", new object[0]);
			}
			this.initialized = true;
		}

		public void Append(StatisticsLogger.StatisticsLogRow row)
		{
			base.CheckDisposed();
			ExAssert.RetailAssert(this.initialized, "Statistics logger has not been initialized yet");
			if (!this.statisticsLoggingEnabled)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "Statistics logging is disabled", new object[0]);
				return;
			}
			row.PopulateFields();
			using (StatisticsLogger.StatisticsLogRowFormatterGenerator statisticsLogRowFormatterGenerator = StatisticsLogger.StatisticsLogRowFormatterGenerator.Create(this.anonymise))
			{
				LogRowFormatter row2 = statisticsLogRowFormatterGenerator.CreateLogRowFormatter(row);
				try
				{
					this.log.Append(row2, -1);
				}
				catch (ObjectDisposedException ex)
				{
					string message = CallIdTracer.FormatMessage("Trying to log a row after disposing the logger. Exception: {0}", new object[]
					{
						ex
					});
					message = CallIdTracer.FormatMessageWithContextAndCallId(this, message);
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "Log file is not accessible because it has been closed, logging will be skipped. Exception: {0}.", new object[]
					{
						ex
					});
				}
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "Disposing statistics logger...", new object[0]);
				if (this.log != null)
				{
					this.log.Close();
					this.log = null;
				}
			}
		}

		private const string LogComponent = "UnifiedMessaging";

		private const int LogMaxAgeInDays = 30;

		private Log log;

		private bool initialized;

		private bool anonymise;

		private bool statisticsLoggingEnabled;

		public class StatisticsLogColumn
		{
			public StatisticsLogColumn(string name, bool anonymise)
			{
				this.name = name;
				this.anonymise = anonymise;
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public bool Anonymise
			{
				get
				{
					return this.anonymise;
				}
			}

			private readonly string name;

			private readonly bool anonymise;
		}

		public abstract class StatisticsLogSchema : LogSchema
		{
			public StatisticsLogSchema(string version, string logType, StatisticsLogger.StatisticsLogColumn[] columns) : base("Microsoft Exchange Server", version, logType, columns.ConvertAll((StatisticsLogger.StatisticsLogColumn column) => column.Name).ToArray())
			{
				this.columns = columns;
			}

			public bool ShouldAnonymise(int index)
			{
				return this.columns[index].Anonymise;
			}

			private const string StatisticsLogSoftware = "Microsoft Exchange Server";

			private readonly StatisticsLogger.StatisticsLogColumn[] columns;
		}

		public abstract class StatisticsLogRow
		{
			public StatisticsLogRow(StatisticsLogger.StatisticsLogSchema logSchema)
			{
				this.logSchema = logSchema;
				this.fields = new string[this.LogSchema.Fields.Length];
			}

			public StatisticsLogger.StatisticsLogSchema LogSchema
			{
				get
				{
					return this.logSchema;
				}
			}

			public string[] Fields
			{
				get
				{
					return this.fields;
				}
			}

			public abstract void PopulateFields();

			private readonly StatisticsLogger.StatisticsLogSchema logSchema;

			private readonly string[] fields;
		}

		private abstract class StatisticsLogRowFormatterGenerator : DisposableBase
		{
			public static StatisticsLogger.StatisticsLogRowFormatterGenerator Create(bool isDatacenter)
			{
				if (isDatacenter)
				{
					return new StatisticsLogger.DatacenterStatisticsLogRowFormatterGenerator();
				}
				return new StatisticsLogger.EnterpriseStatisticsLogRowFormatterGenerator();
			}

			public LogRowFormatter CreateLogRowFormatter(StatisticsLogger.StatisticsLogRow row)
			{
				LogRowFormatter logRowFormatter = new LogRowFormatter(row.LogSchema);
				for (int i = 0; i < row.Fields.Length; i++)
				{
					string text = row.Fields[i];
					logRowFormatter[i] = (row.LogSchema.ShouldAnonymise(i) ? this.Anonymise(text) : text);
				}
				return logRowFormatter;
			}

			protected abstract string Anonymise(string value);
		}

		private class EnterpriseStatisticsLogRowFormatterGenerator : StatisticsLogger.StatisticsLogRowFormatterGenerator
		{
			protected override string Anonymise(string value)
			{
				return value;
			}

			protected override void InternalDispose(bool disposing)
			{
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<StatisticsLogger.EnterpriseStatisticsLogRowFormatterGenerator>(this);
			}
		}

		private class DatacenterStatisticsLogRowFormatterGenerator : StatisticsLogger.StatisticsLogRowFormatterGenerator
		{
			public DatacenterStatisticsLogRowFormatterGenerator()
			{
				this.sha256 = new SHA256Cng();
			}

			protected override string Anonymise(string value)
			{
				if (string.IsNullOrEmpty(value))
				{
					return value;
				}
				byte[] inArray = this.sha256.ComputeHash(Encoding.Default.GetBytes(value));
				return Convert.ToBase64String(inArray);
			}

			protected override void InternalDispose(bool disposing)
			{
				if (disposing && this.sha256 != null)
				{
					this.sha256.Clear();
					this.sha256 = null;
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<StatisticsLogger.DatacenterStatisticsLogRowFormatterGenerator>(this);
			}

			private SHA256Cng sha256;
		}
	}
}
