using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public static class RpcGetRecoveryActionStatusImpl
	{
		public static List<RecoveryActionEntry> SendRequest(string serverName, RecoveryActionId actionId, string resourceName, RecoveryActionState state, RecoveryActionResult result, DateTime startTime, DateTime endTime, string xpathQueryString = null, int maxCount = -1, int timeoutInMsec = 30000)
		{
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(new RpcGetRecoveryActionStatusImpl.Request
			{
				ActionId = actionId,
				ResourceName = resourceName,
				State = state,
				Result = result,
				StartTime = startTime,
				EndTime = endTime,
				MaxEntries = maxCount,
				XPathQuery = xpathQueryString
			}, ActiveMonitoringGenericRpcCommandId.GetRecoveryActionStatus, 1, 0);
			RpcGetRecoveryActionStatusImpl.Reply reply = ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcGetRecoveryActionStatusImpl.Reply>(requestInfo, serverName, timeoutInMsec);
			return reply.RecoveryActionEntries;
		}

		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			bool isMoreEntriesAvailable = false;
			RpcGetRecoveryActionStatusImpl.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcGetRecoveryActionStatusImpl.Request>(requestInfo, 1, 0);
			List<RecoveryActionEntry> recoveryActionEntries = RecoveryActionHelper.ReadEntries(request.ActionId, request.ResourceName, null, request.State, request.Result, request.StartTime, request.EndTime, out isMoreEntriesAvailable, request.XPathQuery, TimeSpan.MaxValue, request.MaxEntries);
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, new RpcGetRecoveryActionStatusImpl.Reply
			{
				IsMoreEntriesAvailable = isMoreEntriesAvailable,
				RecoveryActionEntries = recoveryActionEntries
			}, 1, 0);
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		internal const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.GetRecoveryActionStatus;

		[Serializable]
		internal class Request
		{
			public RecoveryActionId ActionId { get; set; }

			public string ResourceName { get; set; }

			public string InstanceId { get; set; }

			public RecoveryActionState State { get; set; }

			public RecoveryActionResult Result { get; set; }

			public DateTime StartTime { get; set; }

			public DateTime EndTime { get; set; }

			public string XPathQuery { get; set; }

			public int MaxEntries { get; set; }
		}

		[Serializable]
		internal class Reply
		{
			public bool IsMoreEntriesAvailable { get; set; }

			public List<RecoveryActionEntry> RecoveryActionEntries { get; set; }
		}
	}
}
