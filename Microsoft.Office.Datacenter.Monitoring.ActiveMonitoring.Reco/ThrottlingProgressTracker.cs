using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	internal class ThrottlingProgressTracker
	{
		internal ThrottlingProgressTracker(long instanceId, RecoveryActionId actionId, string resourceName, string requesterName, TimeSpan timeout)
		{
			this.InstanceId = instanceId;
			this.actionId = actionId;
			this.resourceName = resourceName;
			this.requesterName = requesterName;
			this.timeout = timeout;
		}

		internal long InstanceId { get; private set; }

		internal void MarkBegin()
		{
			DateTime localTime = ExDateTime.Now.LocalTime;
			RpcSetThrottlingInProgressImpl.ThrottlingProgressInfo progressInfo = new RpcSetThrottlingInProgressImpl.ThrottlingProgressInfo
			{
				InstanceId = this.InstanceId,
				ActionId = this.actionId,
				ResourceName = this.resourceName,
				RequesterName = this.requesterName,
				OperationStartTime = localTime,
				OperationExpectedEndTime = localTime + this.timeout,
				WorkerStartTime = RecoveryActionHelper.CurrentProcessStartTime
			};
			RpcSetThrottlingInProgressImpl.Reply reply = Dependencies.LamRpc.SetThrottlingInProgress(Dependencies.ThrottleHelper.Tunables.LocalMachineName, progressInfo, false, false, 30000);
			if (reply.IsSuccess)
			{
				this.current = progressInfo;
				this.isBeginMarked = true;
				return;
			}
			throw new ThrottlingInProgressException(this.InstanceId, this.actionId.ToString(), this.resourceName, this.requesterName, reply.CurrentProgressInfo.RequesterName, reply.CurrentProgressInfo.OperationStartTime, reply.CurrentProgressInfo.OperationExpectedEndTime);
		}

		internal void MarkEnd()
		{
			if (!this.isBeginMarked || this.isEndMarked)
			{
				return;
			}
			RpcSetThrottlingInProgressImpl.Reply reply = Dependencies.LamRpc.SetThrottlingInProgress(Dependencies.ThrottleHelper.Tunables.LocalMachineName, this.current, true, false, 30000);
			if (reply.IsSuccess)
			{
				this.isEndMarked = true;
				return;
			}
			throw new ThrottlingOverlapException(this.current.InstanceId, reply.CurrentProgressInfo.InstanceId, this.current.RequesterName, reply.CurrentProgressInfo.RequesterName, this.current.OperationStartTime, reply.CurrentProgressInfo.OperationStartTime);
		}

		private readonly RecoveryActionId actionId;

		private readonly string resourceName;

		private readonly string requesterName;

		private readonly TimeSpan timeout;

		private bool isBeginMarked;

		private bool isEndMarked;

		private RpcSetThrottlingInProgressImpl.ThrottlingProgressInfo current;
	}
}
