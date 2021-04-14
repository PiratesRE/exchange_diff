using System;

namespace Microsoft.Exchange.Data.Transport.Internal.MExRuntime
{
	internal sealed class MonitoringOptions
	{
		public int AgentExecutionLimitInMilliseconds
		{
			get
			{
				return this.agentExecutionLimitInMilliseconds;
			}
			set
			{
				this.agentExecutionLimitInMilliseconds = value;
			}
		}

		public bool MessageSnapshotEnabled
		{
			get
			{
				return this.messageSnapshotEnabled;
			}
			set
			{
				this.messageSnapshotEnabled = value;
			}
		}

		private int agentExecutionLimitInMilliseconds = 90000;

		private bool messageSnapshotEnabled = true;
	}
}
