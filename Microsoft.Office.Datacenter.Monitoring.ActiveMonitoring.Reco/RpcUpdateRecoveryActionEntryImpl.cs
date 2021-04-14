using System;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public static class RpcUpdateRecoveryActionEntryImpl
	{
		public static void SendRequest(string serverName, RecoveryActionEntry entry, int timeoutInMsec = 30000)
		{
			RpcUpdateRecoveryActionEntryImpl.Request attachedRequest = new RpcUpdateRecoveryActionEntryImpl.Request
			{
				Entry = RecoveryActionHelper.CreateSerializableRecoveryActionEntry(entry)
			};
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(attachedRequest, ActiveMonitoringGenericRpcCommandId.UpdateRecoveryActionEntry, 1, 0);
			ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcUpdateRecoveryActionEntryImpl.Reply>(requestInfo, serverName, timeoutInMsec);
		}

		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcUpdateRecoveryActionEntryImpl.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcUpdateRecoveryActionEntryImpl.Request>(requestInfo, 1, 0);
			RecoveryActionsRepository.Instance.AddEntry(request.Entry, true, false);
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, new RpcUpdateRecoveryActionEntryImpl.Reply(), 1, 0);
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		internal const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.UpdateRecoveryActionEntry;

		[Serializable]
		internal class Request
		{
			public RecoveryActionHelper.RecoveryActionEntrySerializable Entry { get; set; }
		}

		[Serializable]
		internal class Reply
		{
		}
	}
}
