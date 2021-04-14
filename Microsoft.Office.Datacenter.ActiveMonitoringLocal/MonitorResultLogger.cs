using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class MonitorResultLogger : DisposeTrackableBase
	{
		internal MonitorResultLogger(ILogConfiguration configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			this.configuration = configuration;
			if (this.configuration.IsLoggingEnabled)
			{
				string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
				this.logSchema = new LogSchema("Microsoft Exchange Server", version, this.configuration.LogType, MonitorResultLogger.Fields);
				this.log = new Log(this.configuration.LogPrefix, new LogHeaderFormatter(this.logSchema), this.configuration.LogComponent);
				this.log.Configure(this.configuration.LogPath, this.configuration.MaxLogAge, this.configuration.MaxLogDirectorySizeInBytes, this.configuration.MaxLogFileSizeInBytes, Settings.ResultsLogBufferSizeInBytes, new TimeSpan(0, Settings.ResultsLogFlushIntervalInMinutes, 0));
			}
		}

		public void LogEvent(DateTime timestamp, Dictionary<string, object> tempResults)
		{
			if (!this.configuration.IsLoggingEnabled)
			{
				return;
			}
			LogRowFormatter row = this.CreateRow(timestamp, CrimsonHelper.Serialize(tempResults, true));
			this.log.Append(row, -1);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MonitorResultLogger>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.log != null)
			{
				this.log.Flush();
				this.log.Close();
			}
		}

		private LogRowFormatter CreateRow(DateTime time, string data)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema, true);
			logRowFormatter[0] = time;
			logRowFormatter[1] = data;
			return logRowFormatter;
		}

		private const string SoftwareName = "Microsoft Exchange Server";

		private static readonly string[] Fields = new string[]
		{
			"TimeStamp",
			"Data"
		};

		private readonly LogSchema logSchema;

		private readonly Log log;

		private readonly ILogConfiguration configuration;

		private enum Field
		{
			TimeStamp,
			Data
		}
	}
}
