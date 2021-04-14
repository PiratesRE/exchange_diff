using System;
using System.Collections.Generic;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[Serializable]
	public sealed class TenantInfo
	{
		public TenantInfo(Guid tenantId, string syncSvcUrl, Dictionary<ConfigurationObjectType, SyncInfo> syncInfoTable)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("syncSvcUrl", syncSvcUrl);
			this.TenantId = tenantId;
			this.SyncSvcUrl = syncSvcUrl;
			this.SyncInfoTable = syncInfoTable;
		}

		public Guid TenantId { get; set; }

		public string SyncSvcUrl { get; set; }

		public DateTime? LastAttemptedSyncUTC { get; set; }

		public DateTime? LastSuccessfulSyncUTC { get; set; }

		public DateTime? LastErrorTimeUTC { get; set; }

		public string[] LastErrors { get; set; }

		public Dictionary<ConfigurationObjectType, SyncInfo> SyncInfoTable { get; set; }
	}
}
