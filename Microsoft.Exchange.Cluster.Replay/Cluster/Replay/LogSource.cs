using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.EseRepl;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class LogSource
	{
		internal static LogSource Construct(IReplayConfiguration config, IPerfmonCounters perfmonCounters, NetworkPath initialNetworkPath, int timeoutMs)
		{
			return new LogCopyClient(config, perfmonCounters, initialNetworkPath, timeoutMs);
		}

		internal static int GetLogShipTimeoutInMsec(bool runningAcll)
		{
			if (!runningAcll)
			{
				return RegistryParameters.LogShipTimeoutInMsec;
			}
			return RegistryParameters.LogShipACLLTimeoutInMsec;
		}

		public virtual void SetTimeoutInMsec(int timeoutInMs)
		{
			this.m_defaultTimeoutInMs = timeoutInMs;
		}

		internal void RecordThruput(long byteCount)
		{
			if (this.m_perfmonCounters != null)
			{
				this.m_perfmonCounters.RecordLogCopyThruput(byteCount);
			}
		}

		public long CachedEndOfLog
		{
			get
			{
				return this.m_endOfLog.Generation;
			}
		}

		public DateTime CachedEndOfLogWriteTimeUtc
		{
			get
			{
				return this.m_endOfLog.Utc;
			}
		}

		public bool IsLogInRange(long prospect)
		{
			return prospect <= this.m_endOfLog.Generation;
		}

		public virtual string SourcePath
		{
			get
			{
				return null;
			}
		}

		public int DefaultTimeoutInMs
		{
			get
			{
				return this.m_defaultTimeoutInMs;
			}
		}

		public abstract void Cancel();

		public abstract void Close();

		public abstract long QueryLogRange();

		public abstract long QueryEndOfLog();

		public abstract void CopyLog(long logNum, string toFile, out DateTime writeTimeUtc);

		public void CopyLog(long logNum, string toFile)
		{
			DateTime dateTime;
			this.CopyLog(logNum, toFile, out dateTime);
		}

		public abstract long GetE00Generation();

		public abstract bool LogExists(long logNum);

		protected void AllocateBuffer()
		{
			lock (this)
			{
				if (this.m_buffer == null)
				{
					this.m_buffer = new byte[1048576];
				}
			}
		}

		public const int LogFileSize = 1048576;

		protected byte[] m_buffer;

		protected IReplayConfiguration m_config;

		protected IPerfmonCounters m_perfmonCounters;

		protected bool m_cancelling;

		protected EndOfLog m_endOfLog = new EndOfLog();

		protected int m_defaultTimeoutInMs;
	}
}
