using System;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal sealed class AsyncQueueLogWriter : IQueueLogWriter
	{
		public AsyncQueueLogWriter(string software, string logName, string logComponentName, string logFileNamePrefix, string logDirectory, TimeSpan maxAge, TimeSpan streamFlushInterval, TimeSpan backgroundWriteInterval, long maxDirectorySize = 0L, long maxLogFileSize = 0L, int bufferSize = 0)
		{
			ArgumentValidator.ThrowIfNull("software", software);
			ArgumentValidator.ThrowIfNull("logName", logName);
			ArgumentValidator.ThrowIfNull("logComponentName", logComponentName);
			ArgumentValidator.ThrowIfNull("logFileNamePrefix", logFileNamePrefix);
			ArgumentValidator.ThrowIfNull("logDirectory", logDirectory);
			ArgumentValidator.ThrowIfInvalidValue<TimeSpan>("maxAge", maxAge, (TimeSpan interval) => interval > TimeSpan.Zero);
			ArgumentValidator.ThrowIfInvalidValue<TimeSpan>("streamFlushInterval", streamFlushInterval, (TimeSpan interval) => interval > TimeSpan.Zero);
			ArgumentValidator.ThrowIfInvalidValue<TimeSpan>("backgroundWriteInterval", backgroundWriteInterval, (TimeSpan interval) => interval > TimeSpan.Zero);
			ArgumentValidator.ThrowIfInvalidValue<long>("maxDirectorySize", maxDirectorySize, (long number) => number >= 0L);
			ArgumentValidator.ThrowIfInvalidValue<long>("maxLogFileSize", maxLogFileSize, (long number) => number >= 0L);
			ArgumentValidator.ThrowIfNegative("bufferSize", bufferSize);
			this.logSchema = new LogSchema(software, Assembly.GetExecutingAssembly().GetName().Version.ToString(), logName, AsyncQueueLogWriter.CreateHeaders());
			this.asyncLog = new AsyncLog(logFileNamePrefix, new LogHeaderFormatter(this.logSchema), logComponentName);
			this.asyncLog.Configure(logDirectory, maxAge, maxDirectorySize, maxLogFileSize, bufferSize, streamFlushInterval, backgroundWriteInterval);
		}

		public void Write(QueueLogInfo logInfo)
		{
			ArgumentValidator.ThrowIfNull("logInfo", logInfo);
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema);
			logRowFormatter[0] = logInfo.TimeStamp;
			logRowFormatter[1] = logInfo.Display;
			logRowFormatter[2] = logInfo.Enqueues;
			logRowFormatter[3] = logInfo.Dequeues;
			logRowFormatter[4] = logInfo.Count;
			logRowFormatter[5] = logInfo.UsageData.ProcessingTicks;
			logRowFormatter[6] = logInfo.UsageData.MemoryUsed;
			logRowFormatter[7] = logInfo.UsageData.ProcessingTicks;
			this.asyncLog.Append(logRowFormatter, -1);
		}

		internal void Flush()
		{
			this.asyncLog.Flush();
		}

		private static string[] CreateHeaders()
		{
			string[] array = new string[Enum.GetValues(typeof(AsyncQueueLogWriter.Fields)).Length];
			array[0] = "Timestamp";
			array[1] = "Display";
			array[2] = "Enqueues";
			array[3] = "Dequeues";
			array[4] = "Count";
			array[5] = "TotalProcessingTicks";
			array[6] = "TotalMemoryUsed";
			array[7] = "TotalLockDuration";
			array[8] = "CustomData";
			return array;
		}

		private readonly LogSchema logSchema;

		private readonly AsyncLog asyncLog;

		internal enum Fields
		{
			Timestamp,
			Display,
			Enqueues,
			Dequeues,
			Count,
			TotalProcessingTicks,
			TotalMemoryUsed,
			TotalLockDuration,
			CustomData
		}
	}
}
