using System;
using System.IO;
using System.Security;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class LogStreamResetOnMount
	{
		public LogStreamResetOnMount(IReplayConfiguration config)
		{
			this.m_config = config;
		}

		public bool ResetPending
		{
			get
			{
				return this.m_logResetPending;
			}
		}

		public void ResetLogStream()
		{
			this.m_logResetPending = true;
		}

		public void ConfirmLogReset()
		{
			if (this.m_logResetPending)
			{
				Exception ex = null;
				try
				{
					long num = ShipControl.HighestGenerationInDirectory(new DirectoryInfo(this.m_config.DestinationLogPath), this.m_config.LogFilePrefix, "." + this.m_config.LogExtension);
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, long>((long)this.GetHashCode(), "LogStreamResetOnMount: {0}: log stream reset confirmed at 0x{1:X}", this.m_config.Name, num);
					this.m_logResetPending = false;
					this.m_config.UpdateLastLogGeneratedAndEndOfLogInfo(num);
				}
				catch (IOException ex2)
				{
					ex = ex2;
				}
				catch (SecurityException ex3)
				{
					ex = ex3;
				}
				catch (TransientException ex4)
				{
					ex = ex4;
				}
				catch (AmServerException ex5)
				{
					ex = ex5;
				}
				if (ex != null)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, Exception>((long)this.GetHashCode(), "LogStreamResetOnMount: {0}: log stream reset confirmation failed with exception: {1}", this.m_config.Name, ex);
					return;
				}
			}
			else
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "LogStreamResetOnMount: {0}: normal mount: no log stream reset was pending", this.m_config.Name);
			}
		}

		public void CancelLogReset()
		{
			this.m_logResetPending = false;
			ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>((long)this.GetHashCode(), "LogStreamResetOnMount: {0}: mount failed, so no log stream reset was possible", this.m_config.Name);
		}

		private IReplayConfiguration m_config;

		private bool m_logResetPending;
	}
}
