using System;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc.ActiveMonitoring;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public static class RpcSetThrottlingInProgressImpl
	{
		public static RpcSetThrottlingInProgressImpl.Reply SendRequest(string serverName, RpcSetThrottlingInProgressImpl.ThrottlingProgressInfo progressInfo, bool isClear, bool isForce = false, int timeoutInMsec = 30000)
		{
			RpcSetThrottlingInProgressImpl.Request attachedRequest = new RpcSetThrottlingInProgressImpl.Request
			{
				ProgressInfo = progressInfo,
				IsClear = isClear,
				IsForce = isForce
			};
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(attachedRequest, ActiveMonitoringGenericRpcCommandId.SetThrottlingInProgress, 1, 0);
			return ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcSetThrottlingInProgressImpl.Reply>(requestInfo, serverName, timeoutInMsec);
		}

		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcSetThrottlingInProgressImpl.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcSetThrottlingInProgressImpl.Request>(requestInfo, 1, 0);
			RpcSetThrottlingInProgressImpl.Reply attachedReply = RecoveryActionsRepository.Instance.UpdateThrottlingProgress(request);
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, attachedReply, 1, 0);
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		internal const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.SetThrottlingInProgress;

		[Serializable]
		public class Reply
		{
			public bool IsSuccess { get; set; }

			public bool IsThrottlingAlreadyInProgress { get; set; }

			public RpcSetThrottlingInProgressImpl.ThrottlingProgressInfo CurrentProgressInfo { get; set; }
		}

		[Serializable]
		public class Request
		{
			public RpcSetThrottlingInProgressImpl.ThrottlingProgressInfo ProgressInfo { get; set; }

			public bool IsClear { get; set; }

			public bool IsForce { get; set; }
		}

		[Serializable]
		public class ThrottlingProgressInfo
		{
			internal long InstanceId { get; set; }

			internal RecoveryActionId ActionId { get; set; }

			internal string ResourceName { get; set; }

			internal string RequesterName { get; set; }

			internal DateTime OperationStartTime
			{
				get
				{
					return this.OperationStartTimeUtc.ToLocalTime();
				}
				set
				{
					this.OperationStartTimeUtc = value.ToUniversalTime();
				}
			}

			internal DateTime OperationStartTimeUtc { get; set; }

			internal DateTime OperationExpectedEndTime
			{
				get
				{
					return this.OperationExpectedEndTimeUtc.ToLocalTime();
				}
				set
				{
					this.OperationExpectedEndTimeUtc = value.ToUniversalTime();
				}
			}

			internal DateTime OperationExpectedEndTimeUtc { get; set; }

			internal DateTime WorkerStartTime
			{
				get
				{
					return this.WorkerStartTimeUtc.ToLocalTime();
				}
				set
				{
					this.WorkerStartTimeUtc = value.ToUniversalTime();
				}
			}

			internal DateTime WorkerStartTimeUtc { get; set; }

			internal bool IsInProgress(DateTime workerProcessStartTime)
			{
				return !string.IsNullOrEmpty(this.RequesterName) && !(this.OperationStartTime == DateTime.MinValue) && !(this.OperationExpectedEndTime == DateTime.MinValue) && !(ExDateTime.Now.LocalTime > this.OperationExpectedEndTime) && !(workerProcessStartTime > this.WorkerStartTime) && !(workerProcessStartTime > this.OperationStartTime);
			}
		}
	}
}
