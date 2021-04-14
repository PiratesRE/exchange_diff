using System;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class RpcUpdateHealthStatusImpl
	{
		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcUpdateHealthStatusImpl.Request request = ActiveMonitoringGenericRpcHelper.ValidateAndGetAttachedRequest<RpcUpdateHealthStatusImpl.Request>(requestInfo, 1, 2);
			DateTime lastUpdateTime = MonitorResultCacheManager.Instance.UpdateHealthEntries(request.Definitions, request.Results, request.DefinitionHeadTimeStamp, request.DefinitionTailTimeStamp, request.IsFullUpdate);
			replyInfo = ActiveMonitoringGenericRpcHelper.PrepareServerReply(requestInfo, new RpcUpdateHealthStatusImpl.Reply
			{
				LastUpdateTime = lastUpdateTime
			}, 1, 2);
		}

		public static DateTime SendRequest(string serverName, RpcUpdateHealthStatusImpl.RpcShortMonitorDefinitionEntry[] monitorDefinitions, RpcUpdateHealthStatusImpl.RpcShortMonitorResultEntry[] monitorResults, DateTime definitionHeadTime, DateTime definitionTailTime, bool isFullUpdate, int timeoutInMSec = 30000)
		{
			WTFDiagnostics.TraceDebug<int, int, DateTime, DateTime, bool>(ExTraceGlobals.GenericRpcTracer, RpcUpdateHealthStatusImpl.traceContext, "UpdateHealthStatus.SendRequest: DefinitionsCount:{0} ResultsCount:{1} HeadTime:{2} TailTime:{3} IsFullUpdate:{4}", monitorDefinitions.Length, monitorResults.Length, definitionHeadTime, definitionTailTime, isFullUpdate, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcUpdateHealthStatusImpl.cs", 101);
			RpcGenericRequestInfo rpcGenericRequestInfo = ActiveMonitoringGenericRpcHelper.PrepareClientRequest(new RpcUpdateHealthStatusImpl.Request
			{
				Definitions = monitorDefinitions,
				Results = monitorResults,
				DefinitionHeadTimeStamp = definitionHeadTime,
				DefinitionTailTimeStamp = definitionTailTime,
				IsFullUpdate = isFullUpdate
			}, ActiveMonitoringGenericRpcCommandId.UpdateHealthStatus, 1, 2);
			WTFDiagnostics.TraceDebug<int>(ExTraceGlobals.ResultCacheTracer, RpcUpdateHealthStatusImpl.traceContext, "Invoking RpcUpdateHealthStatus rpc with {0} bytes of data", rpcGenericRequestInfo.AttachedData.Length, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcUpdateHealthStatusImpl.cs", 127);
			RpcUpdateHealthStatusImpl.Reply reply = ActiveMonitoringGenericRpcHelper.RunRpcAndGetReply<RpcUpdateHealthStatusImpl.Reply>(rpcGenericRequestInfo, serverName, timeoutInMSec);
			DateTime lastUpdateTime = reply.LastUpdateTime;
			WTFDiagnostics.TraceDebug<string, DateTime>(ExTraceGlobals.GenericRpcTracer, RpcUpdateHealthStatusImpl.traceContext, "RpcUpdateHealthStatusImpl.SendRequest() returned. (serverName:{0} LastUpdateTime: {1})", serverName, lastUpdateTime, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Local\\Rpc\\RpcUpdateHealthStatusImpl.cs", 141);
			return lastUpdateTime;
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 2;

		public const ActiveMonitoringGenericRpcCommandId CommandCode = ActiveMonitoringGenericRpcCommandId.UpdateHealthStatus;

		private static TracingContext traceContext = TracingContext.Default;

		[Serializable]
		internal class Request
		{
			public RpcUpdateHealthStatusImpl.RpcShortMonitorDefinitionEntry[] Definitions { get; set; }

			public RpcUpdateHealthStatusImpl.RpcShortMonitorResultEntry[] Results { get; set; }

			public DateTime DefinitionHeadTimeStamp { get; set; }

			public DateTime DefinitionTailTimeStamp { get; set; }

			public bool IsFullUpdate { get; set; }
		}

		[Serializable]
		internal class Reply
		{
			public DateTime LastUpdateTime { get; set; }
		}

		[Serializable]
		internal class RpcShortMonitorDefinitionEntry
		{
			public RpcShortMonitorDefinitionEntry(MonitorDefinition definition)
			{
				this.Id = definition.Id;
				this.Name = definition.Name;
				this.TargetResource = definition.TargetResource;
				this.Description = string.Empty;
				this.IsHaImpacting = definition.IsHaImpacting;
				this.RecurranceInterval = definition.RecurrenceIntervalSeconds;
				this.TimeoutSeconds = definition.TimeoutSeconds;
				this.CreatedTimeUtc = definition.CreatedTime.ToUniversalTime();
				this.HealthSetName = definition.Component.Name;
				this.HealthSetDescription = string.Empty;
				this.HealthGroupName = definition.Component.HealthGroup.ToString();
				this.ServerComponentName = definition.Component.ServerComponent.ToString();
				this.Enabled = definition.Enabled;
				this.ServicePriority = definition.ServicePriority;
			}

			public int Id { get; private set; }

			public string Name { get; private set; }

			public string TargetResource { get; private set; }

			public string Description { get; private set; }

			public bool IsHaImpacting { get; private set; }

			public int RecurranceInterval { get; private set; }

			public DateTime CreatedTimeUtc { get; private set; }

			public DateTime CreatedTime
			{
				get
				{
					if (this.createdTimeLocal == null)
					{
						this.createdTimeLocal = new DateTime?(this.CreatedTimeUtc.ToLocalTime());
					}
					return this.createdTimeLocal.Value;
				}
			}

			public string HealthSetName { get; private set; }

			public string HealthSetDescription { get; private set; }

			public string HealthGroupName { get; private set; }

			public string ServerComponentName { get; private set; }

			public int TimeoutSeconds { get; private set; }

			public bool Enabled { get; private set; }

			public int ServicePriority { get; private set; }

			private DateTime? createdTimeLocal;
		}

		[Serializable]
		internal class RpcShortMonitorResultEntry
		{
			public RpcShortMonitorResultEntry(MonitorResult result)
			{
				this.FirstAlertObservedTimeUtc = this.GetUniversalTime(result.FirstAlertObservedTime);
				this.LastTransitionTimeUtc = this.GetUniversalTime(result.HealthStateChangedTime);
				this.LastExecutionTimeUtc = result.ExecutionEndTime.ToUniversalTime();
				this.LastExecutionResult = result.ResultType.ToString();
				this.ResultId = result.ResultId;
				this.WorkItemId = result.WorkItemId;
				this.IsAlert = result.IsAlert;
				this.Error = result.Error;
				this.Exception = result.Exception;
				this.IsNotified = result.IsNotified;
				this.LastFailedProbeId = result.LastFailedProbeId;
				this.LastFailedProbeResultId = result.LastFailedProbeResultId;
			}

			private DateTime GetUniversalTime(DateTime? time)
			{
				return ((time != null) ? time.Value : DateTime.MinValue).ToUniversalTime();
			}

			public DateTime FirstAlertObservedTimeUtc { get; private set; }

			public DateTime FirstAlertObservedTime
			{
				get
				{
					if (this.firstAlertObservedTimeLocal == null)
					{
						this.firstAlertObservedTimeLocal = new DateTime?(this.FirstAlertObservedTimeUtc.ToLocalTime());
					}
					return this.firstAlertObservedTimeLocal.Value;
				}
			}

			public DateTime LastTransitionTimeUtc { get; private set; }

			public DateTime LastTransitionTime
			{
				get
				{
					if (this.lastTransitionTimeLocal == null)
					{
						this.lastTransitionTimeLocal = new DateTime?(this.LastTransitionTimeUtc.ToLocalTime());
					}
					return this.lastTransitionTimeLocal.Value;
				}
			}

			public DateTime LastExecutionTimeUtc { get; private set; }

			public DateTime LastExecutionTime
			{
				get
				{
					if (this.lastExecutionTimeLocal == null)
					{
						this.lastExecutionTimeLocal = new DateTime?(this.LastExecutionTimeUtc.ToLocalTime());
					}
					return this.lastExecutionTimeLocal.Value;
				}
			}

			public string LastExecutionResult { get; private set; }

			public int ResultId { get; private set; }

			public int WorkItemId { get; private set; }

			public bool IsAlert { get; private set; }

			public string Error { get; private set; }

			public string Exception { get; private set; }

			public bool IsNotified { get; private set; }

			public int LastFailedProbeId { get; private set; }

			public int LastFailedProbeResultId { get; private set; }

			private DateTime? firstAlertObservedTimeLocal;

			public DateTime? lastTransitionTimeLocal;

			public DateTime? lastExecutionTimeLocal;
		}
	}
}
