using System;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class RpcGetThrottlingStatisticsImpl
	{
		public static RpcGetThrottlingStatisticsImpl.ThrottlingStatistics SendRequest(string serverName, RecoveryActionId actionId, string resourceName, int maxAllowedInOneHour, int maxAllowedInOneDay, bool isStopSearchWhenLimitExceeds = false, bool isCountFailedActions = false, int timeoutInMsec = 30000)
		{
			RpcGetThrottlingStatisticsImpl.Request attachedRequest = new RpcGetThrottlingStatisticsImpl.Request
			{
				ActionId = actionId,
				ResourceName = resourceName,
				MaxAllowedInOneHour = maxAllowedInOneHour,
				MaxAllowedInOneDay = maxAllowedInOneDay,
				IsStopSearchWhenLimitExceeds = isStopSearchWhenLimitExceeds,
				IsCountFailedActions = isCountFailedActions
			};
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(attachedRequest, ActiveMonitoringGenericRpcCommandId.GetThrottlingStatistics, 1, 0);
			return ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcGetThrottlingStatisticsImpl.ThrottlingStatistics>(requestInfo, serverName, timeoutInMsec);
		}

		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcGetThrottlingStatisticsImpl.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcGetThrottlingStatisticsImpl.Request>(requestInfo, 1, 0);
			RpcGetThrottlingStatisticsImpl.ThrottlingStatistics throttlingStatistics = RecoveryActionsRepository.Instance.GetThrottlingStatistics(request.ActionId, request.ResourceName, request.MaxAllowedInOneHour, request.MaxAllowedInOneDay, false, true);
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, throttlingStatistics, 1, 0);
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		internal const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.GetThrottlingStatistics;

		[Serializable]
		public class ThrottlingStatistics
		{
			public string ServerName { get; set; }

			public int TotalEntriesSearched { get; set; }

			public int NumberOfActionsInOneHour { get; set; }

			public RecoveryActionHelper.RecoveryActionEntrySerializable EntryExceedingOneHourLimit { get; set; }

			public int NumberOfActionsInOneDay { get; set; }

			public RecoveryActionHelper.RecoveryActionEntrySerializable EntryExceedingOneDayLimit { get; set; }

			public RecoveryActionHelper.RecoveryActionEntrySerializable MostRecentEntry { get; set; }

			public RpcSetThrottlingInProgressImpl.ThrottlingProgressInfo ThrottleProgressInfo { get; set; }

			public bool IsThrottlingInProgress { get; set; }

			public bool IsRecoveryInProgress { get; set; }

			public DateTime HostProcessStartTime
			{
				get
				{
					return this.HostProcessStartTimeUtc.ToLocalTime();
				}
				set
				{
					this.HostProcessStartTimeUtc = value.ToUniversalTime();
				}
			}

			public DateTime HostProcessStartTimeUtc { get; set; }

			public DateTime WorkerProcessStartTime
			{
				get
				{
					return this.WorkerProcessStartTimeUtc.ToLocalTime();
				}
				set
				{
					this.WorkerProcessStartTimeUtc = value.ToUniversalTime();
				}
			}

			public DateTime WorkerProcessStartTimeUtc { get; set; }

			public DateTime SystemBootTime
			{
				get
				{
					return this.SystemBootTimeUtc.ToLocalTime();
				}
				set
				{
					this.SystemBootTimeUtc = value.ToUniversalTime();
				}
			}

			public DateTime SystemBootTimeUtc { get; set; }
		}

		[Serializable]
		internal class Request
		{
			public RecoveryActionId ActionId { get; set; }

			public string ResourceName { get; set; }

			public int MaxAllowedInOneHour { get; set; }

			public int MaxAllowedInOneDay { get; set; }

			public bool IsStopSearchWhenLimitExceeds { get; set; }

			public bool IsCountFailedActions { get; set; }
		}
	}
}
