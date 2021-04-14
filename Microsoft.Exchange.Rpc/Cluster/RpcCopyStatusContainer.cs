using System;

namespace Microsoft.Exchange.Rpc.Cluster
{
	[Serializable]
	internal sealed class RpcCopyStatusContainer
	{
		public RpcDatabaseCopyStatus2[] CopyStatuses
		{
			get
			{
				return this.m_copyStatuses;
			}
			set
			{
				this.m_copyStatuses = value;
			}
		}

		public RpcHealthStateInfo[] HealthStates
		{
			get
			{
				return this.m_healthStates;
			}
			set
			{
				this.m_healthStates = value;
			}
		}

		private RpcDatabaseCopyStatus2[] m_copyStatuses;

		private RpcHealthStateInfo[] m_healthStates;
	}
}
