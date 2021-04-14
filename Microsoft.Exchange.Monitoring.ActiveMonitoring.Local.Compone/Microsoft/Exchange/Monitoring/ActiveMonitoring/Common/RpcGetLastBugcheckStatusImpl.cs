using System;
using System.Threading;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class RpcGetLastBugcheckStatusImpl
	{
		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			if (BugcheckSimulator.Instance.IsHangRpc)
			{
				TimeSpan duration = BugcheckSimulator.Instance.Duration;
				Thread.Sleep(duration + TimeSpan.FromSeconds(5.0));
			}
			RpcGetLastBugcheckStatusImpl.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcGetLastBugcheckStatusImpl.Request>(requestInfo, 1, 0);
			DateTime systemBootTime = RecoveryActionHelper.GetSystemBootTime();
			DateTime inflightBugcheckStartTime = DateTime.MinValue;
			TimeSpan inflightBugcheckTimeRemaining = TimeSpan.Zero;
			RecoveryActionEntry recoveryActionEntry = RecoveryActionHelper.FindActionEntry(RecoveryActionId.ForceReboot, null, request.QueryStartTime, request.QueryEndTime);
			if (recoveryActionEntry != null && recoveryActionEntry.State == RecoveryActionState.Started)
			{
				inflightBugcheckStartTime = recoveryActionEntry.StartTime;
				inflightBugcheckTimeRemaining = recoveryActionEntry.EndTime - recoveryActionEntry.StartTime;
			}
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, new RpcGetLastBugcheckStatusImpl.Reply
			{
				SystemStartTime = systemBootTime,
				InflightBugcheckStartTime = inflightBugcheckStartTime,
				InflightBugcheckTimeRemaining = inflightBugcheckTimeRemaining
			}, 1, 0);
		}

		public static void SendRequest(string serverName, DateTime queryStartTime, DateTime queryEndTime, out DateTime systemStartTime, out DateTime inflightBugcheckStartTime, out TimeSpan inflightBugcheckTimeRemaining, int timeoutInMSec = 30000)
		{
			RpcGetLastBugcheckStatusImpl.Request request = new RpcGetLastBugcheckStatusImpl.Request();
			request.QueryStartTime = queryStartTime;
			request.QueryEndTime = queryEndTime;
			WTFDiagnostics.TraceDebug<string, DateTime, DateTime>(ExTraceGlobals.GenericRpcTracer, RpcGetLastBugcheckStatusImpl.traceContext, "RpcGetLastBugcheckStatusImpl.SendRequest() called. (serverName:{0}, queryStartTime:{1}, queryEndTime: {2})", serverName, queryStartTime, queryEndTime, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcGetLastBugcheckStatusImpl.cs", 127);
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(request, ActiveMonitoringGenericRpcCommandId.GetLastBugcheckStatus, 1, 0);
			RpcGetLastBugcheckStatusImpl.Reply reply = ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcGetLastBugcheckStatusImpl.Reply>(requestInfo, serverName, timeoutInMSec);
			systemStartTime = reply.SystemStartTime;
			inflightBugcheckStartTime = reply.InflightBugcheckStartTime;
			inflightBugcheckTimeRemaining = reply.InflightBugcheckTimeRemaining;
			WTFDiagnostics.TraceDebug<string, DateTime, DateTime, TimeSpan>(ExTraceGlobals.GenericRpcTracer, RpcGetLastBugcheckStatusImpl.traceContext, "RpcGetLastBugcheckStatusImpl.SendRequest() returned. (serverName:{0}, systemStartTime:{1}, inflightBugcheckTime:{2}, inflightBugcheckDuration: {3})", serverName, systemStartTime, inflightBugcheckStartTime, inflightBugcheckTimeRemaining, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcGetLastBugcheckStatusImpl.cs", 152);
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		public const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.GetLastBugcheckStatus;

		private static TracingContext traceContext = TracingContext.Default;

		[Serializable]
		internal class Request
		{
			public DateTime QueryStartTime { get; set; }

			public DateTime QueryEndTime { get; set; }
		}

		[Serializable]
		internal class Reply
		{
			public DateTime SystemStartTime { get; set; }

			public DateTime InflightBugcheckStartTime { get; set; }

			public TimeSpan InflightBugcheckTimeRemaining { get; set; }
		}
	}
}
