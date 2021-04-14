using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Inference.Common.Diagnostics
{
	public class LogWrapper : DisposeTrackableBase
	{
		public LogWrapper(ILogConfig config, string[] columnNames)
		{
			ArgumentValidator.ThrowIfNull("config", config);
			ArgumentValidator.ThrowIfNull("config.SoftwareName", config.SoftwareName);
			ArgumentValidator.ThrowIfNull("config.SoftwareVersion", config.SoftwareVersion);
			ArgumentValidator.ThrowIfNull("config.ComponentName", config.ComponentName);
			ArgumentValidator.ThrowIfNull("config.LogType", config.LogType);
			ArgumentValidator.ThrowIfNull("config.LogPrefix", config.LogPrefix);
			ArgumentValidator.ThrowIfNull("config.LogPath", config.LogPath);
			ArgumentValidator.ThrowIfNull("columnNames", columnNames);
			this.inMemoryLogs = new Breadcrumbs<string>(64);
			this.logSchema = new LogSchema(config.SoftwareName, config.SoftwareVersion, config.LogType, columnNames);
			this.log = new Log(config.LogPrefix, new LogHeaderFormatter(this.logSchema), config.ComponentName);
			this.log.Configure(config.LogPath, config.MaxLogAge, (long)config.MaxLogDirectorySize, (long)config.MaxLogFileSize);
			this.isEnabled = config.IsLoggingEnabled;
		}

		public string[] RecentlyLoggedRows
		{
			get
			{
				return this.inMemoryLogs.BreadCrumb;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return this.isEnabled;
			}
		}

		public void Append(IList<object> values)
		{
			if (!this.isEnabled)
			{
				return;
			}
			if (values == null)
			{
				return;
			}
			if (this.logSchema.Fields.Length != values.Count)
			{
				throw new ArgumentException("The number of values to log does not match the number of columns names.");
			}
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema);
			int num = 0;
			foreach (object obj in values)
			{
				object value;
				if (obj is DateTime)
				{
					value = ((DateTime)obj).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffZ", DateTimeFormatInfo.InvariantInfo);
				}
				else if (obj is ExDateTime)
				{
					value = ((ExDateTime)obj).UniversalTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffZ", DateTimeFormatInfo.InvariantInfo);
				}
				else
				{
					value = obj;
				}
				logRowFormatter[num++] = value;
			}
			if (this.inMemoryLogs != null)
			{
				bool flag;
				string bc = LogRowFormatter.FormatCollection(this.GetRowFormatterEnumerator(logRowFormatter), out flag);
				this.inMemoryLogs.Drop(bc);
			}
			this.log.Append(logRowFormatter, -1);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<LogWrapper>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.log != null)
			{
				this.log.Close();
			}
		}

		private IEnumerable GetRowFormatterEnumerator(LogRowFormatter rowFormatter)
		{
			for (int i = 0; i < this.logSchema.Fields.Length; i++)
			{
				yield return rowFormatter[i];
			}
			yield break;
		}

		public const string TimestampFormat = "yyyy-MM-ddTHH\\:mm\\:ss.fffZ";

		private const int InMemoryLogSize = 64;

		private readonly Breadcrumbs<string> inMemoryLogs;

		private readonly LogSchema logSchema;

		private readonly Log log;

		private readonly bool isEnabled;
	}
}
