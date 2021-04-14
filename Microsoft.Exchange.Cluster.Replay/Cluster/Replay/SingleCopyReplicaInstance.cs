using System;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class SingleCopyReplicaInstance : ReplicaInstance
	{
		public SingleCopyReplicaInstance(ReplayConfiguration replayConfiguration, IPerfmonCounters perfCounters) : base(replayConfiguration, false, null, perfCounters)
		{
			ReplicaInstance.DisposeIfActionUnsuccessful(delegate
			{
			}, this);
		}

		protected override bool ShouldAcquireSuspendLockInConfigChecker
		{
			get
			{
				return false;
			}
		}

		internal override bool GetSignatureAndCheckpoint(out JET_SIGNATURE? logfileSignature, out long lowestGenerationRequired, out long highestGenerationRequired, out long lastGenerationBackedUp)
		{
			lowestGenerationRequired = 0L;
			highestGenerationRequired = 0L;
			lastGenerationBackedUp = 0L;
			logfileSignature = base.FileChecker.FileState.LogfileSignature;
			if (logfileSignature == null)
			{
				base.FileChecker.TryUpdateActiveDatabaseLogfileSignature();
				logfileSignature = base.FileChecker.FileState.LogfileSignature;
			}
			return logfileSignature != null;
		}

		internal override AmAcllReturnStatus AttemptCopyLastLogsRcr(AmAcllArgs acllArgs, AcllPerformanceTracker acllPerf)
		{
			throw new AcllInvalidForSingleCopyException(base.Configuration.DisplayName);
		}

		internal override void RequestSuspend(string suspendComment, DatabaseCopyActionFlags flags, ActionInitiatorType initiator)
		{
			throw new ReplayServiceSuspendRpcInvalidForSingleCopyException(base.Configuration.DisplayName);
		}

		internal override void RequestResume(DatabaseCopyActionFlags flags)
		{
			throw new ReplayServiceResumeRpcInvalidForSingleCopyException(base.Configuration.DisplayName);
		}

		protected override bool ConfigurationCheckerInternal()
		{
			base.FileChecker.TryUpdateActiveDatabaseLogfileSignature();
			return true;
		}

		protected override void StartComponents()
		{
		}

		protected override void PrepareToStopInternal()
		{
		}

		protected override void StopInternal()
		{
		}
	}
}
