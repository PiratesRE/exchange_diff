using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public class Log : ILogWriter
	{
		public Log(string fileNamePrefix, LogHeaderFormatter headerFormatter, string logComponent)
		{
			this.logImpl = new Log(fileNamePrefix, headerFormatter.LogHeaderFormatterImpl, logComponent);
		}

		public Log(string fileNamePrefix, LogHeaderFormatter headerFormatter, string logComponent, bool handleKnownExceptions)
		{
			this.logImpl = new Log(fileNamePrefix, headerFormatter.LogHeaderFormatterImpl, logComponent, handleKnownExceptions);
		}

		public bool TestHelper_ForceLogFileRollOver
		{
			get
			{
				return this.logImpl.TestHelper_ForceLogFileRollOver;
			}
			set
			{
				this.logImpl.TestHelper_ForceLogFileRollOver = value;
			}
		}

		public static DirectoryInfo CreateLogDirectory(string path)
		{
			return Log.CreateLogDirectory(path);
		}

		public void Flush()
		{
			this.logImpl.Flush();
		}

		public void Close()
		{
			this.logImpl.Close();
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize)
		{
			this.logImpl.Configure(path, maxAge, maxDirectorySize, maxLogFileSize);
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, int bufferSize, TimeSpan streamFlushInterval)
		{
			this.logImpl.Configure(path, maxAge, maxDirectorySize, maxLogFileSize, bufferSize, streamFlushInterval);
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, int bufferSize, TimeSpan streamFlushInterval, string note)
		{
			this.logImpl.Configure(path, maxAge, maxDirectorySize, maxLogFileSize, bufferSize, streamFlushInterval, note);
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, int bufferSize, TimeSpan streamFlushInterval, string note, bool flushToDisk)
		{
			this.logImpl.Configure(path, maxAge, maxDirectorySize, maxLogFileSize, bufferSize, streamFlushInterval, note, flushToDisk);
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, int bufferSize, TimeSpan streamFlushInterval, bool flushToDisk)
		{
			this.logImpl.Configure(path, maxAge, maxDirectorySize, maxLogFileSize, bufferSize, streamFlushInterval, flushToDisk);
		}

		public void Configure(string path, LogFileRollOver logFileRollOver)
		{
			this.logImpl.Configure(path, (LogFileRollOver)logFileRollOver);
		}

		public void Configure(string path, LogFileRollOver logFileRollOver, int bufferSize, TimeSpan streamFlushInterval)
		{
			this.logImpl.Configure(path, (LogFileRollOver)logFileRollOver, bufferSize, streamFlushInterval);
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, bool enforceAccurateAge)
		{
			this.logImpl.Configure(path, maxAge, maxDirectorySize, maxLogFileSize, enforceAccurateAge);
		}

		public void Configure(string path, LogFileRollOver logFileRollOver, TimeSpan maxAge)
		{
			this.logImpl.Configure(path, (LogFileRollOver)logFileRollOver, maxAge);
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, bool enforceAccurateAge, int bufferSize, TimeSpan streamFlushInterval)
		{
			this.logImpl.Configure(path, maxAge, maxDirectorySize, maxLogFileSize, enforceAccurateAge, bufferSize, streamFlushInterval);
		}

		public void Configure(string path, TimeSpan maxAge, long maxDirectorySize, long maxLogFileSize, bool enforceAccurateAge, int bufferSize, TimeSpan streamFlushInterval, LogFileRollOver logFileRollOver)
		{
			this.logImpl.Configure(path, maxAge, maxDirectorySize, maxLogFileSize, enforceAccurateAge, bufferSize, streamFlushInterval, (LogFileRollOver)logFileRollOver);
		}

		public void Append(IEnumerable<LogRowFormatter> rows, int timestampField)
		{
			ArgumentValidator.ThrowIfNull("rows", rows);
			List<LogRowFormatter> list = new List<LogRowFormatter>();
			foreach (LogRowFormatter logRowFormatter in rows)
			{
				list.Add(logRowFormatter.LogRowFormatterImpl);
			}
			this.logImpl.Append(list, timestampField);
		}

		public void Append(LogRowFormatter row, int timestampField)
		{
			this.logImpl.Append(row.LogRowFormatterImpl, timestampField, DateTime.MinValue);
		}

		public void Append(LogRowFormatter row, int timestampField, DateTime timeStamp)
		{
			this.logImpl.Append(row.LogRowFormatterImpl, timestampField, timeStamp);
		}

		private Log logImpl;
	}
}
