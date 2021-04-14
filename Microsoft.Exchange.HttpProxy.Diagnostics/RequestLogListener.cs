using System;
using System.Collections;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy
{
	internal class RequestLogListener
	{
		static RequestLogListener()
		{
			RequestLogListener.log = new Log(DiagnosticsConfiguration.LogFileNamePrefix.Value, new LogHeaderFormatter(RequestLogListener.logSchema, true), "LogComponent");
			RequestLogListener.log.Configure(DiagnosticsConfiguration.LogFolderPath.Value, TimeSpan.FromDays((double)DiagnosticsConfiguration.MaxLogRetentionInDays.Value), (long)(DiagnosticsConfiguration.MaxLogDirectorySizeInGB.Value * 1024 * 1024 * 1024), (long)(DiagnosticsConfiguration.MaxLogFileSizeInMB.Value * 1024 * 1024), true);
			ThreadPool.QueueUserWorkItem(new WaitCallback(RequestLogListener.CommitLogLines));
		}

		public static void AppendLog(LogData logData)
		{
			RequestLogListener.logQueue.Enqueue(logData);
			RequestLogListener.logCommitSignal.Set();
		}

		public static void FlushLogLines()
		{
			RequestLogListener.logCommitSignal.Set();
		}

		private static void CommitLog(LogData logData)
		{
			if (logData == null)
			{
				throw new ArgumentNullException("logData");
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(RequestLogListener.logSchema);
			foreach (LogKey logKey in LogData.LogKeys)
			{
				object obj = logData[logKey];
				if (obj != null)
				{
					int index = (int)logKey;
					logRowFormatter[index] = obj;
				}
			}
			RequestLogListener.log.Append(logRowFormatter, -1);
		}

		private static void CommitLogLines(object state)
		{
			for (;;)
			{
				if (RequestLogListener.logQueue.Count <= 0)
				{
					RequestLogListener.logCommitSignal.WaitOne();
				}
				else
				{
					LogData logData = RequestLogListener.logQueue.Dequeue() as LogData;
					RequestLogListener.CommitLog(logData);
				}
			}
		}

		private static Log log;

		private static Queue logQueue = Queue.Synchronized(new Queue());

		private static AutoResetEvent logCommitSignal = new AutoResetEvent(false);

		private static LogSchema logSchema = new LogSchema("Microsoft Exchange Server", "15.00.1497.015", "ProxyLogs", LogData.LogColumnNames);
	}
}
