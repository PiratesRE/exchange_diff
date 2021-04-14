using System;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class TenantRelocationSyncPartitionData
	{
		public OrganizationId OrganizationId { get; private set; }

		public ADObjectId TenantOrganizationUnit
		{
			get
			{
				return this.OrganizationId.OrganizationalUnit;
			}
		}

		public ADObjectId TenantConfigurationUnit
		{
			get
			{
				return this.OrganizationId.ConfigurationUnit;
			}
		}

		public ADObjectId TenantConfigurationUnitRoot { get; private set; }

		public ADObjectId PartitionRoot { get; private set; }

		public ADObjectId PartitionConfigNcRoot { get; private set; }

		public PartitionId PartitionId { get; private set; }

		public bool IsConfigurationUnitUnderConfigNC { get; private set; }

		internal TenantRelocationSyncPartitionData(OrganizationId sourceTenantOrganizationId, ADObjectId sourcePartitionRoot, PartitionId sourcePartionId)
		{
			this.OrganizationId = sourceTenantOrganizationId;
			this.PartitionRoot = sourcePartitionRoot;
			this.PartitionId = sourcePartionId;
			this.PartitionConfigNcRoot = this.PartitionRoot.GetChildId("Configuration");
			this.IsConfigurationUnitUnderConfigNC = this.TenantConfigurationUnit.IsDescendantOf(this.PartitionConfigNcRoot);
			this.TenantConfigurationUnitRoot = this.TenantConfigurationUnit.Parent;
		}

		internal bool IsTenantRootObject(ADObjectId id)
		{
			return this.TenantConfigurationUnit.Equals(id) || this.TenantConfigurationUnitRoot.Equals(id) || this.TenantOrganizationUnit.Equals(id);
		}

		internal bool IsUnderTenantScope(ADObjectId value)
		{
			if (string.IsNullOrEmpty(value.DistinguishedName))
			{
				throw new ArgumentException("value.DistinguishedName must not be null");
			}
			return value.IsDescendantOf(this.TenantConfigurationUnitRoot) || value.IsDescendantOf(this.TenantOrganizationUnit);
		}
	}
}
