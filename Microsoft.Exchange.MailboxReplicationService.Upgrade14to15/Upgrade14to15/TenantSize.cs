using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	internal struct TenantSize
	{
		public Guid ExternalDirectoryOrganizationId { get; set; }

		public string Name { get; set; }

		public string[] Constraints { get; set; }

		public bool? UpgradeConstraintsDisabled { get; set; }

		public int? UpgradeUnitsOverride { get; set; }

		public string ServicePlan { get; set; }

		public string ProgramId { get; set; }

		public string OfferId { get; set; }

		public ExchangeObjectVersion AdminDisplayVersion { get; set; }

		public bool IsUpgradingOrganization { get; set; }

		public bool IsPilotingOrganization { get; set; }

		public int E14PrimaryMbxCount { get; set; }

		public double E14PrimaryMbxSize { get; set; }

		public int E14ArchiveMbxCount { get; set; }

		public double E14ArchiveMbxSize { get; set; }

		public int E15PrimaryMbxCount { get; set; }

		public double E15PrimaryMbxSize { get; set; }

		public int E15ArchiveMbxCount { get; set; }

		public double E15ArchiveMbxSize { get; set; }

		public int TotalPrimaryMbxCount { get; set; }

		public double TotalPrimaryMbxSize { get; set; }

		public int TotalArchiveMbxCount { get; set; }

		public double TotalArchiveMbxSize { get; set; }

		public int UploadedSize { get; set; }

		public string ValidationError { get; set; }
	}
}
