using System;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	internal struct TenantUpgradeData
	{
		public TenantOrganizationPresentationObjectWrapper Tenant { get; set; }

		public RecipientWrapper PilotUser { get; set; }

		public string ErrorType { get; set; }

		public string ErrorDetails { get; set; }
	}
}
