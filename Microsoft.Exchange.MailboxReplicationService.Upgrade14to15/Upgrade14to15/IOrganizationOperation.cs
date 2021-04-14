using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	internal interface IOrganizationOperation
	{
		TenantOrganizationPresentationObjectWrapper GetOrganization(string tenantId);

		List<TenantOrganizationPresentationObjectWrapper> GetAllOrganizations(bool checkAllPartitions);

		void SetOrganization(TenantOrganizationPresentationObjectWrapper tenant, UpgradeStatusTypes status, UpgradeRequestTypes request, string message, string details, UpgradeStage? upgradeStage = 0, int e14MbxCountForCurrentStage = -1, int nonUpgradeMoveRequestCount = -1);

		RecipientWrapper GetUser(string organizationId, string userId);

		void SetUser(RecipientWrapper user, UpgradeStatusTypes status, UpgradeRequestTypes request, string message, string details, UpgradeStage? stage);

		void InvokeOrganizationCmdlet(string organizationId, string cmdlet, bool configOnly);

		bool TryGetAnchorMailbox(string tenantId, out RecipientWrapper anchorMailbox);

		void CreateAnchorMailbox(string tenantId);

		void SetTenantUpgradeCapability(string identity, bool tenantUpgradeCapabilityEnabled);

		bool TryRemoveMoveRequest(string identity);
	}
}
