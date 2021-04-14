using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal abstract class ActivityContextLogger : DisposeTrackableBase, IWorkloadLogger
	{
		private protected Log Log { protected get; private set; }

		private protected bool Enabled { protected get; private set; }

		private protected LogSchema LogSchema { protected get; private set; }

		protected abstract string LogTypeName { get; }

		protected abstract string FileNamePrefix { get; }

		protected abstract string LogComponentName { get; }

		protected abstract Trace Tracer { get; }

		protected abstract int TimestampField { get; }

		protected virtual string ServerName
		{
			get
			{
				return ActivityContextLogger.serverName;
			}
		}

		private static Dictionary<ActivityEventType, string> CreateActivityEventTypeDictionary()
		{
			Dictionary<ActivityEventType, string> dictionary = new Dictionary<ActivityEventType, string>();
			foreach (object obj in Enum.GetValues(typeof(ActivityEventType)))
			{
				ActivityEventType activityEventType = (ActivityEventType)obj;
				dictionary.Add(activityEventType, activityEventType.ToString());
			}
			return dictionary;
		}

		protected ActivityContextLogger() : this(string.Empty)
		{
		}

		protected ActivityContextLogger(string fileNamePrefix)
		{
			this.SafeTraceDebug(0L, "Start creating {0}.", new object[]
			{
				this.LogTypeName
			});
			string version = "0.0.0.0";
			Version version2 = Assembly.GetExecutingAssembly().GetName().Version;
			if (version2 != null)
			{
				version = version2.ToString();
			}
			this.LogSchema = new LogSchema("Microsoft Exchange", version, this.LogTypeName, this.GetLogFields());
			LogHeaderFormatter headerFormatter = new LogHeaderFormatter(this.LogSchema);
			string fileNamePrefix2 = string.IsNullOrWhiteSpace(fileNamePrefix) ? this.FileNamePrefix : fileNamePrefix;
			this.Log = new Log(fileNamePrefix2, headerFormatter, this.LogComponentName);
			ActivityContextLogFileSettings logFileSettings = this.GetLogFileSettings();
			if (logFileSettings.Enabled)
			{
				this.Enabled = true;
				this.Log.Configure(logFileSettings.DirectoryPath, logFileSettings.MaxAge, (long)logFileSettings.MaxDirectorySize.ToBytes(), (long)logFileSettings.MaxFileSize.ToBytes(), (int)logFileSettings.CacheSize.ToBytes(), logFileSettings.FlushInterval, logFileSettings.FlushToDisk);
				this.InternalConfigure(logFileSettings);
				this.SafeTraceDebug(0L, "{0} is configured.", new object[]
				{
					this.LogTypeName
				});
			}
			else
			{
				this.Enabled = false;
				this.SafeTraceDebug(0L, "{0} is disabled.", new object[]
				{
					this.LogTypeName
				});
			}
			this.SafeTraceDebug(0L, "{0} on server {1} is created and ready for use.", new object[]
			{
				this.LogTypeName,
				this.ServerName
			});
		}

		public void LogActivityEvent(IActivityScope activityScope, ActivityEventType eventType)
		{
			if (activityScope == null)
			{
				throw new ArgumentNullException("activityScope");
			}
			this.InternalLogActivityEvent(activityScope, eventType);
		}

		protected void SafeTraceDebug(long id, string message, params object[] args)
		{
			if (this.Tracer != null)
			{
				this.Tracer.TraceDebug(id, message, args);
			}
		}

		protected void FlushLog()
		{
			this.SafeTraceDebug(0L, "Start flushing {0}.", new object[]
			{
				this.LogTypeName
			});
			this.Log.Flush();
			this.SafeTraceDebug(0L, "{0} is flushed.", new object[]
			{
				this.LogTypeName
			});
		}

		protected void AppendLog(LogRowFormatter row)
		{
			this.Log.Append(row, this.TimestampField);
			this.SafeTraceDebug(0L, "Row is appended for {0}.", new object[]
			{
				this.LogTypeName
			});
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ActivityContextLogger>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.SafeTraceDebug(0L, "Closing {0}.", new object[]
				{
					this.LogTypeName
				});
				if (this.Log != null)
				{
					this.Log.Close();
					this.Log = null;
				}
				this.SafeTraceDebug(0L, "{0} is closed.", new object[]
				{
					this.LogTypeName
				});
			}
		}

		protected abstract void InternalLogActivityEvent(IActivityScope activityScope, ActivityEventType eventType);

		protected abstract string[] GetLogFields();

		protected abstract ActivityContextLogFileSettings GetLogFileSettings();

		protected virtual void InternalConfigure(ActivityContextLogFileSettings settings)
		{
		}

		protected bool IsDebugTraceEnabled
		{
			get
			{
				return this.Tracer != null && this.Tracer.IsTraceEnabled(TraceType.DebugTrace);
			}
		}

		private const string DefaultAssemblyVersion = "0.0.0.0";

		private const string SoftwareName = "Microsoft Exchange";

		private static readonly string serverName = Environment.MachineName;

		protected static readonly Dictionary<ActivityEventType, string> ActivityEventTypeDictionary = ActivityContextLogger.CreateActivityEventTypeDictionary();
	}
}
