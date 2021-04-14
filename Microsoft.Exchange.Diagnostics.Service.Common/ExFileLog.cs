using System;
using System.Threading;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	internal class ExFileLog : ILog, IDisposable
	{
		public ExFileLog(string logDirectory)
		{
			string[] fields = new string[]
			{
				"date-time",
				"log-level",
				"thread-id",
				"message"
			};
			this.logSchema = new LogSchema("Microsoft Exchange Server", "15.0.0.0", "Diagnostics Service Log", fields);
			LogHeaderFormatter headerFormatter = new LogHeaderFormatter(this.logSchema, LogHeaderCsvOption.CsvStrict);
			this.exlog = new Log("DiagnosticsServiceLog", headerFormatter, "Diagnostics Service Log");
			this.exlog.Configure(logDirectory, TimeSpan.Zero, 104857600L, 10485760L, 1024, TimeSpan.FromSeconds(5.0), true);
		}

		public void Dispose()
		{
			if (this.exlog != null)
			{
				this.exlog.Close();
			}
		}

		public void LogInformationMessage(string format, params object[] args)
		{
			this.WriteLine("Information", format, args);
		}

		public void LogWarningMessage(string format, params object[] args)
		{
			this.WriteLine("Warning", format, args);
		}

		public void LogErrorMessage(string format, params object[] args)
		{
			this.WriteLine("Error", format, args);
		}

		private void WriteLine(string entryType, string format, params object[] args)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema);
			logRowFormatter[1] = entryType;
			logRowFormatter[2] = Thread.CurrentThread.ManagedThreadId;
			logRowFormatter[3] = CommonUtils.FoldIntoSingleLine(string.Format(format, args));
			this.exlog.Append(logRowFormatter, 0);
		}

		private const string ComponentName = "Diagnostics Service Log";

		private const string LogFilePrefix = "DiagnosticsServiceLog";

		private readonly Log exlog;

		private readonly LogSchema logSchema;

		private enum LogFields
		{
			DateTime,
			LogLevel,
			ThreadId,
			Message
		}
	}
}
