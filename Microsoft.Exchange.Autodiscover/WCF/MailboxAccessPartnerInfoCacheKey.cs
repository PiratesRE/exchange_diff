using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal sealed class MailboxAccessPartnerInfoCacheKey
	{
		public MailboxAccessPartnerInfoCacheKey(ADObjectId adObjectId, OrganizationId organizationId)
		{
			this.ADObjectId = adObjectId;
			this.OrganizationId = organizationId;
		}

		public ADObjectId ADObjectId { get; private set; }

		public OrganizationId OrganizationId { get; private set; }

		public override bool Equals(object obj)
		{
			MailboxAccessPartnerInfoCacheKey mailboxAccessPartnerInfoCacheKey = obj as MailboxAccessPartnerInfoCacheKey;
			return mailboxAccessPartnerInfoCacheKey != null && mailboxAccessPartnerInfoCacheKey.ADObjectId.Equals(this.ADObjectId) && mailboxAccessPartnerInfoCacheKey.OrganizationId.Equals(this.OrganizationId);
		}

		public override int GetHashCode()
		{
			return this.ADObjectId.GetHashCode();
		}
	}
}
