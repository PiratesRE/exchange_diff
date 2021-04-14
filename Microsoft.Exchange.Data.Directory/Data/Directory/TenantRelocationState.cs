using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class TenantRelocationState
	{
		public string SourceForestFQDN { get; private set; }

		public string TargetForestFQDN { get; private set; }

		public TenantRelocationStatus SourceForestState { get; private set; }

		public RelocationStatusDetailsDestination TargetForestState { get; private set; }

		public OrganizationId OrganizationId { get; private set; }

		public OrganizationId TargetOrganizationId { get; private set; }

		public TenantRelocationState(string sourceForestFQDN, TenantRelocationStatus sourceForestState)
		{
			if (string.IsNullOrEmpty(sourceForestFQDN))
			{
				throw new ArgumentNullException("sourceForestFQDN");
			}
			this.SourceForestFQDN = sourceForestFQDN;
			this.SourceForestState = sourceForestState;
		}

		public TenantRelocationState(string sourceForestFQDN, TenantRelocationStatus sourceForestState, string targetForestFQDN, RelocationStatusDetailsDestination targetForestState, OrganizationId organizationId, OrganizationId targetOrganizationId) : this(sourceForestFQDN, sourceForestState)
		{
			if (string.IsNullOrEmpty(targetForestFQDN))
			{
				throw new ArgumentNullException("targetForestFQDN");
			}
			this.TargetForestFQDN = targetForestFQDN;
			this.TargetForestState = targetForestState;
			this.OrganizationId = organizationId;
			this.TargetOrganizationId = targetOrganizationId;
		}
	}
}
