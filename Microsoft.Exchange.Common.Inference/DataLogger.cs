using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Inference.Common.Diagnostics
{
	public class DataLogger : DataLoggerBase
	{
		public DataLogger(ILogConfig logConfig, IList<string> columnNames, IList<Type> columnTypes) : base(new List<string>
		{
			"LogSessionId",
			"LogSequenceNumber"
		}.Concat(columnNames).ToList<string>(), new List<Type>
		{
			typeof(string),
			typeof(string)
		}.Concat(columnTypes).ToList<Type>())
		{
			this.Initialize(logConfig);
		}

		public override string[] RecentlyLoggedRows
		{
			get
			{
				return this.FileLog.RecentlyLoggedRows;
			}
		}

		public ILogConfig LogConfig
		{
			get
			{
				return this.config;
			}
		}

		public Guid CurrentLogSessionId
		{
			get
			{
				return this.logSessionId;
			}
		}

		protected LogWrapper FileLog
		{
			get
			{
				if (this.FileLogInstance == null)
				{
					this.FileLogInstance = new LogWrapper(this.LogConfig, base.ColumnNames.ToArray<string>());
				}
				return this.FileLogInstance;
			}
		}

		protected LogWrapper FileLogInstance { get; set; }

		public override void Log(IList<object> values)
		{
			if (!this.LogConfig.IsLoggingEnabled)
			{
				return;
			}
			if (this.logSequenceNumber == this.LogConfig.LogSessionLineCount - 1)
			{
				lock (this.logSessionResetLockObject)
				{
					if (this.logSequenceNumber == this.LogConfig.LogSessionLineCount - 1)
					{
						this.ResetLogSession();
					}
				}
			}
			List<object> first = new List<object>
			{
				this.logSessionId,
				Interlocked.Increment(ref this.logSequenceNumber)
			};
			this.FileLog.Append(first.Concat(values).ToList<object>());
		}

		public override void Flush()
		{
		}

		public override void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public string[] GetRecentlyLoggedRows()
		{
			return this.FileLog.RecentlyLoggedRows;
		}

		internal void Initialize(ILogConfig logConfig)
		{
			ArgumentValidator.ThrowIfNull("logConfig", logConfig);
			this.config = logConfig;
			this.logSessionResetLockObject = new object();
			this.ResetLogSession();
			this.ReleaseLogInstance();
		}

		protected void ReleaseLogInstance()
		{
			if (this.FileLogInstance != null)
			{
				this.FileLogInstance.Dispose();
				this.FileLogInstance = null;
			}
		}

		private void ResetLogSession()
		{
			this.logSessionId = Guid.NewGuid();
			this.logSequenceNumber = -1;
		}

		private void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing)
				{
					this.ReleaseLogInstance();
				}
				this.isDisposed = true;
			}
		}

		private ILogConfig config;

		private bool isDisposed;

		private Guid logSessionId;

		private int logSequenceNumber;

		private object logSessionResetLockObject;
	}
}
