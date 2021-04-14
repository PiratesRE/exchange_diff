using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Net.DiagnosticsAggregation;

namespace Microsoft.Exchange.Servicelets.DiagnosticsAggregation
{
	internal class ServerQueuesSnapshot
	{
		public ServerQueuesSnapshot(ADObjectId localServer)
		{
			if (localServer == null)
			{
				throw new ArgumentNullException("localServer");
			}
			this.serverIdentity = localServer;
		}

		public ServerQueuesSnapshot(ADObjectId localServer, IEnumerable<LocalQueueInfo> queues, DateTime timeStampOfQueues)
		{
			if (localServer == null)
			{
				throw new ArgumentNullException("localServer");
			}
			if (queues == null)
			{
				throw new ArgumentNullException("queues");
			}
			this.serverIdentity = localServer;
			this.UpdateSuccess(queues, timeStampOfQueues);
		}

		public IEnumerable<LocalQueueInfo> Queues
		{
			get
			{
				if (this.queues == null)
				{
					throw new InvalidOperationException("queues has not been set yet");
				}
				return this.queues;
			}
		}

		public DateTime TimeStampOfQueues
		{
			get
			{
				if (this.timeStampOfQueues == null)
				{
					throw new InvalidOperationException("timeStampOfQueues has not been set yet");
				}
				return this.timeStampOfQueues.Value;
			}
		}

		public string LastError
		{
			get
			{
				return this.lastError;
			}
		}

		public void UpdateSuccess(LocalViewResponse response)
		{
			this.queues = response.QueueLocalViewResponse.LocalQueues;
			this.timeStampOfQueues = new DateTime?(response.QueueLocalViewResponse.Timestamp);
			this.timeOfLastSuccess = response.ServerSnapshotStatus.TimeOfLastSuccess;
			this.timeOfLastFailure = response.ServerSnapshotStatus.TimeOfLastFailure;
			this.lastError = response.ServerSnapshotStatus.LastError;
		}

		public void UpdateSuccess(IEnumerable<LocalQueueInfo> localQueues, DateTime timeStampOfQueues)
		{
			this.queues = localQueues;
			this.timeStampOfQueues = new DateTime?(timeStampOfQueues);
			this.timeOfLastSuccess = new DateTime?(DateTime.UtcNow);
			this.timeOfLastFailure = null;
			this.lastError = string.Empty;
		}

		public void UpdateFailure(string errorMessage)
		{
			this.lastError = errorMessage;
			this.timeOfLastFailure = new DateTime?(DateTime.UtcNow);
		}

		public void SetAsFailed(string errorMessage)
		{
			this.queues = null;
			this.timeStampOfQueues = null;
			this.timeOfLastSuccess = null;
			this.timeOfLastFailure = new DateTime?(DateTime.UtcNow);
			this.lastError = errorMessage;
		}

		public ServerSnapshotStatus GetServerSnapshotStatus()
		{
			return new ServerSnapshotStatus(this.serverIdentity.ToString())
			{
				TimeOfLastSuccess = this.timeOfLastSuccess,
				TimeOfLastFailure = this.timeOfLastFailure,
				LastError = this.lastError
			};
		}

		public ServerQueuesSnapshot Clone()
		{
			return new ServerQueuesSnapshot(this.serverIdentity)
			{
				queues = this.queues,
				timeStampOfQueues = this.timeStampOfQueues,
				timeOfLastSuccess = this.timeOfLastSuccess,
				timeOfLastFailure = this.timeOfLastFailure,
				lastError = this.lastError
			};
		}

		public bool IsEmpty()
		{
			return this.queues == null;
		}

		private readonly ADObjectId serverIdentity;

		private IEnumerable<LocalQueueInfo> queues;

		private DateTime? timeStampOfQueues;

		private DateTime? timeOfLastSuccess;

		private DateTime? timeOfLastFailure;

		private string lastError = string.Empty;
	}
}
