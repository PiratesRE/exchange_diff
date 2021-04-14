using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class RpcObserverHeartbeatImpl
	{
		public static void HandleObserverHeartbeat(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcObserverHeartbeatImpl.ObserverHeartbeatInput observerHeartbeatInput = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcObserverHeartbeatImpl.ObserverHeartbeatInput>(requestInfo, 1, 0);
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, new RpcObserverHeartbeatImpl.ObserverHeartbeatReply
			{
				Response = MonitoringServerManager.UpdateObserverHeartbeat(observerHeartbeatInput.ServerName)
			}, 1, 0);
		}

		public static void SendObserverHeartbeat(string serverName, out ObserverHeartbeatResponse response)
		{
			response = ObserverHeartbeatResponse.NoResponse;
			RpcObserverHeartbeatImpl.ObserverHeartbeatInput observerHeartbeatInput = new RpcObserverHeartbeatImpl.ObserverHeartbeatInput();
			observerHeartbeatInput.ServerName = NativeHelpers.GetLocalComputerFqdn(true);
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.GenericRpcTracer, RpcObserverHeartbeatImpl.traceContext, "RpcObserverHeartbeatImpl.ObserverHeartbeat() called. (serverName:{0})", serverName, null, "SendObserverHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcObserverHeartbeatImpl.cs", 116);
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(observerHeartbeatInput, ActiveMonitoringGenericRpcCommandId.ObserverHeartbeat, 1, 0);
			RpcObserverHeartbeatImpl.ObserverHeartbeatReply observerHeartbeatReply = ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcObserverHeartbeatImpl.ObserverHeartbeatReply>(requestInfo, serverName, 5000);
			response = observerHeartbeatReply.Response;
			WTFDiagnostics.TraceDebug<string, ObserverHeartbeatResponse>(ExTraceGlobals.GenericRpcTracer, RpcObserverHeartbeatImpl.traceContext, "RpcObserverHeartbeatImpl.ObserverHeartbeat() returned. (serverName:{0}, response:{1})", serverName, response, null, "SendObserverHeartbeat", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcObserverHeartbeatImpl.cs", 137);
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		private static TracingContext traceContext = TracingContext.Default;

		[Serializable]
		internal class ObserverHeartbeatInput
		{
			public string ServerName { get; set; }
		}

		[Serializable]
		internal class ObserverHeartbeatReply
		{
			public ObserverHeartbeatResponse Response { get; set; }
		}
	}
}
