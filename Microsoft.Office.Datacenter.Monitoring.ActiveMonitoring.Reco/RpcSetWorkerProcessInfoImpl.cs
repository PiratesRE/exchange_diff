using System;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public static class RpcSetWorkerProcessInfoImpl
	{
		public static void SendRequest(string serverName, RpcSetWorkerProcessInfoImpl.WorkerProcessInfo info, int timeoutInMsec = 30000)
		{
			RpcSetWorkerProcessInfoImpl.Request attachedRequest = new RpcSetWorkerProcessInfoImpl.Request
			{
				WorkerInfo = info
			};
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(attachedRequest, ActiveMonitoringGenericRpcCommandId.SetWorkerProcessInfo, 1, 0);
			ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcSetWorkerProcessInfoImpl.Reply>(requestInfo, serverName, timeoutInMsec);
		}

		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcSetWorkerProcessInfoImpl.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcSetWorkerProcessInfoImpl.Request>(requestInfo, 1, 0);
			WorkerProcessRepository.Instance.WorkerProcessInfo = request.WorkerInfo;
			RpcSetWorkerProcessInfoImpl.Reply attachedReply = new RpcSetWorkerProcessInfoImpl.Reply();
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, attachedReply, 1, 0);
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		internal const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.SetWorkerProcessInfo;

		[Serializable]
		public class WorkerProcessInfo
		{
			internal DateTime ProcessStartTime
			{
				get
				{
					return this.ProcessStartTimeUtc.ToLocalTime();
				}
				set
				{
					this.ProcessStartTimeUtc = value.ToUniversalTime();
				}
			}

			internal DateTime ProcessStartTimeUtc { get; set; }
		}

		[Serializable]
		internal class Request
		{
			public RpcSetWorkerProcessInfoImpl.WorkerProcessInfo WorkerInfo { get; set; }
		}

		[Serializable]
		internal class Reply
		{
		}
	}
}
