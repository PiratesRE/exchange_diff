using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.Diagnostics
{
	internal sealed class DiagnosticsLog
	{
		internal DiagnosticsLog(IDiagnosticsLogConfig diagnosticsLogConfig, string[] columns)
		{
			Util.ThrowOnNullArgument(diagnosticsLogConfig, "diagnosticsLogConfig");
			this.diagnosticsLogConfig = diagnosticsLogConfig;
			this.columns = columns;
			this.IsEnabled = diagnosticsLogConfig.IsEnabled;
			if (this.IsEnabled)
			{
				this.Schema = new LogSchema("Microsoft Exchange", "15.00.1497.012", this.diagnosticsLogConfig.LogTypeName, columns);
				if (!DiagnosticsLog.logSingletons.TryGetValue(this.diagnosticsLogConfig.LogTypeName, out this.log))
				{
					lock (DiagnosticsLog.lockObject)
					{
						if (!DiagnosticsLog.logSingletons.TryGetValue(this.diagnosticsLogConfig.LogTypeName, out this.log))
						{
							this.log = new Log(this.diagnosticsLogConfig.LogFilePrefix, new LogHeaderFormatter(this.Schema), this.diagnosticsLogConfig.LogComponent);
							this.log.Configure(this.diagnosticsLogConfig.LogFilePath, TimeSpan.FromHours((double)diagnosticsLogConfig.MaxAge), (long)(this.diagnosticsLogConfig.MaxDirectorySize * 1024), (long)(this.diagnosticsLogConfig.MaxFileSize * 1024));
							DiagnosticsLog.logSingletons.Add(this.diagnosticsLogConfig.LogTypeName, this.log);
						}
					}
				}
			}
		}

		internal static int CountOfLogInstances
		{
			get
			{
				return DiagnosticsLog.logSingletons.Count;
			}
		}

		internal Log Log
		{
			get
			{
				return this.log;
			}
		}

		internal LogSchema Schema { get; set; }

		internal bool IsEnabled { get; set; }

		internal IDiagnosticsLogConfig Config
		{
			get
			{
				return this.diagnosticsLogConfig;
			}
		}

		private static Breadcrumbs<string> ExtendedLoggingRows
		{
			get
			{
				if (DiagnosticsLog.extendedLoggingRows == null)
				{
					DiagnosticsLog.extendedLoggingRows = new Breadcrumbs<string>(16);
				}
				return DiagnosticsLog.extendedLoggingRows;
			}
		}

		public string[] GetFormattedExtendedLogging()
		{
			if (!this.diagnosticsLogConfig.IncludeExtendedLogging)
			{
				return null;
			}
			return DiagnosticsLog.ExtendedLoggingRows.BreadCrumb;
		}

		internal string Append(params object[] values)
		{
			if (this.columns.Length != values.Length)
			{
				throw new ArgumentException("The number of values to log does not match the number of columns names.");
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.Schema);
			int num = 0;
			foreach (object value in values)
			{
				logRowFormatter[num++] = value;
			}
			string text = string.Empty;
			if (this.diagnosticsLogConfig.IncludeExtendedLogging)
			{
				bool flag;
				text = ExDateTime.UtcNow + LogRowFormatter.FormatCollection(this.GetRowFormatterEnumerator(logRowFormatter), out flag);
				DiagnosticsLog.ExtendedLoggingRows.Drop(text);
			}
			this.log.Append(logRowFormatter, 0);
			return text;
		}

		private IEnumerable GetRowFormatterEnumerator(LogRowFormatter rowFormatter)
		{
			for (int i = 0; i < this.columns.Length; i++)
			{
				yield return rowFormatter[i];
			}
			yield break;
		}

		private static readonly object lockObject = new object();

		private static Dictionary<string, Log> logSingletons = new Dictionary<string, Log>();

		[ThreadStatic]
		private static Breadcrumbs<string> extendedLoggingRows;

		private IDiagnosticsLogConfig diagnosticsLogConfig;

		private string[] columns;

		private Log log;
	}
}
