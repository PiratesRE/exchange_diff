using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class CoordinatedOfflineAction : CoordinatedRecoveryAction
	{
		internal CoordinatedOfflineAction(RecoveryActionId actionId, TimeSpan duration, ServerComponentEnum serverComponent, string requester, int minimumRequiredTobeInReadyState, string[] servers) : base(actionId, requester, minimumRequiredTobeInReadyState, 1, servers)
		{
			this.duration = duration;
			this.serverComponent = serverComponent;
		}

		internal void InvokeOfflineOnMajority(TimeSpan arbitrationTimeout)
		{
			base.Execute(arbitrationTimeout, delegate(CoordinatedRecoveryAction.ResourceAvailabilityStatistics stats)
			{
				ServerComponentStateManager.SetOffline(this.serverComponent);
			});
		}

		protected override ResourceAvailabilityStatus RunCheck(string serverName, out string debugInfo)
		{
			debugInfo = string.Empty;
			if (RecoveryActionHelper.IsLocalServerName(serverName))
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.RecoveryActionTracer, base.TraceContext, "Avoding rpc loop back to local machine since we are already in the Arbitration phase, return directly status of 'Arbitrating'", null, "RunCheck", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\RecoveryAction\\CoordinatedOfflineAction.cs", 95);
				return ResourceAvailabilityStatus.Arbitrating;
			}
			DateTime localTime = ExDateTime.Now.LocalTime;
			DateTime queryStartTime = localTime - this.duration;
			bool flag = false;
			DateTime dateTime;
			DateTime dateTime2;
			RpcGetServerComponentStatusImpl.SendRequest(serverName, this.serverComponent, queryStartTime, localTime, out flag, out dateTime, out dateTime2, 30000);
			ResourceAvailabilityStatus result;
			if (dateTime2 < ExDateTime.Now.LocalTime)
			{
				if (flag)
				{
					result = ResourceAvailabilityStatus.Ready;
				}
				else
				{
					result = ResourceAvailabilityStatus.Offline;
				}
			}
			else
			{
				result = ResourceAvailabilityStatus.Arbitrating;
			}
			debugInfo = string.Format("[serverComponent={0}, isOnline = {1}, lastOfflineRequestStartTime = {2}, lastOfflineRequestEndTime = {3}]", new object[]
			{
				this.serverComponent,
				flag,
				dateTime,
				dateTime2
			});
			return result;
		}

		private readonly TimeSpan duration;

		private ServerComponentEnum serverComponent;
	}
}
