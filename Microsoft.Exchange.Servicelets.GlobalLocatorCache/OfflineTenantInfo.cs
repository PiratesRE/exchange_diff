using System;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	internal class OfflineTenantInfo
	{
		public OfflineTenantInfo(Guid tenantId, int partnerId, int minorPartnerId, int resourceForestId, int accountForestId)
		{
			this.TenantId = tenantId;
			this.PartnerId = partnerId;
			this.MinorPartnerId = minorPartnerId;
			this.ResourceForestId = resourceForestId;
			this.AccountForestId = accountForestId;
		}

		public Guid TenantId { get; private set; }

		public int PartnerId { get; private set; }

		public int MinorPartnerId { get; private set; }

		public int ResourceForestId { get; private set; }

		public int AccountForestId { get; private set; }
	}
}
