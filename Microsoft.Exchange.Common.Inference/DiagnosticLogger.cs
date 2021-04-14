using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Inference.Common.Diagnostics
{
	public class DiagnosticLogger : IDiagnosticLogger, IDisposable
	{
		public DiagnosticLogger(IDiagnosticLogConfig config) : this(config, null, 0)
		{
		}

		public DiagnosticLogger(IDiagnosticLogConfig config, Trace tracer, int traceId)
		{
			ArgumentValidator.ThrowIfNull("config", config);
			this.traceId = traceId;
			this.tracer = tracer;
			this.config = config;
			this.log = new LogWrapper(this.config, DiagnosticLogger.Columns);
		}

		internal string[] RecentlyLoggedRows
		{
			get
			{
				return this.log.RecentlyLoggedRows;
			}
		}

		public void LogError(string format, params object[] arguments)
		{
			if (this.tracer != null)
			{
				this.tracer.TraceError((long)this.traceId, format, arguments);
			}
			this.Log(LoggingLevel.Error, format, arguments);
		}

		public void LogInformation(string format, params object[] arguments)
		{
			if (this.tracer != null)
			{
				this.tracer.TraceInformation(0, (long)this.traceId, format, arguments);
			}
			this.Log(LoggingLevel.Information, format, arguments);
		}

		public void LogDebug(string format, params object[] arguments)
		{
			if (this.tracer != null)
			{
				this.tracer.TraceDebug((long)this.traceId, format, arguments);
			}
			this.Log(LoggingLevel.Debug, format, arguments);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Log(LoggingLevel loggingLevel, string format, params object[] arguments)
		{
			if (!this.config.IsLoggingEnabled)
			{
				return;
			}
			if (this.config.LoggingLevel < loggingLevel)
			{
				return;
			}
			IList<object> list = new List<object>(DiagnosticLogger.Columns.Length);
			string item = (arguments.Length != 0) ? new StringBuilder(format.Length).AppendFormat(format, arguments).ToString() : format;
			list.Add(DateTime.UtcNow);
			list.Add(loggingLevel);
			list.Add(item);
			this.log.Append(list);
		}

		private void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing && this.log != null)
				{
					this.log.Dispose();
				}
				this.isDisposed = true;
			}
		}

		internal static readonly string[] Columns = new string[]
		{
			"date-time",
			"level",
			"message"
		};

		private readonly IDiagnosticLogConfig config;

		private readonly LogWrapper log;

		private readonly Trace tracer;

		private readonly int traceId;

		private bool isDisposed;

		private enum LogField
		{
			Time,
			Level,
			Message
		}
	}
}
