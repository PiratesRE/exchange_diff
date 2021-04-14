using System;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class DistinguishedNameMapItem
	{
		public ADObjectId SourceDN { get; private set; }

		public ADObjectId TargetDN { get; private set; }

		public Guid CorrelationId { get; private set; }

		public DistinguishedNameMapItem(ADObjectId source, ADObjectId target, Guid correlationId)
		{
			this.SourceDN = source;
			this.TargetDN = target;
			this.CorrelationId = correlationId;
		}
	}
}
