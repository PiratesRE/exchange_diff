using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class RpcGetServerComponentStatusImpl
	{
		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcGetServerComponentStatusImpl.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcGetServerComponentStatusImpl.Request>(requestInfo, 1, 0);
			DateTime lastOfflineRequestStartTime = DateTime.MinValue;
			DateTime lastOfflineRequestEndTime = DateTime.MinValue;
			RecoveryActionEntry recoveryActionEntry = RecoveryActionHelper.FindActionEntry(RecoveryActionId.TakeComponentOffline, null, request.QueryStartTime, request.QueryEndTime);
			if (recoveryActionEntry != null)
			{
				lastOfflineRequestStartTime = recoveryActionEntry.StartTime;
				lastOfflineRequestEndTime = recoveryActionEntry.EndTime;
			}
			RpcGetServerComponentStatusImpl.Reply reply = new RpcGetServerComponentStatusImpl.Reply();
			ServerComponentEnum serverComponentEnum = ServerComponentEnum.None;
			Enum.TryParse<ServerComponentEnum>(request.ServerComponentName, out serverComponentEnum);
			if (serverComponentEnum != ServerComponentEnum.None)
			{
				reply.IsOnline = ServerComponentStateManager.IsOnline(serverComponentEnum);
			}
			else
			{
				reply.IsOnline = true;
			}
			reply.LastOfflineRequestStartTime = lastOfflineRequestStartTime;
			reply.LastOfflineRequestEndTime = lastOfflineRequestEndTime;
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, reply, 1, 0);
		}

		public static void SendRequest(string serverName, ServerComponentEnum serverComponent, DateTime queryStartTime, DateTime queryEndTime, out bool isOnline, out DateTime lastOfflineRequestStartTime, out DateTime lastOfflineRequestEndTime, int timeoutInMSec = 30000)
		{
			RpcGetServerComponentStatusImpl.Request request = new RpcGetServerComponentStatusImpl.Request();
			request.ServerComponentName = serverComponent.ToString();
			request.QueryStartTime = queryStartTime;
			request.QueryEndTime = queryEndTime;
			WTFDiagnostics.TraceDebug<string, DateTime, DateTime>(ExTraceGlobals.GenericRpcTracer, RpcGetServerComponentStatusImpl.traceContext, "RpcGetServerComponentStatusImpl.SendRequest() called. (serverName:{0}, queryStartTime:{1}, queryEndTime: {2})", serverName, queryStartTime, queryEndTime, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcGetServerComponentStatusImpl.cs", 137);
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(request, ActiveMonitoringGenericRpcCommandId.GetServerComponentStatus, 1, 0);
			RpcGetServerComponentStatusImpl.Reply reply = ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcGetServerComponentStatusImpl.Reply>(requestInfo, serverName, timeoutInMSec);
			isOnline = reply.IsOnline;
			lastOfflineRequestStartTime = reply.LastOfflineRequestStartTime;
			lastOfflineRequestEndTime = reply.LastOfflineRequestEndTime;
			WTFDiagnostics.TraceDebug<string, bool, DateTime, DateTime>(ExTraceGlobals.GenericRpcTracer, RpcGetServerComponentStatusImpl.traceContext, "RpcGetServerComponentStatusImpl.SendRequest() returned. (serverName:{0}, isOnline:{1} lastOfflineRequestStartTime:{2}, lastOfflineRequestEndTime: {3})", serverName, isOnline, lastOfflineRequestStartTime, lastOfflineRequestEndTime, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcGetServerComponentStatusImpl.cs", 162);
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		public const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.GetServerComponentStatus;

		private static TracingContext traceContext = TracingContext.Default;

		[Serializable]
		internal class Request
		{
			public string ServerComponentName { get; set; }

			public DateTime QueryStartTime { get; set; }

			public DateTime QueryEndTime { get; set; }
		}

		[Serializable]
		internal class Reply
		{
			public bool IsOnline { get; set; }

			public DateTime LastOfflineRequestStartTime { get; set; }

			public DateTime LastOfflineRequestEndTime { get; set; }
		}
	}
}
