using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.TenantMonitoring
{
	internal sealed class MSExchangeTenantMonitoringInstance : PerformanceCounterInstance
	{
		internal MSExchangeTenantMonitoringInstance(string instanceName, MSExchangeTenantMonitoringInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTenantMonitoring")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.MSExchangeHomeSiteLocationAttempts = new ExPerformanceCounter(base.CategoryName, "Datacenter and Site Location Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeHomeSiteLocationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeHomeSiteLocationAttempts);
				this.MSExchangeHomeSiteLocationSuccesses = new ExPerformanceCounter(base.CategoryName, "Datacenter and Site Location Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeHomeSiteLocationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeHomeSiteLocationSuccesses);
				this.MSExchangePartnerHomeSiteLocationAttempts = new ExPerformanceCounter(base.CategoryName, "Partner Datacenter and Site Location Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangePartnerHomeSiteLocationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangePartnerHomeSiteLocationAttempts);
				this.MSExchangePartnerHomeSiteLocationSuccesses = new ExPerformanceCounter(base.CategoryName, "Partner Datacenter and Site Location Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangePartnerHomeSiteLocationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangePartnerHomeSiteLocationSuccesses);
				this.MSExchangeRemotePoweshellUserAuthorizationAttempts = new ExPerformanceCounter(base.CategoryName, "Remote PowerShell Tenant User Authorization Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemotePoweshellUserAuthorizationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellUserAuthorizationAttempts);
				this.MSExchangeRemotePoweshellUserAuthorizationSuccesses = new ExPerformanceCounter(base.CategoryName, "Remote PoweSshell Tenant User Authorization Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemotePoweshellUserAuthorizationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellUserAuthorizationSuccesses);
				this.MSExchangeRemotePoweshellSessionCreationAttempts = new ExPerformanceCounter(base.CategoryName, "Remote PowerShell Tenant Session Creation Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemotePoweshellSessionCreationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellSessionCreationAttempts);
				this.MSExchangeRemotePoweshellSessionCreationSuccesses = new ExPerformanceCounter(base.CategoryName, "Remote PowerShell Tenant Session Creation Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemotePoweshellSessionCreationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellSessionCreationSuccesses);
				this.MSExchangeRemotePoweshellPartnerAuthorizationAttempts = new ExPerformanceCounter(base.CategoryName, "Remote PowerShell Partner Authorization Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemotePoweshellPartnerAuthorizationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellPartnerAuthorizationAttempts);
				this.MSExchangeRemotePoweshellPartnerAuthorizationSuccesses = new ExPerformanceCounter(base.CategoryName, "Remote Powershell Partner Authorization Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemotePoweshellPartnerAuthorizationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellPartnerAuthorizationSuccesses);
				this.MSExchangeRemotePoweshellPartnerSessionCreationAttempts = new ExPerformanceCounter(base.CategoryName, "Remote PowerShell Partner Session Creation Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemotePoweshellPartnerSessionCreationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellPartnerSessionCreationAttempts);
				this.MSExchangeRemotePoweshellPartnerSessionCreationSuccesses = new ExPerformanceCounter(base.CategoryName, "Remote PowerShell partner session creation successes per period.", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemotePoweshellPartnerSessionCreationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellPartnerSessionCreationSuccesses);
				this.MSExchangeECPSessionCreationAttempts = new ExPerformanceCounter(base.CategoryName, "Exchange Control Panel Session Creation Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeECPSessionCreationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeECPSessionCreationAttempts);
				this.MSExchangeECPSessionCreationSuccesses = new ExPerformanceCounter(base.CategoryName, "Exchange Control Panel Session Creation Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeECPSessionCreationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeECPSessionCreationSuccesses);
				this.MSExchangeECPRedirectionSuccesses = new ExPerformanceCounter(base.CategoryName, "ECP session redirection successes per period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeECPRedirectionSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeECPRedirectionSuccesses);
				this.MSExchangeNewMailboxAttempts = new ExPerformanceCounter(base.CategoryName, "NewMailbox Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeNewMailboxAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewMailboxAttempts);
				this.MSExchangeNewMailboxSuccesses = new ExPerformanceCounter(base.CategoryName, "NewMailbox Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeNewMailboxSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewMailboxSuccesses);
				this.MSExchangeNewMailboxIterationAttempts = new ExPerformanceCounter(base.CategoryName, "NewMailbox Iteration Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeNewMailboxIterationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewMailboxIterationAttempts);
				this.MSExchangeNewMailboxIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "NewMailbox Iteration Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeNewMailboxIterationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewMailboxIterationSuccesses);
				this.MSExchangeNewOrganizationAttempts = new ExPerformanceCounter(base.CategoryName, "NewOrganization Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeNewOrganizationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewOrganizationAttempts);
				this.MSExchangeNewOrganizationSuccesses = new ExPerformanceCounter(base.CategoryName, "NewOrganization Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeNewOrganizationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewOrganizationSuccesses);
				this.MSExchangeNewOrganizationIterationAttempts = new ExPerformanceCounter(base.CategoryName, "NewOrganization Iteration Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeNewOrganizationIterationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewOrganizationIterationAttempts);
				this.MSExchangeNewOrganizationIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "NewOrganization Iteration Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeNewOrganizationIterationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewOrganizationIterationSuccesses);
				this.MSExchangeRemoveOrganizationAttempts = new ExPerformanceCounter(base.CategoryName, "RemoveOrganization Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemoveOrganizationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveOrganizationAttempts);
				this.MSExchangeRemoveOrganizationSuccesses = new ExPerformanceCounter(base.CategoryName, "RemoveOrganization successes per period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemoveOrganizationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveOrganizationSuccesses);
				this.MSExchangeRemoveOrganizationIterationAttempts = new ExPerformanceCounter(base.CategoryName, "RemoveOrganization Iteration Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemoveOrganizationIterationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveOrganizationIterationAttempts);
				this.MSExchangeRemoveOrganizationIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "RemoveOrganization Iteration Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemoveOrganizationIterationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveOrganizationIterationSuccesses);
				this.MSExchangeAddSecondaryDomainAttempts = new ExPerformanceCounter(base.CategoryName, "AddSecondaryDomain Attempts Per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeAddSecondaryDomainAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeAddSecondaryDomainAttempts);
				this.MSExchangeAddSecondaryDomainSuccesses = new ExPerformanceCounter(base.CategoryName, "AddSecondaryDomain Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeAddSecondaryDomainSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeAddSecondaryDomainSuccesses);
				this.MSExchangeAddSecondaryDomainIterationAttempts = new ExPerformanceCounter(base.CategoryName, "AddSecondaryDomainIteration Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeAddSecondaryDomainIterationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeAddSecondaryDomainIterationAttempts);
				this.MSExchangeAddSecondaryDomainIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "AddSecondaryDomain Iteration Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeAddSecondaryDomainIterationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeAddSecondaryDomainIterationSuccesses);
				this.MSExchangeRemoveSecondaryDomainAttempts = new ExPerformanceCounter(base.CategoryName, "RemoveSecondaryDomain Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemoveSecondaryDomainAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveSecondaryDomainAttempts);
				this.MSExchangeRemoveSecondaryDomainSuccesses = new ExPerformanceCounter(base.CategoryName, "RemoveSecondaryDomain Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemoveSecondaryDomainSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveSecondaryDomainSuccesses);
				this.MSExchangeRemoveSecondaryDomainIterationAttempts = new ExPerformanceCounter(base.CategoryName, "RemoveSecondaryDomain Iteration Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemoveSecondaryDomainIterationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveSecondaryDomainIterationAttempts);
				this.MSExchangeRemoveSecondaryDomainIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "RemoveSecondaryDomain Iteration Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeRemoveSecondaryDomainIterationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveSecondaryDomainIterationSuccesses);
				this.MSExchangeStartOrganizationPilotAttempts = new ExPerformanceCounter(base.CategoryName, "StartOrganizationPilot Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeStartOrganizationPilotAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationPilotAttempts);
				this.MSExchangeStartOrganizationPilotSuccesses = new ExPerformanceCounter(base.CategoryName, "StartOrganizationPilot Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeStartOrganizationPilotSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationPilotSuccesses);
				this.MSExchangeStartOrganizationPilotIterationAttempts = new ExPerformanceCounter(base.CategoryName, "StartOrganizationPilot Iteration Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeStartOrganizationPilotIterationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationPilotIterationAttempts);
				this.MSExchangeStartOrganizationPilotIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "StartOrganizationPilot Iteration Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeStartOrganizationPilotIterationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationPilotIterationSuccesses);
				this.MSExchangeStartOrganizationUpgradeAttempts = new ExPerformanceCounter(base.CategoryName, "StartOrganizationUpgrade Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeStartOrganizationUpgradeAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationUpgradeAttempts);
				this.MSExchangeStartOrganizationUpgradeSuccesses = new ExPerformanceCounter(base.CategoryName, "StartOrganizationUpgrade Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeStartOrganizationUpgradeSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationUpgradeSuccesses);
				this.MSExchangeStartOrganizationUpgradeIterationAttempts = new ExPerformanceCounter(base.CategoryName, "StartOrganizationUpgrade Iteration Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeStartOrganizationUpgradeIterationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationUpgradeIterationAttempts);
				this.MSExchangeStartOrganizationUpgradeIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "StartOrganizationUpgrade Iteration Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeStartOrganizationUpgradeIterationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationUpgradeIterationSuccesses);
				this.MSExchangeCompleteOrganizationUpgradeAttempts = new ExPerformanceCounter(base.CategoryName, "CompleteOrganizationUpgrade Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeCompleteOrganizationUpgradeAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCompleteOrganizationUpgradeAttempts);
				this.MSExchangeCompleteOrganizationUpgradeSuccesses = new ExPerformanceCounter(base.CategoryName, "CompleteOrganizationUpgrade Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeCompleteOrganizationUpgradeSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCompleteOrganizationUpgradeSuccesses);
				this.MSExchangeCompleteOrganizationUpgradeIterationAttempts = new ExPerformanceCounter(base.CategoryName, "CompleteOrganizationUpgrade Iteration Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeCompleteOrganizationUpgradeIterationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCompleteOrganizationUpgradeIterationAttempts);
				this.MSExchangeCompleteOrganizationUpgradeIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "CompleteOrganizationUpgrade Iteration Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeCompleteOrganizationUpgradeIterationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCompleteOrganizationUpgradeIterationSuccesses);
				this.MSExchangeGetManagementEndpointAttempts = new ExPerformanceCounter(base.CategoryName, "GetManagementEndpoint Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeGetManagementEndpointAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeGetManagementEndpointAttempts);
				this.MSExchangeGetManagementEndpointSuccesses = new ExPerformanceCounter(base.CategoryName, "GetManagementEndpoint Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeGetManagementEndpointSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeGetManagementEndpointSuccesses);
				this.MSExchangeGetManagementEndpointIterationAttempts = new ExPerformanceCounter(base.CategoryName, "GetManagementEndpoint Iteration Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeGetManagementEndpointIterationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeGetManagementEndpointIterationAttempts);
				this.MSExchangeGetManagementEndpointIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "GetManagementEndpoint Iteration Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeGetManagementEndpointIterationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeGetManagementEndpointIterationSuccesses);
				this.MSExchangeCmdletAttempts = new ExPerformanceCounter(base.CategoryName, "Cmdlet attempts per period. This is only for cmdlets which are to be monitored.", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeCmdletAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCmdletAttempts);
				this.MSExchangeCmdletSuccesses = new ExPerformanceCounter(base.CategoryName, "Cmdlet Successes per Period. This is only for cmdlets which are to be monitored.", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeCmdletSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCmdletSuccesses);
				this.MSExchangeCmdletIterationAttempts = new ExPerformanceCounter(base.CategoryName, "Cmdlet Iteration Attempts per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeCmdletIterationAttempts, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCmdletIterationAttempts);
				this.MSExchangeCmdletIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "Cmdlet Iteration Successes per Period", instanceName, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MSExchangeCmdletIterationSuccesses, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCmdletIterationSuccesses);
				long num = this.MSExchangeHomeSiteLocationAttempts.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal MSExchangeTenantMonitoringInstance(string instanceName) : base(instanceName, "MSExchangeTenantMonitoring")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.MSExchangeHomeSiteLocationAttempts = new ExPerformanceCounter(base.CategoryName, "Datacenter and Site Location Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeHomeSiteLocationAttempts);
				this.MSExchangeHomeSiteLocationSuccesses = new ExPerformanceCounter(base.CategoryName, "Datacenter and Site Location Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeHomeSiteLocationSuccesses);
				this.MSExchangePartnerHomeSiteLocationAttempts = new ExPerformanceCounter(base.CategoryName, "Partner Datacenter and Site Location Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangePartnerHomeSiteLocationAttempts);
				this.MSExchangePartnerHomeSiteLocationSuccesses = new ExPerformanceCounter(base.CategoryName, "Partner Datacenter and Site Location Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangePartnerHomeSiteLocationSuccesses);
				this.MSExchangeRemotePoweshellUserAuthorizationAttempts = new ExPerformanceCounter(base.CategoryName, "Remote PowerShell Tenant User Authorization Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellUserAuthorizationAttempts);
				this.MSExchangeRemotePoweshellUserAuthorizationSuccesses = new ExPerformanceCounter(base.CategoryName, "Remote PoweSshell Tenant User Authorization Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellUserAuthorizationSuccesses);
				this.MSExchangeRemotePoweshellSessionCreationAttempts = new ExPerformanceCounter(base.CategoryName, "Remote PowerShell Tenant Session Creation Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellSessionCreationAttempts);
				this.MSExchangeRemotePoweshellSessionCreationSuccesses = new ExPerformanceCounter(base.CategoryName, "Remote PowerShell Tenant Session Creation Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellSessionCreationSuccesses);
				this.MSExchangeRemotePoweshellPartnerAuthorizationAttempts = new ExPerformanceCounter(base.CategoryName, "Remote PowerShell Partner Authorization Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellPartnerAuthorizationAttempts);
				this.MSExchangeRemotePoweshellPartnerAuthorizationSuccesses = new ExPerformanceCounter(base.CategoryName, "Remote Powershell Partner Authorization Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellPartnerAuthorizationSuccesses);
				this.MSExchangeRemotePoweshellPartnerSessionCreationAttempts = new ExPerformanceCounter(base.CategoryName, "Remote PowerShell Partner Session Creation Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellPartnerSessionCreationAttempts);
				this.MSExchangeRemotePoweshellPartnerSessionCreationSuccesses = new ExPerformanceCounter(base.CategoryName, "Remote PowerShell partner session creation successes per period.", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemotePoweshellPartnerSessionCreationSuccesses);
				this.MSExchangeECPSessionCreationAttempts = new ExPerformanceCounter(base.CategoryName, "Exchange Control Panel Session Creation Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeECPSessionCreationAttempts);
				this.MSExchangeECPSessionCreationSuccesses = new ExPerformanceCounter(base.CategoryName, "Exchange Control Panel Session Creation Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeECPSessionCreationSuccesses);
				this.MSExchangeECPRedirectionSuccesses = new ExPerformanceCounter(base.CategoryName, "ECP session redirection successes per period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeECPRedirectionSuccesses);
				this.MSExchangeNewMailboxAttempts = new ExPerformanceCounter(base.CategoryName, "NewMailbox Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewMailboxAttempts);
				this.MSExchangeNewMailboxSuccesses = new ExPerformanceCounter(base.CategoryName, "NewMailbox Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewMailboxSuccesses);
				this.MSExchangeNewMailboxIterationAttempts = new ExPerformanceCounter(base.CategoryName, "NewMailbox Iteration Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewMailboxIterationAttempts);
				this.MSExchangeNewMailboxIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "NewMailbox Iteration Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewMailboxIterationSuccesses);
				this.MSExchangeNewOrganizationAttempts = new ExPerformanceCounter(base.CategoryName, "NewOrganization Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewOrganizationAttempts);
				this.MSExchangeNewOrganizationSuccesses = new ExPerformanceCounter(base.CategoryName, "NewOrganization Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewOrganizationSuccesses);
				this.MSExchangeNewOrganizationIterationAttempts = new ExPerformanceCounter(base.CategoryName, "NewOrganization Iteration Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewOrganizationIterationAttempts);
				this.MSExchangeNewOrganizationIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "NewOrganization Iteration Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeNewOrganizationIterationSuccesses);
				this.MSExchangeRemoveOrganizationAttempts = new ExPerformanceCounter(base.CategoryName, "RemoveOrganization Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveOrganizationAttempts);
				this.MSExchangeRemoveOrganizationSuccesses = new ExPerformanceCounter(base.CategoryName, "RemoveOrganization successes per period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveOrganizationSuccesses);
				this.MSExchangeRemoveOrganizationIterationAttempts = new ExPerformanceCounter(base.CategoryName, "RemoveOrganization Iteration Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveOrganizationIterationAttempts);
				this.MSExchangeRemoveOrganizationIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "RemoveOrganization Iteration Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveOrganizationIterationSuccesses);
				this.MSExchangeAddSecondaryDomainAttempts = new ExPerformanceCounter(base.CategoryName, "AddSecondaryDomain Attempts Per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeAddSecondaryDomainAttempts);
				this.MSExchangeAddSecondaryDomainSuccesses = new ExPerformanceCounter(base.CategoryName, "AddSecondaryDomain Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeAddSecondaryDomainSuccesses);
				this.MSExchangeAddSecondaryDomainIterationAttempts = new ExPerformanceCounter(base.CategoryName, "AddSecondaryDomainIteration Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeAddSecondaryDomainIterationAttempts);
				this.MSExchangeAddSecondaryDomainIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "AddSecondaryDomain Iteration Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeAddSecondaryDomainIterationSuccesses);
				this.MSExchangeRemoveSecondaryDomainAttempts = new ExPerformanceCounter(base.CategoryName, "RemoveSecondaryDomain Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveSecondaryDomainAttempts);
				this.MSExchangeRemoveSecondaryDomainSuccesses = new ExPerformanceCounter(base.CategoryName, "RemoveSecondaryDomain Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveSecondaryDomainSuccesses);
				this.MSExchangeRemoveSecondaryDomainIterationAttempts = new ExPerformanceCounter(base.CategoryName, "RemoveSecondaryDomain Iteration Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveSecondaryDomainIterationAttempts);
				this.MSExchangeRemoveSecondaryDomainIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "RemoveSecondaryDomain Iteration Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeRemoveSecondaryDomainIterationSuccesses);
				this.MSExchangeStartOrganizationPilotAttempts = new ExPerformanceCounter(base.CategoryName, "StartOrganizationPilot Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationPilotAttempts);
				this.MSExchangeStartOrganizationPilotSuccesses = new ExPerformanceCounter(base.CategoryName, "StartOrganizationPilot Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationPilotSuccesses);
				this.MSExchangeStartOrganizationPilotIterationAttempts = new ExPerformanceCounter(base.CategoryName, "StartOrganizationPilot Iteration Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationPilotIterationAttempts);
				this.MSExchangeStartOrganizationPilotIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "StartOrganizationPilot Iteration Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationPilotIterationSuccesses);
				this.MSExchangeStartOrganizationUpgradeAttempts = new ExPerformanceCounter(base.CategoryName, "StartOrganizationUpgrade Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationUpgradeAttempts);
				this.MSExchangeStartOrganizationUpgradeSuccesses = new ExPerformanceCounter(base.CategoryName, "StartOrganizationUpgrade Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationUpgradeSuccesses);
				this.MSExchangeStartOrganizationUpgradeIterationAttempts = new ExPerformanceCounter(base.CategoryName, "StartOrganizationUpgrade Iteration Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationUpgradeIterationAttempts);
				this.MSExchangeStartOrganizationUpgradeIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "StartOrganizationUpgrade Iteration Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeStartOrganizationUpgradeIterationSuccesses);
				this.MSExchangeCompleteOrganizationUpgradeAttempts = new ExPerformanceCounter(base.CategoryName, "CompleteOrganizationUpgrade Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCompleteOrganizationUpgradeAttempts);
				this.MSExchangeCompleteOrganizationUpgradeSuccesses = new ExPerformanceCounter(base.CategoryName, "CompleteOrganizationUpgrade Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCompleteOrganizationUpgradeSuccesses);
				this.MSExchangeCompleteOrganizationUpgradeIterationAttempts = new ExPerformanceCounter(base.CategoryName, "CompleteOrganizationUpgrade Iteration Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCompleteOrganizationUpgradeIterationAttempts);
				this.MSExchangeCompleteOrganizationUpgradeIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "CompleteOrganizationUpgrade Iteration Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCompleteOrganizationUpgradeIterationSuccesses);
				this.MSExchangeGetManagementEndpointAttempts = new ExPerformanceCounter(base.CategoryName, "GetManagementEndpoint Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeGetManagementEndpointAttempts);
				this.MSExchangeGetManagementEndpointSuccesses = new ExPerformanceCounter(base.CategoryName, "GetManagementEndpoint Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeGetManagementEndpointSuccesses);
				this.MSExchangeGetManagementEndpointIterationAttempts = new ExPerformanceCounter(base.CategoryName, "GetManagementEndpoint Iteration Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeGetManagementEndpointIterationAttempts);
				this.MSExchangeGetManagementEndpointIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "GetManagementEndpoint Iteration Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeGetManagementEndpointIterationSuccesses);
				this.MSExchangeCmdletAttempts = new ExPerformanceCounter(base.CategoryName, "Cmdlet attempts per period. This is only for cmdlets which are to be monitored.", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCmdletAttempts);
				this.MSExchangeCmdletSuccesses = new ExPerformanceCounter(base.CategoryName, "Cmdlet Successes per Period. This is only for cmdlets which are to be monitored.", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCmdletSuccesses);
				this.MSExchangeCmdletIterationAttempts = new ExPerformanceCounter(base.CategoryName, "Cmdlet Iteration Attempts per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCmdletIterationAttempts);
				this.MSExchangeCmdletIterationSuccesses = new ExPerformanceCounter(base.CategoryName, "Cmdlet Iteration Successes per Period", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MSExchangeCmdletIterationSuccesses);
				long num = this.MSExchangeHomeSiteLocationAttempts.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter MSExchangeHomeSiteLocationAttempts;

		public readonly ExPerformanceCounter MSExchangeHomeSiteLocationSuccesses;

		public readonly ExPerformanceCounter MSExchangePartnerHomeSiteLocationAttempts;

		public readonly ExPerformanceCounter MSExchangePartnerHomeSiteLocationSuccesses;

		public readonly ExPerformanceCounter MSExchangeRemotePoweshellUserAuthorizationAttempts;

		public readonly ExPerformanceCounter MSExchangeRemotePoweshellUserAuthorizationSuccesses;

		public readonly ExPerformanceCounter MSExchangeRemotePoweshellSessionCreationAttempts;

		public readonly ExPerformanceCounter MSExchangeRemotePoweshellSessionCreationSuccesses;

		public readonly ExPerformanceCounter MSExchangeRemotePoweshellPartnerAuthorizationAttempts;

		public readonly ExPerformanceCounter MSExchangeRemotePoweshellPartnerAuthorizationSuccesses;

		public readonly ExPerformanceCounter MSExchangeRemotePoweshellPartnerSessionCreationAttempts;

		public readonly ExPerformanceCounter MSExchangeRemotePoweshellPartnerSessionCreationSuccesses;

		public readonly ExPerformanceCounter MSExchangeECPSessionCreationAttempts;

		public readonly ExPerformanceCounter MSExchangeECPSessionCreationSuccesses;

		public readonly ExPerformanceCounter MSExchangeECPRedirectionSuccesses;

		public readonly ExPerformanceCounter MSExchangeNewMailboxAttempts;

		public readonly ExPerformanceCounter MSExchangeNewMailboxSuccesses;

		public readonly ExPerformanceCounter MSExchangeNewMailboxIterationAttempts;

		public readonly ExPerformanceCounter MSExchangeNewMailboxIterationSuccesses;

		public readonly ExPerformanceCounter MSExchangeNewOrganizationAttempts;

		public readonly ExPerformanceCounter MSExchangeNewOrganizationSuccesses;

		public readonly ExPerformanceCounter MSExchangeNewOrganizationIterationAttempts;

		public readonly ExPerformanceCounter MSExchangeNewOrganizationIterationSuccesses;

		public readonly ExPerformanceCounter MSExchangeRemoveOrganizationAttempts;

		public readonly ExPerformanceCounter MSExchangeRemoveOrganizationSuccesses;

		public readonly ExPerformanceCounter MSExchangeRemoveOrganizationIterationAttempts;

		public readonly ExPerformanceCounter MSExchangeRemoveOrganizationIterationSuccesses;

		public readonly ExPerformanceCounter MSExchangeAddSecondaryDomainAttempts;

		public readonly ExPerformanceCounter MSExchangeAddSecondaryDomainSuccesses;

		public readonly ExPerformanceCounter MSExchangeAddSecondaryDomainIterationAttempts;

		public readonly ExPerformanceCounter MSExchangeAddSecondaryDomainIterationSuccesses;

		public readonly ExPerformanceCounter MSExchangeRemoveSecondaryDomainAttempts;

		public readonly ExPerformanceCounter MSExchangeRemoveSecondaryDomainSuccesses;

		public readonly ExPerformanceCounter MSExchangeRemoveSecondaryDomainIterationAttempts;

		public readonly ExPerformanceCounter MSExchangeRemoveSecondaryDomainIterationSuccesses;

		public readonly ExPerformanceCounter MSExchangeStartOrganizationPilotAttempts;

		public readonly ExPerformanceCounter MSExchangeStartOrganizationPilotSuccesses;

		public readonly ExPerformanceCounter MSExchangeStartOrganizationPilotIterationAttempts;

		public readonly ExPerformanceCounter MSExchangeStartOrganizationPilotIterationSuccesses;

		public readonly ExPerformanceCounter MSExchangeStartOrganizationUpgradeAttempts;

		public readonly ExPerformanceCounter MSExchangeStartOrganizationUpgradeSuccesses;

		public readonly ExPerformanceCounter MSExchangeStartOrganizationUpgradeIterationAttempts;

		public readonly ExPerformanceCounter MSExchangeStartOrganizationUpgradeIterationSuccesses;

		public readonly ExPerformanceCounter MSExchangeCompleteOrganizationUpgradeAttempts;

		public readonly ExPerformanceCounter MSExchangeCompleteOrganizationUpgradeSuccesses;

		public readonly ExPerformanceCounter MSExchangeCompleteOrganizationUpgradeIterationAttempts;

		public readonly ExPerformanceCounter MSExchangeCompleteOrganizationUpgradeIterationSuccesses;

		public readonly ExPerformanceCounter MSExchangeGetManagementEndpointAttempts;

		public readonly ExPerformanceCounter MSExchangeGetManagementEndpointSuccesses;

		public readonly ExPerformanceCounter MSExchangeGetManagementEndpointIterationAttempts;

		public readonly ExPerformanceCounter MSExchangeGetManagementEndpointIterationSuccesses;

		public readonly ExPerformanceCounter MSExchangeCmdletAttempts;

		public readonly ExPerformanceCounter MSExchangeCmdletSuccesses;

		public readonly ExPerformanceCounter MSExchangeCmdletIterationAttempts;

		public readonly ExPerformanceCounter MSExchangeCmdletIterationSuccesses;
	}
}
