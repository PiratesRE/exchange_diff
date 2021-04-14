using System;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class RpcGetCrimsonEventImpl
	{
		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcGetCrimsonEventImpl.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcGetCrimsonEventImpl.Request>(requestInfo, 1, 0);
			RpcGetCrimsonEventImpl.Reply reply = new RpcGetCrimsonEventImpl.Reply();
			if (DirectoryAccessor.Instance.IsRecoveryActionsEnabledOffline(request.Servername))
			{
				reply.LastResultTimestamp = RpcGetCrimsonEventImpl.GetLocalLastResultTimestamp<MonitorResult>("HealthManagerHeartbeatMonitor");
				reply.ResultType = WorkItemResultType.Monitor;
			}
			else
			{
				reply.LastResultTimestamp = RpcGetCrimsonEventImpl.GetLocalLastResultTimestamp<ResponderResult>("HealthManagerHeartbeatResponder");
				reply.ResultType = WorkItemResultType.Responder;
			}
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, reply, 1, 0);
		}

		public static void SendRequest(string serverName, out DateTime? lastResultTimestamp, int timeoutInMSec = 30000)
		{
			RpcGetCrimsonEventImpl.Request request = new RpcGetCrimsonEventImpl.Request();
			request.Servername = serverName;
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.GenericRpcTracer, RpcGetCrimsonEventImpl.traceContext, "RpcGetCrimsonEventImpl.SendRequest() called. (serverName:{0})", serverName, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcGetCrimsonEventImpl.cs", 123);
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(request, ActiveMonitoringGenericRpcCommandId.GetCrimsonMostRecentResultInfo, 1, 0);
			RpcGetCrimsonEventImpl.Reply reply = ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcGetCrimsonEventImpl.Reply>(requestInfo, serverName, timeoutInMSec);
			lastResultTimestamp = reply.LastResultTimestamp;
			WTFDiagnostics.TraceDebug<string, DateTime?, WorkItemResultType>(ExTraceGlobals.GenericRpcTracer, RpcGetCrimsonEventImpl.traceContext, "RpcGetCrimsonEventImpl.SendRequest() returned. (serverName:{0}, lastResultTimestamp:{1} type:{2})", serverName, lastResultTimestamp, reply.ResultType, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcGetCrimsonEventImpl.cs", 144);
		}

		private static DateTime? GetLocalLastResultTimestamp<TResult>(string resultName) where TResult : WorkItemResult, IPersistence, new()
		{
			DateTime? result = null;
			TResult tresult = default(TResult);
			using (CrimsonReader<TResult> crimsonReader = new CrimsonReader<TResult>())
			{
				crimsonReader.QueryUserPropertyCondition = string.Format("(ResultName='{0}')", resultName);
				crimsonReader.IsReverseDirection = true;
				tresult = crimsonReader.ReadNext();
			}
			if (tresult != null)
			{
				result = new DateTime?(tresult.ExecutionEndTime);
			}
			return result;
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		public const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.GetCrimsonMostRecentResultInfo;

		private static TracingContext traceContext = TracingContext.Default;

		[Serializable]
		internal class Request
		{
			public string Servername { get; set; }
		}

		[Serializable]
		internal class Reply
		{
			public DateTime? LastResultTimestamp { get; set; }

			public WorkItemResultType ResultType { get; set; }
		}
	}
}
