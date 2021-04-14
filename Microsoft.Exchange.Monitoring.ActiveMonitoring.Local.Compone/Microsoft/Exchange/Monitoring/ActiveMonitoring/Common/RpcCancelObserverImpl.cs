using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class RpcCancelObserverImpl
	{
		public static void HandleCancelObserver(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcCancelObserverImpl.CancelObserverInput cancelObserverInput = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcCancelObserverImpl.CancelObserverInput>(requestInfo, 1, 0);
			RpcCancelObserverImpl.CancelObserverReply cancelObserverReply = new RpcCancelObserverImpl.CancelObserverReply();
			MonitoringServerManager.RemoveSubject(cancelObserverInput.ServerName);
			cancelObserverReply.IsAccepted = true;
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, cancelObserverReply, 1, 0);
		}

		public static void SendCancelObserver(string serverName)
		{
			RpcCancelObserverImpl.CancelObserverInput cancelObserverInput = new RpcCancelObserverImpl.CancelObserverInput();
			cancelObserverInput.ServerName = NativeHelpers.GetLocalComputerFqdn(true);
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.GenericRpcTracer, RpcCancelObserverImpl.traceContext, "RpcCancelObserverImpl.CancelObserver() called. (serverName:{0})", serverName, null, "SendCancelObserver", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcCancelObserverImpl.cs", 89);
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(cancelObserverInput, ActiveMonitoringGenericRpcCommandId.CancelObserver, 1, 0);
			RpcCancelObserverImpl.CancelObserverReply cancelObserverReply = ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcCancelObserverImpl.CancelObserverReply>(requestInfo, serverName, 5000);
			WTFDiagnostics.TraceDebug<string, bool>(ExTraceGlobals.GenericRpcTracer, RpcCancelObserverImpl.traceContext, "RpcCancelObserverImpl.CancelObserver() returned. (serverName:{0}, isAccepted:{1})", serverName, cancelObserverReply.IsAccepted, null, "SendCancelObserver", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcCancelObserverImpl.cs", 108);
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		private static TracingContext traceContext = TracingContext.Default;

		[Serializable]
		internal class CancelObserverInput
		{
			public string ServerName { get; set; }
		}

		[Serializable]
		internal class CancelObserverReply
		{
			public bool IsAccepted { get; set; }
		}
	}
}
