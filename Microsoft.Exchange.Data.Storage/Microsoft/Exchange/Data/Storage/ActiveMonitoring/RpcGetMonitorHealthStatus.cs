using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.ActiveMonitoring;

namespace Microsoft.Exchange.Data.Storage.ActiveMonitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RpcGetMonitorHealthStatus
	{
		public static List<RpcGetMonitorHealthStatus.RpcMonitorHealthEntry> Invoke(string serverName, int timeoutInMSec = 30000)
		{
			return RpcGetMonitorHealthStatus.Invoke(serverName, null, timeoutInMSec);
		}

		public static List<RpcGetMonitorHealthStatus.RpcMonitorHealthEntry> Invoke(string serverName, string healthSetName, int timeoutInMSec = 30000)
		{
			RpcGetMonitorHealthStatus.Request attachedRequest = new RpcGetMonitorHealthStatus.Request(healthSetName);
			RpcGenericRequestInfo requestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(attachedRequest, ActiveMonitoringGenericRpcCommandId.GetMonitorHealthStatus, 1, 2);
			RpcGetMonitorHealthStatus.Reply reply = ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcGetMonitorHealthStatus.Reply>(requestInfo, serverName, timeoutInMSec);
			return reply.HealthEntries;
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 2;

		public const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.GetMonitorHealthStatus;

		[Serializable]
		public class Request
		{
			public Request(string healthSetName)
			{
				this.HealthSetName = healthSetName;
			}

			public string HealthSetName { get; set; }
		}

		[Serializable]
		public class Reply
		{
			public List<RpcGetMonitorHealthStatus.RpcMonitorHealthEntry> HealthEntries { get; set; }
		}

		[Serializable]
		public class RpcMonitorHealthEntry
		{
			public string Name { get; set; }

			public string TargetResource { get; set; }

			public string Description { get; set; }

			public bool IsHaImpacting { get; set; }

			public int RecurranceInterval { get; set; }

			public DateTime DefinitionCreatedTime { get; set; }

			public string HealthSetName { get; set; }

			public string HealthSetDescription { get; set; }

			public string HealthGroupName { get; set; }

			public string ServerComponentName { get; set; }

			public string AlertValue { get; set; }

			public DateTime FirstAlertObservedTime { get; set; }

			public DateTime LastTransitionTime { get; set; }

			public DateTime LastExecutionTime { get; set; }

			public string LastExecutionResult { get; set; }

			public string CurrentHealthSetState { get; set; }

			public int ResultId { get; set; }

			public int WorkItemId { get; set; }

			public int TimeoutSeconds { get; set; }

			public bool IsStale { get; set; }

			public string Error { get; set; }

			public string Exception { get; set; }

			public bool IsNotified { get; set; }

			public int LastFailedProbeId { get; set; }

			public int LastFailedProbeResultId { get; set; }

			public int ServicePriority { get; set; }
		}
	}
}
