using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IReplicaInstanceManager
	{
		bool TryWaitForFirstFullConfigUpdater();

		bool TryGetReplicaInstance(Guid guid, out ReplicaInstance instance);

		bool TryGetReplicaInstanceContainer(Guid guid, out ReplicaInstanceContainer instance);

		List<ReplicaInstance> GetAllReplicaInstances();

		List<ReplicaInstanceContainer> GetAllReplicaInstanceContainers();

		ISetVolumeInfo GetVolumeInfoCallback(Guid instanceGuid, bool activePassiveAgnostic);

		ISetSeeding GetSeederStatusCallback(Guid instanceGuid);

		IGetStatus GetStatusRetrieverCallback(Guid instanceGuid);

		bool GetRunningInstanceSignatureAndCheckpoint(Guid instanceGuid, out JET_SIGNATURE? logfileSignature, out long lowestGenerationRequired, out long highestGenerationRequired, out long lastGenerationBackedUp);

		CopyStatusEnum GetCopyStatus(Guid instanceGuid);

		ReplayState GetRunningInstanceState(Guid instanceGuid);

		bool CreateTempLogFileForRunningInstance(Guid instanceGuid);

		AmAcllReturnStatus AmAttemptCopyLastLogsCallback(Guid mdbGuid, AmAcllArgs acllArgs);

		void AmPreMountCallback(Guid mdbGuid, ref int storeMountFlags, AmMountFlags amMountFlags, MountDirectPerformanceTracker mountPerf, out LogStreamResetOnMount logReset, out ReplicaInstanceContext instanceContext);

		void RequestSuspend(Guid guid, string suspendComment, DatabaseCopyActionFlags flags, ActionInitiatorType initiator);

		void RequestResume(Guid guid, DatabaseCopyActionFlags flags);

		void SyncSuspendResumeState(Guid guid);

		void DisableReplayLag(Guid guid, ActionInitiatorType actionInitiator, string reason);

		void EnableReplayLag(Guid guid, ActionInitiatorType actionInitiator);

		ISetPassiveSeeding GetPassiveSeederStatusCallback(Guid instanceGuid);

		ISetActiveSeeding GetActiveSeederStatusCallback(Guid instanceGuid);

		ISetGeneration GetSetGenerationCallback(Guid instanceGuid);
	}
}
