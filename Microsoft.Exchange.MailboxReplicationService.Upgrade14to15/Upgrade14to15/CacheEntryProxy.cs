using System;
using Microsoft.Exchange.AnchorService;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	internal class CacheEntryProxy : ICacheEntry
	{
		public CacheEntryProxy(CacheEntryBase cacheEntryBase)
		{
			this.CacheEntryBase = cacheEntryBase;
			this.ADSessionProxy = new ADSessionProxy(cacheEntryBase.ADProvider);
		}

		public string OrgName
		{
			get
			{
				if (this.CacheEntryBase.OrganizationId.OrganizationalUnit == null)
				{
					return "<null>";
				}
				return this.CacheEntryBase.OrganizationId.OrganizationalUnit.Name;
			}
		}

		public string ExternalDirectoryOrganizationId
		{
			get
			{
				return this.CacheEntryBase.OrganizationId.ToExternalDirectoryOrganizationId();
			}
		}

		public CacheEntryBase CacheEntryBase { get; private set; }

		public IADSession ADSessionProxy { get; private set; }
	}
}
