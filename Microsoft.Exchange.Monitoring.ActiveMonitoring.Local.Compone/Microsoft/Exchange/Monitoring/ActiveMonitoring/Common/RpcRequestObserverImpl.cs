using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class RpcRequestObserverImpl
	{
		public static void HandleRequestObserver(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcRequestObserverImpl.RequestObserverInput requestObserverInput = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcRequestObserverImpl.RequestObserverInput>(requestInfo, 1, 0);
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, new RpcRequestObserverImpl.RequestObserverReply
			{
				IsAccepted = MonitoringServerManager.TryAddSubject(requestObserverInput.ServerName)
			}, 1, 0);
		}

		public static void SendRequestObserver(string serverName, out bool isAccepted)
		{
			isAccepted = false;
			RpcRequestObserverImpl.RequestObserverInput requestObserverInput = new RpcRequestObserverImpl.RequestObserverInput();
			requestObserverInput.ServerName = NativeHelpers.GetLocalComputerFqdn(true);
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.GenericRpcTracer, RpcRequestObserverImpl.traceContext, "RpcRequestObserverImpl.RequestObserver() called. (serverName:{0})", serverName, null, "SendRequestObserver", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcRequestObserverImpl.cs", 90);
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(requestObserverInput, ActiveMonitoringGenericRpcCommandId.RequestObserver, 1, 0);
			RpcRequestObserverImpl.RequestObserverReply requestObserverReply = ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcRequestObserverImpl.RequestObserverReply>(requestInfo, serverName, 5000);
			isAccepted = requestObserverReply.IsAccepted;
			WTFDiagnostics.TraceDebug<string, bool>(ExTraceGlobals.GenericRpcTracer, RpcRequestObserverImpl.traceContext, "RpcRequestObserverImpl.RequestObserver() returned. (serverName:{0}, isAccepted:{1})", serverName, isAccepted, null, "SendRequestObserver", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcRequestObserverImpl.cs", 111);
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		private static TracingContext traceContext = TracingContext.Default;

		[Serializable]
		internal class RequestObserverInput
		{
			public string ServerName { get; set; }
		}

		[Serializable]
		internal class RequestObserverReply
		{
			public bool IsAccepted { get; set; }
		}
	}
}
