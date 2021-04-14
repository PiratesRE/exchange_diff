using System;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class TenantRelocationSyncData
	{
		public TenantPartitionHint SourceTenantPartitionHint { get; private set; }

		public ADObjectId ResourcePartitionRoot { get; private set; }

		public ADObjectId ResourcePartitionConfigNc { get; private set; }

		public ADObjectId SourceConfigContainer { get; private set; }

		public TenantRelocationSyncPartitionData Source { get; private set; }

		public TenantRelocationSyncPartitionData Target { get; private set; }

		public bool LargeTenantModeEnabled { get; private set; }

		public bool IsSourceSoftLinkEnabled
		{
			get
			{
				return !this.ResourcePartitionRoot.Equals(this.Source.PartitionRoot);
			}
		}

		public bool IsTargetSoftLinkEnabled
		{
			get
			{
				return !this.ResourcePartitionRoot.Equals(this.Target.PartitionRoot);
			}
		}

		public TenantRelocationSyncData(OrganizationId sourceTenantOrganizationId, OrganizationId targetTenantOrganizationId, ADObjectId resourcePartitionRoot, TenantPartitionHint partitionHint, bool largeTenantModeEnabled)
		{
			if (targetTenantOrganizationId.ConfigurationUnit == null)
			{
				throw new ArgumentNullException("targetTenantConfigurationUnit");
			}
			ADObjectId domainId = sourceTenantOrganizationId.OrganizationalUnit.DomainId;
			ADObjectId domainId2 = targetTenantOrganizationId.OrganizationalUnit.DomainId;
			PartitionId partitionId = sourceTenantOrganizationId.OrganizationalUnit.GetPartitionId();
			PartitionId partitionId2 = targetTenantOrganizationId.OrganizationalUnit.GetPartitionId();
			this.Source = new TenantRelocationSyncPartitionData(sourceTenantOrganizationId, domainId, partitionId);
			this.Target = new TenantRelocationSyncPartitionData(targetTenantOrganizationId, domainId2, partitionId2);
			this.ResourcePartitionRoot = resourcePartitionRoot;
			this.ResourcePartitionConfigNc = this.ResourcePartitionRoot.GetChildId("Configuration");
			this.SourceTenantPartitionHint = partitionHint;
			this.SourceConfigContainer = this.Source.TenantConfigurationUnit.Parent;
			this.LargeTenantModeEnabled = largeTenantModeEnabled;
		}
	}
}
