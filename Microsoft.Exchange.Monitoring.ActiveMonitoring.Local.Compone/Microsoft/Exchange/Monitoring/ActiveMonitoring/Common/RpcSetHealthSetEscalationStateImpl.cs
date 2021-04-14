using System;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class RpcSetHealthSetEscalationStateImpl
	{
		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcSetHealthSetEscalationStateImpl.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcSetHealthSetEscalationStateImpl.Request>(requestInfo, 1, 0);
			bool succeeded = MonitorResultCacheManager.Instance.SetHealthSetEscalationState(request.HealthSetName, request.EscalationState, request.LockOwnerId);
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, new RpcSetHealthSetEscalationStateImpl.Reply
			{
				Succeeded = succeeded
			}, 1, 0);
		}

		public static bool SendRequest(string serverName, string healthSetName, EscalationState escalationState, string lockOwnerId, int timeoutInMSec = 30000)
		{
			WTFDiagnostics.TraceDebug<string, string, string>(ExTraceGlobals.GenericRpcTracer, RpcSetHealthSetEscalationStateImpl.traceContext, "RpcSetHealthSetEscalationStateImpl.SetHealthSetEscalationState: healthSetName:{0} escalationState:{1} lockOwnerId:{2}", healthSetName, escalationState.ToString(), lockOwnerId, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcSetHealthSetEscalationStateImpl.cs", 91);
			RpcSetHealthSetEscalationStateImpl.Request attachedRequest = new RpcSetHealthSetEscalationStateImpl.Request(healthSetName, escalationState, lockOwnerId);
			RpcGenericRequestInfo rpcGenericRequestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(attachedRequest, ActiveMonitoringGenericRpcCommandId.SetHealthSetEscalationState, 1, 0);
			WTFDiagnostics.TraceDebug<int>(ExTraceGlobals.ResultCacheTracer, RpcSetHealthSetEscalationStateImpl.traceContext, "Invoking SetHealthSetEscalationState RPC with {0} bytes of data", rpcGenericRequestInfo.AttachedData.Length, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcSetHealthSetEscalationStateImpl.cs", 109);
			RpcSetHealthSetEscalationStateImpl.Reply reply = ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcSetHealthSetEscalationStateImpl.Reply>(rpcGenericRequestInfo, serverName, timeoutInMSec);
			WTFDiagnostics.TraceDebug<bool, string>(ExTraceGlobals.GenericRpcTracer, RpcSetHealthSetEscalationStateImpl.traceContext, "RpcSetHealthSetEscalationStateImpl.SetHealthSetEscalationState returned '{0}'. (serverName:{1})", reply.Succeeded, serverName, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcSetHealthSetEscalationStateImpl.cs", 121);
			return reply.Succeeded;
		}

		public const int CommandMajorVersion = 1;

		public const int CommandMinorVersion = 0;

		public const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.SetHealthSetEscalationState;

		private static TracingContext traceContext = TracingContext.Default;

		[Serializable]
		internal class Request
		{
			public Request(string healthSetName, EscalationState escalationState, string lockOwnerId)
			{
				this.HealthSetName = healthSetName;
				this.EscalationState = escalationState;
				this.LockOwnerId = lockOwnerId;
			}

			public string HealthSetName { get; set; }

			public EscalationState EscalationState { get; set; }

			public string LockOwnerId { get; set; }
		}

		[Serializable]
		internal class Reply
		{
			public bool Succeeded { get; set; }
		}
	}
}
