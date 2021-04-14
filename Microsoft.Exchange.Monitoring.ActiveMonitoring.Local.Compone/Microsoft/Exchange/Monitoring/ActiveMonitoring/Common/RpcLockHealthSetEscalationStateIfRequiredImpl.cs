using System;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class RpcLockHealthSetEscalationStateIfRequiredImpl
	{
		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcLockHealthSetEscalationStateIfRequiredImpl.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcLockHealthSetEscalationStateIfRequiredImpl.Request>(requestInfo, 1, 0);
			HealthSetEscalationState healthSetEscalationState = MonitorResultCacheManager.Instance.LockHealthSetEscalationStateIfRequired(request.HealthSetName, request.EscalationState, request.LockOwnerId);
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, new RpcLockHealthSetEscalationStateIfRequiredImpl.Reply
			{
				HealthSetEscalationState = healthSetEscalationState
			}, 1, 0);
		}

		public static HealthSetEscalationState SendRequest(string serverName, string healthSetName, EscalationState escalationState, string lockOwnerId, int timeoutInMSec = 30000)
		{
			WTFDiagnostics.TraceDebug<string, string, string>(ExTraceGlobals.GenericRpcTracer, RpcLockHealthSetEscalationStateIfRequiredImpl.traceContext, "RpcLockHealthSetEscalationStateIfRequiredImpl.LockHealthSetEscalationStateIfRequired: healthSetName:{0} escalationState:{1} lockOwnerId:{2}", healthSetName, escalationState.ToString(), lockOwnerId, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcLockHealthSetEscalationStateIfRequiredImpl.cs", 91);
			RpcLockHealthSetEscalationStateIfRequiredImpl.Request attachedRequest = new RpcLockHealthSetEscalationStateIfRequiredImpl.Request(healthSetName, escalationState, lockOwnerId);
			RpcGenericRequestInfo rpcGenericRequestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(attachedRequest, ActiveMonitoringGenericRpcCommandId.LockHealthSetEscalationStateIfRequired, 1, 0);
			WTFDiagnostics.TraceDebug<int>(ExTraceGlobals.ResultCacheTracer, RpcLockHealthSetEscalationStateIfRequiredImpl.traceContext, "Invoking LockHealthSetEscalationStateIfRequired RPC with {0} bytes of data", rpcGenericRequestInfo.AttachedData.Length, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcLockHealthSetEscalationStateIfRequiredImpl.cs", 109);
			RpcLockHealthSetEscalationStateIfRequiredImpl.Reply reply = ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcLockHealthSetEscalationStateIfRequiredImpl.Reply>(rpcGenericRequestInfo, serverName, timeoutInMSec);
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.GenericRpcTracer, RpcLockHealthSetEscalationStateIfRequiredImpl.traceContext, "RpcLockHealthSetEscalationStateIfRequiredImpl.LockHealthSetEscalationStateIfRequired returned. (serverName:{0})", serverName, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcLockHealthSetEscalationStateIfRequiredImpl.cs", 121);
			return reply.HealthSetEscalationState;
		}

		public const int CommandMajorVersion = 1;

		public const int CommandMinorVersion = 0;

		public const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.LockHealthSetEscalationStateIfRequired;

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
			public HealthSetEscalationState HealthSetEscalationState { get; set; }
		}
	}
}
