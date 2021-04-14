using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal class AsyncLogWrapper : IAsyncLogWrapper
	{
		public AsyncLogWrapper(string logFileName, LogHeaderFormatter logHeaderFormatter, string logComponentName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("logFileName", logFileName);
			ArgumentValidator.ThrowIfNull("logHeaderFormatter", logHeaderFormatter);
			ArgumentValidator.ThrowIfNullOrEmpty("logComponentName", logComponentName);
			this.asyncLog = new AsyncLog(logFileName, logHeaderFormatter, logComponentName);
		}

		public void Append(LogRowFormatter row, int timestampField)
		{
			ArgumentValidator.ThrowIfNull("row", row);
			ArgumentValidator.ThrowIfNegative("timestampField", timestampField);
			try
			{
				this.asyncLog.Append(row, timestampField);
			}
			catch (ObjectDisposedException)
			{
			}
		}

		public void Configure(string logDirectory, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, int bufferSize, TimeSpan streamFlushInterval, TimeSpan backgroundWriteInterval)
		{
			this.asyncLog.Configure(logDirectory, maxAge, maxDirectorySize, maxLogFileSize, bufferSize, streamFlushInterval, backgroundWriteInterval);
		}

		private readonly AsyncLog asyncLog;
	}
}
