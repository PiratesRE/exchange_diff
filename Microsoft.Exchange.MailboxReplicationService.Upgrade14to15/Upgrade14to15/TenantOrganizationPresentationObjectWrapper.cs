using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	public class TenantOrganizationPresentationObjectWrapper
	{
		public TenantOrganizationPresentationObjectWrapper()
		{
		}

		public TenantOrganizationPresentationObjectWrapper(TenantOrganizationPresentationObject tenant)
		{
			this.Name = tenant.Name;
			this.UpgradeStatus = tenant.UpgradeStatus;
			this.UpgradeRequest = tenant.UpgradeRequest;
			this.UpgradeMessage = tenant.UpgradeMessage;
			this.UpgradeDetails = tenant.UpgradeDetails;
			this.UpgradeConstraints = tenant.UpgradeConstraints;
			this.IsUpgradingOrganization = tenant.IsUpgradingOrganization;
			this.IsPilotingOrganization = tenant.IsPilotingOrganization;
			this.AdminDisplayVersion = tenant.AdminDisplayVersion;
			this.ServicePlan = tenant.ServicePlan;
			this.ExternalDirectoryOrganizationId = tenant.ExternalDirectoryOrganizationId;
			this.IsUpgradeOperationInProgress = tenant.IsUpgradeOperationInProgress;
			this.ProgramId = tenant.ProgramId;
			this.OfferId = tenant.OfferId;
			this.UpgradeStage = tenant.UpgradeStage;
			this.UpgradeStageTimeStamp = tenant.UpgradeStageTimeStamp;
			this.UpgradeE14MbxCountForCurrentStage = tenant.UpgradeE14MbxCountForCurrentStage;
			this.UpgradeE14RequestCountForCurrentStage = tenant.UpgradeE14RequestCountForCurrentStage;
			this.UpgradeLastE14CountsUpdateTime = tenant.UpgradeLastE14CountsUpdateTime;
			this.UpgradeConstraintsDisabled = tenant.UpgradeConstraintsDisabled;
			this.UpgradeUnitsOverride = tenant.UpgradeUnitsOverride;
		}

		public string Name { get; private set; }

		public UpgradeStatusTypes UpgradeStatus { get; set; }

		public UpgradeRequestTypes UpgradeRequest { get; set; }

		public string UpgradeMessage { get; set; }

		public string UpgradeDetails { get; set; }

		public UpgradeConstraintArray UpgradeConstraints { get; set; }

		public bool IsUpgradingOrganization { get; private set; }

		public bool IsPilotingOrganization { get; private set; }

		public ExchangeObjectVersion AdminDisplayVersion { get; private set; }

		public string ServicePlan { get; private set; }

		public string ExternalDirectoryOrganizationId { get; private set; }

		public bool IsUpgradeOperationInProgress { get; private set; }

		public string ProgramId { get; private set; }

		public string OfferId { get; private set; }

		public UpgradeStage? UpgradeStage { get; set; }

		public DateTime? UpgradeStageTimeStamp { get; set; }

		public int? UpgradeE14MbxCountForCurrentStage { get; set; }

		public int? UpgradeE14RequestCountForCurrentStage { get; set; }

		public DateTime? UpgradeLastE14CountsUpdateTime { get; set; }

		public bool? UpgradeConstraintsDisabled { get; set; }

		public int? UpgradeUnitsOverride { get; set; }
	}
}
