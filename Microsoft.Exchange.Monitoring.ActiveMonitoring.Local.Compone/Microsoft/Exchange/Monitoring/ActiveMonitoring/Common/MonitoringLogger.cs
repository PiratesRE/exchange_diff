using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class MonitoringLogger : DisposeTrackableBase
	{
		internal MonitoringLogger(ILogConfiguration configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			this.configuration = configuration;
			string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			this.logSchema = new LogSchema("Microsoft Exchange Server", version, this.configuration.LogType, MonitoringLogger.Fields);
			this.log = new Log(this.configuration.LogPrefix, new LogHeaderFormatter(this.logSchema), this.configuration.LogComponent);
			this.log.Configure(this.configuration.LogPath, this.configuration.MaxLogAge, this.configuration.MaxLogDirectorySizeInBytes, this.configuration.MaxLogFileSizeInBytes);
		}

		public void LogEvent(DateTime timestamp, string formatString, object[] parameters)
		{
			if (!this.configuration.IsLoggingEnabled)
			{
				return;
			}
			this.LogRow(timestamp, formatString, parameters);
		}

		public void LogEvents(List<DateTime> timestamps, List<string> formatStrings, List<object[]> parameters)
		{
			if (!this.configuration.IsLoggingEnabled)
			{
				return;
			}
			this.LogRows(timestamps, formatStrings, parameters);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MonitoringLogger>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.log.Flush();
				this.log.Close();
			}
		}

		private LogRowFormatter CreateRow(DateTime time, string data)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema, false);
			logRowFormatter[0] = time;
			logRowFormatter[1] = data;
			return logRowFormatter;
		}

		private void LogRow(DateTime time, string formatString, object[] parameters)
		{
			string data = formatString;
			if (parameters != null && parameters.Length > 0)
			{
				data = string.Format(formatString, parameters);
			}
			LogRowFormatter row = this.CreateRow(time, data);
			this.log.Append(row, -1);
		}

		private void LogRows(List<DateTime> timestamps, List<string> formatStrings, List<object[]> parameters)
		{
			List<LogRowFormatter> list = new List<LogRowFormatter>();
			for (int i = 0; i < timestamps.Count; i++)
			{
				string text = formatStrings[i];
				if (parameters[i] != null && parameters[i].Length > 0)
				{
					text = string.Format(text, parameters[i]);
				}
				list.Add(this.CreateRow(timestamps[i], text));
			}
			this.log.Append(list, -1);
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
