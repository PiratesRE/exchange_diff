using System;
using System.Collections.Concurrent;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Net
{
	internal class AsyncLog
	{
		public AsyncLog(string filePrefix, LogHeaderFormatter headerFormatter, string logComponent)
		{
			this.backingLog = new Log(filePrefix, headerFormatter, logComponent);
			this.component = logComponent;
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, int bufferSize, TimeSpan streamFlushInterval, TimeSpan backgroundWriteInterval)
		{
			if (backgroundWriteInterval < AsyncLog.MinimumAsyncLogInterval)
			{
				throw new ArgumentException(string.Format("The backgroundWriteInterval must exceed {0}.", AsyncLog.MinimumAsyncLogInterval), "backgroundWriteInterval");
			}
			this.backingLog.Configure(path, maxAge, maxDirectorySize, maxLogFileSize, bufferSize, streamFlushInterval);
			if (this.logWriterTimer != null)
			{
				this.logWriterTimer.Change(streamFlushInterval, streamFlushInterval);
				return;
			}
			this.logWriterTimer = new GuardedTimer(delegate(object state)
			{
				this.WriteRequests(false);
			}, null, backgroundWriteInterval, backgroundWriteInterval);
			this.enabled = true;
		}

		public void Append(LogRowFormatter row, int timestampField)
		{
			if (!this.enabled)
			{
				throw new InvalidOperationException("The log must first be configured before it can be written to.");
			}
			if (this.flushRequired || this.pendingRequests.Count > 50000)
			{
				this.WriteRequests(true);
			}
			this.pendingRequests.Enqueue(new AsyncLog.AppendRequest
			{
				Row = new LogRowFormatter(row),
				TimeStampField = timestampField,
				TimeStamp = DateTime.UtcNow
			});
		}

		public void WriteRequests(bool forcedFlush = false)
		{
			try
			{
				lock (this.loggingLock)
				{
					if (forcedFlush && this.pendingRequests.Count > 50000)
					{
						Log.EventLog.LogEvent(CommonEventLogConstants.Tuple_PendingLoggingRequestsReachedMaximum, this.backingLog.LogDirectory.FullName, new object[]
						{
							this.component,
							50000
						});
						this.flushRequired = true;
					}
					AsyncLog.AppendRequest appendRequest;
					while (this.pendingRequests.TryDequeue(out appendRequest))
					{
						this.backingLog.Append(appendRequest.Row, appendRequest.TimeStampField, appendRequest.TimeStamp);
					}
					this.flushRequired = false;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public void Flush()
		{
			if (!this.enabled)
			{
				throw new InvalidOperationException("The log must first be configured before it can be flushed.");
			}
			this.WriteRequests(false);
			this.backingLog.Flush();
		}

		public void Close()
		{
			if (this.enabled)
			{
				this.logWriterTimer.Dispose(true);
				this.logWriterTimer = null;
				this.Flush();
				this.enabled = false;
			}
			this.backingLog.Close();
		}

		private const int MaximumQueueSize = 50000;

		private static readonly TimeSpan MinimumAsyncLogInterval = TimeSpan.FromMilliseconds(100.0);

		private readonly Log backingLog;

		private readonly string component;

		private ConcurrentQueue<AsyncLog.AppendRequest> pendingRequests = new ConcurrentQueue<AsyncLog.AppendRequest>();

		private GuardedTimer logWriterTimer;

		private bool enabled;

		private bool flushRequired;

		private object loggingLock = new object();

		private class AppendRequest
		{
			internal LogRowFormatter Row { get; set; }

			internal int TimeStampField { get; set; }

			internal DateTime TimeStamp { get; set; }
		}
	}
}
