using System;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class StateLockRemote
	{
		public StateLockRemote(string prefix, string dbName, IStateIO stateIO)
		{
			this.m_stateIO = stateIO;
			this.m_prefix = prefix;
			this.m_databaseName = dbName;
		}

		public void EnterSuspend()
		{
			this.SuspendWanted = true;
			if (!this.SuspendWanted)
			{
				ExTraceGlobals.StateLockTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: EnterSuspend(): Error occurred while setting SuspendWanted flag.", this.m_databaseName);
				throw new SuspendWantedWriteFailedException();
			}
		}

		public bool TryLeaveSuspend()
		{
			this.SuspendWanted = false;
			return !this.SuspendWanted;
		}

		internal bool SuspendWanted
		{
			get
			{
				bool result;
				this.m_stateIO.TryReadBool(string.Format("{0}SuspendWanted", this.m_prefix), false, out result);
				return result;
			}
			private set
			{
				this.m_stateIO.WriteBool(string.Format("{0}SuspendWanted", this.m_prefix), value, true);
			}
		}

		private const string SuspendWantedFormat = "{0}SuspendWanted";

		private IStateIO m_stateIO;

		private string m_databaseName;

		private string m_prefix;
	}
}
