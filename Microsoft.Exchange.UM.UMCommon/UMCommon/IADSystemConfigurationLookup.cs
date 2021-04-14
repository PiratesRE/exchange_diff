using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface IADSystemConfigurationLookup
	{
		UMDialPlan GetDialPlanFromId(ADObjectId dialPlanId);

		UMDialPlan GetDialPlanFromRecipient(IADRecipient recipient);

		UMDialPlan GetDialPlanFromPilotIdentifier(string pilotIdentifier);

		UMIPGateway GetIPGatewayFromId(ADObjectId gatewayId);

		ExchangeConfigurationUnit GetConfigurationUnitByTenantGuid(Guid tenantGuid);

		IEnumerable<UMIPGateway> GetAllGlobalGateways();

		IEnumerable<UMDialPlan> GetAllDialPlans();

		UMIPGateway GetIPGatewayFromAddress(IList<string> fqdns);

		IEnumerable<UMIPGateway> GetAllIPGateways();

		IEnumerable<UMIPGateway> GetGatewaysLinkedToDialPlan(UMDialPlan dialPlan);

		UMAutoAttendant GetAutoAttendantFromId(ADObjectId autoAttendantId);

		UMAutoAttendant GetAutoAttendantFromPilotIdentifierAndDialPlan(string pilotIdentifier, ADObjectId dialPlanId);

		UMAutoAttendant GetAutoAttendantWithNoDialplanInformation(string pilotIdentifier);

		OrganizationId GetOrganizationIdFromDomainName(string domainName, out bool isAuthoritative);

		MicrosoftExchangeRecipient GetMicrosoftExchangeRecipient();

		AcceptedDomain GetDefaultAcceptedDomain();

		ExchangeConfigurationUnit GetConfigurationUnitByADObjectId(ADObjectId configUnit);

		UMMailboxPolicy GetPolicyFromRecipient(ADRecipient recipient);

		UMMailboxPolicy GetUMMailboxPolicyFromId(ADObjectId mbxPolicyId);

		UMAutoAttendant GetAutoAttendantFromName(string autoAttendantName);

		IEnumerable<Guid> GetAutoAttendantDialPlans();

		void GetAutoAttendantAddressLists(HashSet<Guid> addressListGuids);

		Guid GetExternalDirectoryOrganizationId();

		void GetGlobalAddressLists(HashSet<Guid> addressListGuids);
	}
}
