﻿using System;

namespace Microsoft.Exchange.Configuration.TenantMonitoring
{
	internal enum CounterType
	{
		HomeSiteLocationAttempts,
		HomeSiteLocationSuccesses,
		PartnerHomeSiteLocationAttempts,
		PartnerHomeSiteLocationSuccesses,
		RemotePoweshellUserAuthorizationAttempts,
		RemotePoweshellUserAuthorizationSuccesses,
		RemotePoweshellSessionCreationAttempts,
		RemotePoweshellSessionCreationSuccesses,
		RemotePoweshellPartnerAuthorizationAttempts,
		RemotePoweshellPartnerAuthorizationSuccesses,
		RemotePoweshellPartnerSessionCreationAttempts,
		RemotePoweshellPartnerSessionCreationSuccesses,
		ECPSessionCreationAttempts,
		ECPSessionCreationSuccesses,
		ECPRedirectionSuccesses,
		NewMailboxAttempts,
		NewMailboxSuccesses,
		NewMailboxIterationAttempts,
		NewMailboxIterationSuccesses,
		NewOrganizationAttempts,
		NewOrganizationSuccesses,
		NewOrganizationIterationAttempts,
		NewOrganizationIterationSuccesses,
		RemoveOrganizationAttempts,
		RemoveOrganizationSuccesses,
		RemoveOrganizationIterationAttempts,
		RemoveOrganizationIterationSuccesses,
		AddSecondaryDomainAttempts,
		AddSecondaryDomainSuccesses,
		AddSecondaryDomainIterationAttempts,
		AddSecondaryDomainIterationSuccesses,
		RemoveSecondaryDomainAttempts,
		RemoveSecondaryDomainSuccesses,
		RemoveSecondaryDomainIterationAttempts,
		RemoveSecondaryDomainIterationSuccesses,
		StartOrganizationPilotAttempts,
		StartOrganizationPilotSuccesses,
		StartOrganizationPilotIterationAttempts,
		StartOrganizationPilotIterationSuccesses,
		StartOrganizationUpgradeAttempts,
		StartOrganizationUpgradeSuccesses,
		StartOrganizationUpgradeIterationAttempts,
		StartOrganizationUpgradeIterationSuccesses,
		CompleteOrganizationUpgradeAttempts,
		CompleteOrganizationUpgradeSuccesses,
		CompleteOrganizationUpgradeIterationAttempts,
		CompleteOrganizationUpgradeIterationSuccesses,
		GetManagementEndpointAttempts,
		GetManagementEndpointSuccesses,
		GetManagementEndpointIterationAttempts,
		GetManagementEndpointIterationSuccesses,
		CmdletAttempts,
		CmdletSuccesses,
		CmdletIterationAttempts,
		CmdletIterationSuccesses,
		MaxValue = 54
	}
}