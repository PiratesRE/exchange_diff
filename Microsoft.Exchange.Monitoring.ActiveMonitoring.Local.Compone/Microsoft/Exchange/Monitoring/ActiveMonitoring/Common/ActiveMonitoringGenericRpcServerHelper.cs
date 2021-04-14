using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class ActiveMonitoringGenericRpcServerHelper
	{
		public static void Dispatch(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcGenericReplyInfo tmpReplyInfo = null;
			ActiveMonitoringGenericRpcServerHelper.WrapExceptionsWithActiveMonitoringServerException(delegate
			{
				ActiveMonitoringGenericRpcServerHelper.DispatchInternal(requestInfo, ref tmpReplyInfo);
			});
			if (tmpReplyInfo != null)
			{
				replyInfo = tmpReplyInfo;
			}
		}

		private static void DispatchInternal(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			switch (requestInfo.CommandId)
			{
			case 1:
				RpcGetLastBugcheckStatusImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			case 2:
				RpcGetRecoveryActionStatusImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			case 3:
				ActiveMonitoringGenericRpcServerHelper.RpcServerGetMonitorHealthStatus(requestInfo, ref replyInfo);
				return;
			case 4:
				ActiveMonitoringGenericRpcServerHelper.RpcServerServerMonitor(requestInfo, ref replyInfo);
				return;
			case 5:
				RpcRequestObserverImpl.HandleRequestObserver(requestInfo, ref replyInfo);
				return;
			case 6:
				RpcCancelObserverImpl.HandleCancelObserver(requestInfo, ref replyInfo);
				return;
			case 7:
				RpcObserverHeartbeatImpl.HandleObserverHeartbeat(requestInfo, ref replyInfo);
				return;
			case 8:
				RpcUpdateHealthStatusImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			case 9:
				RpcGetServerComponentStatusImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			case 10:
				RpcGetMonitoringItemIdentityImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			case 11:
				RpcGetMonitoringItemHelpImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			case 12:
				RpcInvokeMonitoringProbeImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			case 13:
				RpcGetRecoveryActionQuotaInfoImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			case 14:
				RpcLogCrimsonEventImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			case 15:
				RpcLockHealthSetEscalationStateIfRequiredImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			case 16:
				RpcSetHealthSetEscalationStateImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			case 17:
				RpcGetThrottlingStatisticsImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			case 18:
				RpcUpdateRecoveryActionEntryImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			case 19:
				RpcSetThrottlingInProgressImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			case 20:
				RpcSetWorkerProcessInfoImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			case 21:
				RpcGetCrimsonEventImpl.HandleRequest(requestInfo, ref replyInfo);
				return;
			default:
				throw new ActiveMonitoringUnknownGenericRpcCommandException(requestInfo.ServerVersion, ActiveMonitoringGenericRpcHelper.LocalServerVersion, requestInfo.CommandId);
			}
		}

		public static void RpcServerGetMonitorHealthStatus(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcGetMonitorHealthStatus.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcGetMonitorHealthStatus.Request>(requestInfo, 1, 2);
			byte[] cachedSerializedMonitorHealthEntries = MonitorResultCacheManager.Instance.GetCachedSerializedMonitorHealthEntries(request.HealthSetName);
			replyInfo = new RpcGenericReplyInfo(ActiveMonitoringGenericRpcHelper.LocalServerVersion, requestInfo.CommandId, 1, 2, cachedSerializedMonitorHealthEntries);
		}

		public static void RpcServerServerMonitor(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcSetServerMonitor.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcSetServerMonitor.Request>(requestInfo, 1, 0);
			if (request.IsRepairing != null)
			{
				MonitorStateHelper.SetMonitorIsRepairingFlag(request.MonitorName, request.TargetResource, request.IsRepairing.Value);
			}
			RpcSetServerMonitor.Reply attachedReply = new RpcSetServerMonitor.Reply();
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, attachedReply, 1, 0);
		}

		public static void WrapExceptionsWithActiveMonitoringServerException(Action action)
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				if (ex is ActiveMonitoringServerException || ex is ActiveMonitoringServerTransientException)
				{
					throw;
				}
				throw new ActiveMonitoringServerException(ex.Message, ex);
			}
		}
	}
}
