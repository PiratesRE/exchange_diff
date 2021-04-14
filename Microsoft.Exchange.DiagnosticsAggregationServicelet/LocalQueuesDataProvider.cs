using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Net.DiagnosticsAggregation;
using Microsoft.Exchange.Transport.DiagnosticsAggregationService;

namespace Microsoft.Exchange.Servicelets.DiagnosticsAggregation
{
	internal class LocalQueuesDataProvider : ILocalQueuesDataProvider
	{
		public LocalQueuesDataProvider(DiagnosticsAggregationLog log, ADObjectId localServerId)
		{
			this.log = log;
			this.localServerId = localServerId;
			this.serverQueuesSnapshot = new ServerQueuesSnapshot(localServerId);
		}

		public void Start()
		{
			this.RefreshLocalServerQueues();
		}

		public void Stop()
		{
		}

		public ADObjectId GetLocalServerId()
		{
			return this.localServerId;
		}

		public ServerQueuesSnapshot GetLocalServerQueues()
		{
			this.RefreshLocalServerQueues();
			return this.serverQueuesSnapshot;
		}

		private void RefreshLocalServerQueues()
		{
			QueueAggregationInfo queueAggregationInfo;
			Exception ex;
			bool flag = DiagnosticsAggregationHelper.TryGetParsedQueueInfo(DiagnosticsAggregationServicelet.LocalServer.QueueLogPath, out queueAggregationInfo, out ex);
			if (flag)
			{
				this.log.Log(DiagnosticsAggregationEvent.QueueSnapshotFileReadSucceeded, string.Empty, new object[0]);
				this.serverQueuesSnapshot.UpdateSuccess(queueAggregationInfo.QueueInfo, queueAggregationInfo.Time);
				return;
			}
			this.log.Log(DiagnosticsAggregationEvent.QueueSnapshotFileReadFailed, "Refreshing local queue information failed. Details {0}", new object[]
			{
				ex.Message
			});
			this.serverQueuesSnapshot.UpdateFailure(ex.Message);
		}

		private DiagnosticsAggregationLog log;

		private ADObjectId localServerId;

		private ServerQueuesSnapshot serverQueuesSnapshot;
	}
}
