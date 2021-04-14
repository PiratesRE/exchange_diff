using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AuditingOpticsLoggerInstance : IAuditingOpticsLoggerInstance
	{
		private Log Logger { get; set; }

		internal static IDisposable SetActivityIdTestHook(Guid actId)
		{
			AuditingOpticsLoggerInstance.hookableActivityId = Hookable<Guid>.Create(true, Guid.Empty);
			return AuditingOpticsLoggerInstance.hookableActivityId.SetTestHook(actId);
		}

		private string ServerName
		{
			get
			{
				Server localServer = LocalServerCache.LocalServer;
				if (localServer == null)
				{
					return string.Empty;
				}
				return localServer.Name;
			}
		}

		internal AuditingOpticsLoggerType LoggerType { get; private set; }

		public bool Enabled { get; private set; }

		private LogSchema LogSchema { get; set; }

		internal bool IsDebugTraceEnabled()
		{
			return this.Tracer != null && this.Tracer.IsTraceEnabled(TraceType.DebugTrace);
		}

		private Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AdminAuditLogTracer;
			}
		}

		private string LogComponentName
		{
			get
			{
				return AuditingOpticsConstants.LoggerComponentName;
			}
		}

		private string LogTypeName
		{
			get
			{
				return this.LoggerType.ToString() + AuditingOpticsConstants.AuditLoggerTypeName;
			}
		}

		private string FileNamePrefix
		{
			get
			{
				return this.LoggerType.ToString() + AuditingOpticsConstants.AuditLoggerFileNamePrefix;
			}
		}

		private int TimestampField
		{
			get
			{
				return 0;
			}
		}

		internal AuditingOpticsLoggerInstance(AuditingOpticsLoggerType loggerType)
		{
			EnumValidator.AssertValid<AuditingOpticsLoggerType>(loggerType);
			AuditingOpticsLoggerSettings auditingOpticsLoggerSettings = AuditingOpticsLoggerSettings.Load();
			if (auditingOpticsLoggerSettings.Enabled)
			{
				this.Enabled = true;
				this.LoggerType = loggerType;
				if (this.IsDebugTraceEnabled())
				{
					this.SafeTraceDebug(0L, "Start creating Auditing Optics log.", new object[0]);
				}
				this.LogSchema = new LogSchema(AuditingOpticsConstants.SoftwareName, "15.00.1497.012", this.LogTypeName, this.GetLogFields());
				LogHeaderFormatter headerFormatter = new LogHeaderFormatter(this.LogSchema);
				this.Logger = new Log(this.FileNamePrefix, headerFormatter, this.LogComponentName);
				if (this.IsDebugTraceEnabled())
				{
					this.SafeTraceDebug(0L, "Start configuring the Auditing Optics log.", new object[0]);
				}
				this.Logger.Configure(Path.Combine(auditingOpticsLoggerSettings.DirectoryPath, this.FileNamePrefix), auditingOpticsLoggerSettings.MaxAge, (long)auditingOpticsLoggerSettings.MaxDirectorySize.ToBytes(), (long)auditingOpticsLoggerSettings.MaxFileSize.ToBytes(), (int)auditingOpticsLoggerSettings.CacheSize.ToBytes(), auditingOpticsLoggerSettings.FlushInterval, auditingOpticsLoggerSettings.FlushToDisk);
				if (this.IsDebugTraceEnabled())
				{
					this.SafeTraceDebug(0L, "Auditing Optics log on server '{0}' is created and ready for use.", new object[]
					{
						this.ServerName
					});
					return;
				}
			}
			else
			{
				this.Enabled = false;
				if (this.IsDebugTraceEnabled())
				{
					this.SafeTraceDebug(0L, "The Auditing Optics log is disabled.", new object[0]);
				}
			}
		}

		private string[] GetLogFields()
		{
			return Enum.GetNames(typeof(AuditingOpticsLogFields));
		}

		private string GetActivityId()
		{
			if (AuditingOpticsLoggerInstance.hookableActivityId == null)
			{
				return Guid.NewGuid().ToString("D");
			}
			return AuditingOpticsLoggerInstance.hookableActivityId.Value.ToString("D");
		}

		internal void SafeTraceDebug(long id, string message, params object[] args)
		{
			if (this.Tracer != null)
			{
				this.Tracer.TraceDebug(id, message, args);
			}
		}

		internal void Stop()
		{
			if (this.Logger != null)
			{
				this.Logger.Close();
				this.Logger = null;
			}
		}

		public void InternalLogRow(List<KeyValuePair<string, object>> customData)
		{
			if (!this.Enabled)
			{
				if (this.IsDebugTraceEnabled())
				{
					this.SafeTraceDebug(0L, "AuditingOpticsLogger log is disabled, skip writing to the log file.", new object[0]);
				}
				return;
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.LogSchema);
			if (this.IsDebugTraceEnabled())
			{
				string text = string.Empty;
				if (customData != null)
				{
					bool flag;
					text = LogRowFormatter.FormatCollection(customData, out flag);
				}
				if (this.IsDebugTraceEnabled())
				{
					this.SafeTraceDebug(0L, "Start writing row to audit log: ServerName='{0}', CustomData='{1}'", new object[]
					{
						this.ServerName,
						text
					});
				}
			}
			logRowFormatter[1] = this.GetActivityId();
			logRowFormatter[2] = this.ServerName;
			logRowFormatter[3] = ApplicationName.Current.Name.ToLower();
			logRowFormatter[4] = ApplicationName.Current.ProcessId;
			logRowFormatter[5] = customData;
			this.Logger.Append(logRowFormatter, 0);
			if (this.IsDebugTraceEnabled())
			{
				this.SafeTraceDebug(0L, "The above row is written to the log successfully.", new object[0]);
			}
		}

		private static Hookable<Guid> hookableActivityId;
	}
}
