using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IStoreRpc : IListMDBStatus, IStoreMountDismount, IDisposable
	{
		TimeSpan ConnectivityTimeout { get; }

		void ForceNewLog(Guid guidMdb, long numLogsToRoll);

		void LogReplayRequest(Guid guidMdb, uint ulgenLogReplayMax, uint flags, out uint ulgenLogReplayNext, out JET_DBINFOMISC dbinfo, out IPagePatchReply pagePatchReply, out uint[] corruptedPages);

		void StartBlockModeReplicationToPassive(Guid guidMdb, string passiveName, uint ulFirstGenToSend);

		bool TestStoreConnectivity(TimeSpan timeout, out LocalizedException ex);

		void SnapshotPrepare(Guid dbGuid, uint flags);

		void SnapshotFreeze(Guid dbGuid, uint flags);

		void SnapshotThaw(Guid dbGuid, uint flags);

		void SnapshotTruncateLogInstance(Guid dbGuid, uint flags);

		void SnapshotStop(Guid dbGuid, uint flags);

		void GetDatabaseInformation(Guid guidMdb, out JET_DBINFOMISC databaseInformation);

		void GetDatabaseProcessInfo(Guid guidMdb, out int workerProcessId, out int minVersion, out int maxVersion, out int requestedVersion);

		void Close();
	}
}
