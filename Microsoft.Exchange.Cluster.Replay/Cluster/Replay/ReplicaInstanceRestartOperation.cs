using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplicaInstanceRestartOperation : ReplicaInstanceQueuedItem
	{
		public ReplicaInstanceRestartOperation(RestartInstanceWrapper instanceWrapper, ReplicaInstanceManager riManager) : base(instanceWrapper.OldReplicaInstance.ReplicaInstance)
		{
			base.IsDuplicateAllowed = false;
			this.m_instanceWrapper = instanceWrapper;
			this.m_replicaInstanceManager = riManager;
		}

		protected override void CheckOperationApplicable()
		{
			if (base.ReplicaInstance.CurrentContext.IsFailoverPending())
			{
				throw new ReplayServiceRestartInvalidDuringMoveException(base.DbCopyName);
			}
			if (base.ReplicaInstance.CurrentContext.Seeding)
			{
				throw new ReplayServiceRestartInvalidSeedingException(base.DbCopyName);
			}
		}

		protected override void ExecuteInternal()
		{
			base.ExecuteInternal();
			this.m_replicaInstanceManager.RestartInstance(this.m_instanceWrapper);
		}

		private ReplicaInstanceManager m_replicaInstanceManager;

		private RestartInstanceWrapper m_instanceWrapper;
	}
}
