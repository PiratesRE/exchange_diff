using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.TenantMonitoring
{
	internal class IntervalCounterInstance
	{
		public DateTime LastUpdateTime
		{
			get
			{
				return this.lastUpdateTime;
			}
		}

		public void Increment(CounterType counterType)
		{
			Interlocked.Increment(ref this.values[(int)counterType]);
			this.lastUpdateTime = DateTime.UtcNow;
		}

		private void UpdateCounter(ExPerformanceCounter counter, CounterType counterType)
		{
			int num = this.values[(int)counterType];
			int num2 = (int)counter.RawValue;
			counter.IncrementBy((long)(num - num2));
			Interlocked.Exchange(ref this.values[(int)counterType], 0);
		}

		internal void CalculateIntervalDataAndUpdateCounters(string instanceName)
		{
			MSExchangeTenantMonitoringInstance instance = MSExchangeTenantMonitoring.GetInstance(instanceName);
			this.UpdateCounter(instance.MSExchangeHomeSiteLocationAttempts, CounterType.HomeSiteLocationAttempts);
			this.UpdateCounter(instance.MSExchangeHomeSiteLocationSuccesses, CounterType.HomeSiteLocationSuccesses);
			this.UpdateCounter(instance.MSExchangePartnerHomeSiteLocationAttempts, CounterType.PartnerHomeSiteLocationAttempts);
			this.UpdateCounter(instance.MSExchangePartnerHomeSiteLocationSuccesses, CounterType.PartnerHomeSiteLocationSuccesses);
			this.UpdateCounter(instance.MSExchangeRemotePoweshellUserAuthorizationAttempts, CounterType.RemotePoweshellUserAuthorizationAttempts);
			this.UpdateCounter(instance.MSExchangeRemotePoweshellUserAuthorizationSuccesses, CounterType.RemotePoweshellUserAuthorizationSuccesses);
			this.UpdateCounter(instance.MSExchangeRemotePoweshellSessionCreationAttempts, CounterType.RemotePoweshellSessionCreationAttempts);
			this.UpdateCounter(instance.MSExchangeRemotePoweshellSessionCreationSuccesses, CounterType.RemotePoweshellSessionCreationSuccesses);
			this.UpdateCounter(instance.MSExchangeRemotePoweshellPartnerAuthorizationAttempts, CounterType.RemotePoweshellPartnerAuthorizationAttempts);
			this.UpdateCounter(instance.MSExchangeRemotePoweshellPartnerAuthorizationSuccesses, CounterType.RemotePoweshellPartnerAuthorizationSuccesses);
			this.UpdateCounter(instance.MSExchangeRemotePoweshellPartnerSessionCreationAttempts, CounterType.RemotePoweshellPartnerSessionCreationAttempts);
			this.UpdateCounter(instance.MSExchangeRemotePoweshellPartnerSessionCreationSuccesses, CounterType.RemotePoweshellPartnerSessionCreationSuccesses);
			this.UpdateCounter(instance.MSExchangeECPSessionCreationAttempts, CounterType.ECPSessionCreationAttempts);
			this.UpdateCounter(instance.MSExchangeECPSessionCreationSuccesses, CounterType.ECPSessionCreationSuccesses);
			this.UpdateCounter(instance.MSExchangeECPRedirectionSuccesses, CounterType.ECPRedirectionSuccesses);
			this.UpdateCounter(instance.MSExchangeNewMailboxAttempts, CounterType.NewMailboxAttempts);
			this.UpdateCounter(instance.MSExchangeNewMailboxSuccesses, CounterType.NewMailboxSuccesses);
			this.UpdateCounter(instance.MSExchangeNewMailboxIterationAttempts, CounterType.NewMailboxIterationAttempts);
			this.UpdateCounter(instance.MSExchangeNewMailboxIterationSuccesses, CounterType.NewMailboxIterationSuccesses);
			this.UpdateCounter(instance.MSExchangeNewOrganizationAttempts, CounterType.NewOrganizationAttempts);
			this.UpdateCounter(instance.MSExchangeNewOrganizationSuccesses, CounterType.NewOrganizationSuccesses);
			this.UpdateCounter(instance.MSExchangeNewOrganizationIterationAttempts, CounterType.NewOrganizationIterationAttempts);
			this.UpdateCounter(instance.MSExchangeNewOrganizationIterationSuccesses, CounterType.NewOrganizationIterationSuccesses);
			this.UpdateCounter(instance.MSExchangeRemoveOrganizationAttempts, CounterType.RemoveOrganizationAttempts);
			this.UpdateCounter(instance.MSExchangeRemoveOrganizationSuccesses, CounterType.RemoveOrganizationSuccesses);
			this.UpdateCounter(instance.MSExchangeRemoveOrganizationIterationAttempts, CounterType.RemoveOrganizationIterationAttempts);
			this.UpdateCounter(instance.MSExchangeRemoveOrganizationIterationSuccesses, CounterType.RemoveOrganizationIterationSuccesses);
			this.UpdateCounter(instance.MSExchangeAddSecondaryDomainAttempts, CounterType.AddSecondaryDomainAttempts);
			this.UpdateCounter(instance.MSExchangeAddSecondaryDomainSuccesses, CounterType.AddSecondaryDomainSuccesses);
			this.UpdateCounter(instance.MSExchangeAddSecondaryDomainIterationAttempts, CounterType.AddSecondaryDomainIterationAttempts);
			this.UpdateCounter(instance.MSExchangeAddSecondaryDomainIterationSuccesses, CounterType.AddSecondaryDomainIterationSuccesses);
			this.UpdateCounter(instance.MSExchangeRemoveSecondaryDomainAttempts, CounterType.RemoveSecondaryDomainAttempts);
			this.UpdateCounter(instance.MSExchangeRemoveSecondaryDomainSuccesses, CounterType.RemoveSecondaryDomainSuccesses);
			this.UpdateCounter(instance.MSExchangeRemoveSecondaryDomainIterationAttempts, CounterType.RemoveSecondaryDomainIterationAttempts);
			this.UpdateCounter(instance.MSExchangeRemoveSecondaryDomainIterationSuccesses, CounterType.RemoveSecondaryDomainIterationSuccesses);
			this.UpdateCounter(instance.MSExchangeStartOrganizationPilotAttempts, CounterType.StartOrganizationPilotAttempts);
			this.UpdateCounter(instance.MSExchangeStartOrganizationPilotSuccesses, CounterType.StartOrganizationPilotSuccesses);
			this.UpdateCounter(instance.MSExchangeStartOrganizationPilotIterationAttempts, CounterType.StartOrganizationPilotIterationAttempts);
			this.UpdateCounter(instance.MSExchangeStartOrganizationPilotIterationSuccesses, CounterType.StartOrganizationPilotIterationSuccesses);
			this.UpdateCounter(instance.MSExchangeStartOrganizationUpgradeAttempts, CounterType.StartOrganizationUpgradeAttempts);
			this.UpdateCounter(instance.MSExchangeStartOrganizationUpgradeSuccesses, CounterType.StartOrganizationUpgradeSuccesses);
			this.UpdateCounter(instance.MSExchangeStartOrganizationUpgradeIterationAttempts, CounterType.StartOrganizationUpgradeIterationAttempts);
			this.UpdateCounter(instance.MSExchangeStartOrganizationUpgradeIterationSuccesses, CounterType.StartOrganizationUpgradeIterationSuccesses);
			this.UpdateCounter(instance.MSExchangeCompleteOrganizationUpgradeAttempts, CounterType.CompleteOrganizationUpgradeAttempts);
			this.UpdateCounter(instance.MSExchangeCompleteOrganizationUpgradeSuccesses, CounterType.CompleteOrganizationUpgradeSuccesses);
			this.UpdateCounter(instance.MSExchangeCompleteOrganizationUpgradeIterationAttempts, CounterType.CompleteOrganizationUpgradeIterationAttempts);
			this.UpdateCounter(instance.MSExchangeCompleteOrganizationUpgradeIterationSuccesses, CounterType.CompleteOrganizationUpgradeIterationSuccesses);
			this.UpdateCounter(instance.MSExchangeGetManagementEndpointAttempts, CounterType.GetManagementEndpointAttempts);
			this.UpdateCounter(instance.MSExchangeGetManagementEndpointSuccesses, CounterType.GetManagementEndpointSuccesses);
			this.UpdateCounter(instance.MSExchangeGetManagementEndpointIterationAttempts, CounterType.GetManagementEndpointIterationAttempts);
			this.UpdateCounter(instance.MSExchangeGetManagementEndpointIterationSuccesses, CounterType.GetManagementEndpointIterationSuccesses);
			this.UpdateCounter(instance.MSExchangeCmdletAttempts, CounterType.CmdletAttempts);
			this.UpdateCounter(instance.MSExchangeCmdletSuccesses, CounterType.CmdletSuccesses);
			this.UpdateCounter(instance.MSExchangeCmdletIterationAttempts, CounterType.CmdletIterationAttempts);
			this.UpdateCounter(instance.MSExchangeCmdletIterationSuccesses, CounterType.CmdletIterationSuccesses);
		}

		private int[] values = new int[55];

		private DateTime lastUpdateTime;
	}
}
