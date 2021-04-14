using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplicaInstancePreMountCallback : ReplicaInstanceAmQueuedItem
	{
		public ReplicaInstancePreMountCallback(int storeMountFlags, AmMountFlags amMountFlags, MountDirectPerformanceTracker mountPerf, ReplicaInstance instance) : base(instance)
		{
			this.StoreMountFlags = storeMountFlags;
			this.AmMountFlags = amMountFlags;
			this.MountPerfTracker = mountPerf;
		}

		internal int StoreMountFlags
		{
			get
			{
				return this.m_storeMountFlags;
			}
			set
			{
				this.m_storeMountFlags = value;
			}
		}

		private AmMountFlags AmMountFlags { get; set; }

		internal MountDirectPerformanceTracker MountPerfTracker { get; private set; }

		internal ReplayQueuedItemBase RestartOperation { get; private set; }

		internal LogStreamResetOnMount LogReset { get; private set; }

		protected override Exception GetReplicaInstanceNotFoundException()
		{
			return new AmPreMountCallbackFailedNoReplicaInstanceException(base.DbName, Environment.MachineName);
		}

		protected override void CheckOperationApplicable()
		{
		}

		protected override void ExecuteInternal()
		{
			base.ExecuteInternal();
			LogStreamResetOnMount logReset;
			ReplayQueuedItemBase restartOperation = base.ReplicaInstance.AmPreMountCallback(base.DbGuid, ref this.m_storeMountFlags, this.AmMountFlags, this.MountPerfTracker, out logReset);
			this.RestartOperation = restartOperation;
			this.LogReset = logReset;
		}

		private int m_storeMountFlags;
	}
}
