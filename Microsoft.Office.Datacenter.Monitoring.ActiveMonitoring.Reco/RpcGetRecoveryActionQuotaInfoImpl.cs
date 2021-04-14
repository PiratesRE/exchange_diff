using System;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public static class RpcGetRecoveryActionQuotaInfoImpl
	{
		public static RecoveryActionHelper.RecoveryActionQuotaInfo SendRequest(string serverName, RecoveryActionId actionId, string resourceName, int maxAllowedQuota, DateTime queryStartTime, DateTime queryEndTime, int timeoutInMSec = 30000)
		{
			RpcGetRecoveryActionQuotaInfoImpl.Request attachedRequest = new RpcGetRecoveryActionQuotaInfoImpl.Request(actionId, resourceName, maxAllowedQuota, queryStartTime, queryEndTime, TimeSpan.FromMilliseconds((double)timeoutInMSec));
			WTFDiagnostics.TraceDebug<string, DateTime, DateTime>(ExTraceGlobals.GenericRpcTracer, RpcGetRecoveryActionQuotaInfoImpl.traceContext, "RpcGetRecoveryActionQuotaInfoImpl.SendRequest() called. (serverName:{0}, queryStartTime:{1}, queryEndTime: {2})", serverName, queryStartTime, queryEndTime, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\Rpc\\RpcGetRecoveryActionQuotaInfoImpl.cs", 78);
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(attachedRequest, ActiveMonitoringGenericRpcCommandId.GetRecoveryActionQuotaInfo, 1, 0);
			RpcGetRecoveryActionQuotaInfoImpl.Reply reply = ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcGetRecoveryActionQuotaInfoImpl.Reply>(requestInfo, serverName, timeoutInMSec);
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.GenericRpcTracer, RpcGetRecoveryActionQuotaInfoImpl.traceContext, "RpcGetRecoveryActionQuotaInfoImpl.SendRequest() returned. (serverName:{0})", serverName, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\Rpc\\RpcGetRecoveryActionQuotaInfoImpl.cs", 99);
			return reply.QuotaInfo;
		}

		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcGetRecoveryActionQuotaInfoImpl.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcGetRecoveryActionQuotaInfoImpl.Request>(requestInfo, 1, 0);
			RecoveryActionHelper.RecoveryActionQuotaInfo recoveryActionQuotaInfo = RecoveryActionHelper.GetRecoveryActionQuotaInfo(request.ActionId, request.ResourceName, request.MaxAllowedQuota, request.QueryStartTime, request.QueryEndTime, request.Timeout);
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, new RpcGetRecoveryActionQuotaInfoImpl.Reply
			{
				QuotaInfo = recoveryActionQuotaInfo
			}, 1, 0);
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		internal const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.GetRecoveryActionQuotaInfo;

		private static TracingContext traceContext = TracingContext.Default;

		[Serializable]
		internal class Request
		{
			public Request(RecoveryActionId actionId, string resourceName, int maxAllowedQuota, DateTime queryStartTime, DateTime queryEndTime, TimeSpan timeout)
			{
				this.ActionId = actionId;
				this.ResourceName = resourceName;
				this.MaxAllowedQuota = maxAllowedQuota;
				this.QueryStartTimeUtc = queryStartTime.ToUniversalTime();
				this.QueryEndTimeUtc = queryEndTime.ToUniversalTime();
				this.Timeout = timeout;
			}

			public RecoveryActionId ActionId { get; set; }

			public string ResourceName { get; set; }

			public int MaxAllowedQuota { get; private set; }

			public DateTime QueryStartTimeUtc { get; set; }

			public DateTime QueryStartTime
			{
				get
				{
					return this.QueryStartTimeUtc.ToLocalTime();
				}
			}

			public DateTime QueryEndTimeUtc { get; set; }

			public DateTime QueryEndTime
			{
				get
				{
					return this.QueryEndTimeUtc.ToLocalTime();
				}
			}

			public TimeSpan Timeout { get; set; }
		}

		[Serializable]
		internal class Reply
		{
			public RecoveryActionHelper.RecoveryActionQuotaInfo QuotaInfo { get; set; }
		}
	}
}
