using System;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplicaInstanceAttemptCopyLastLogs : ReplicaInstanceAmQueuedItem
	{
		public ReplicaInstanceAttemptCopyLastLogs(AmAcllArgs acllArgs, AcllPerformanceTracker acllPerf, ReplicaInstance instance) : base(instance)
		{
			this.AcllArgs = acllArgs;
			this.AcllPerfTracker = acllPerf;
		}

		internal AmAcllArgs AcllArgs { get; private set; }

		internal AcllPerformanceTracker AcllPerfTracker { get; private set; }

		internal AmAcllReturnStatus AcllStatus { get; private set; }

		protected override Exception GetReplicaInstanceNotFoundException()
		{
			return new AmDbAcllErrorNoReplicaInstance(base.DbName, Environment.MachineName);
		}

		protected override void CheckOperationApplicable()
		{
			if (base.ReplicaInstance.CurrentContext.IsFailoverPending())
			{
				throw new AcllAlreadyRunningException(base.DbCopyName);
			}
		}

		protected override void ExecuteInternal()
		{
			base.ExecuteInternal();
			AmAcllReturnStatus acllStatus = base.ReplicaInstance.AttemptCopyLastLogsRcr(this.AcllArgs, this.AcllPerfTracker);
			this.AcllStatus = acllStatus;
		}
	}
}
