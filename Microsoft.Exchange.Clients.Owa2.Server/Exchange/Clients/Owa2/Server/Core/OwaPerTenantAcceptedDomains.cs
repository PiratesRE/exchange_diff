using System;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class OwaPerTenantAcceptedDomains : TenantConfigurationCacheableItem<AcceptedDomain>
	{
		public override long ItemSize
		{
			get
			{
				if (this.acceptedDomainMap == null)
				{
					return 18L;
				}
				long num = 18L;
				return num + (this.estimatedAcceptedDomainEntryArraySize + 8L + 4L);
			}
		}

		public override void ReadData(IConfigurationSession session)
		{
			ADPagedReader<AcceptedDomain> adpagedReader = session.FindAllPaged<AcceptedDomain>();
			AcceptedDomain[] source = adpagedReader.ReadAllPages();
			AcceptedDomainEntry[] array = (from domain in source
			select new AcceptedDomainEntry(domain, base.OrganizationId)).ToArray<AcceptedDomainEntry>();
			this.acceptedDomainMap = new AcceptedDomainMap(array);
			int num = 0;
			foreach (AcceptedDomainEntry acceptedDomainEntry in array)
			{
				num += acceptedDomainEntry.EstimatedSize;
			}
			this.estimatedAcceptedDomainEntryArraySize = (long)num;
		}

		internal AcceptedDomainMap GetAcceptedDomainMap(OrganizationId organizationId, out bool scopedToOrganization)
		{
			scopedToOrganization = false;
			if (base.OrganizationId == organizationId)
			{
				scopedToOrganization = true;
				return this.acceptedDomainMap;
			}
			return null;
		}

		private const int FixedClrObjectOverhead = 18;

		private long estimatedAcceptedDomainEntryArraySize;

		private AcceptedDomainMap acceptedDomainMap;
	}
}
