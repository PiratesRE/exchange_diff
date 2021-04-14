using System;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[Serializable]
	public sealed class TenantContext
	{
		public TenantContext(Guid tenantId, string tenantContextInfo)
		{
			this.TenantId = tenantId;
			this.TenantContextInfo = tenantContextInfo;
		}

		public Guid TenantId { get; set; }

		public string TenantContextInfo { get; set; }
	}
}
