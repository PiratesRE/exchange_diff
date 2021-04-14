using System;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class OwaPerTenantRemoteDomains : TenantConfigurationCacheableItem<DomainContentConfig>
	{
		public override long ItemSize
		{
			get
			{
				if (this.remoteDomainMap == null)
				{
					return 18L;
				}
				long num = 18L;
				return num + (this.estimatedRemoteDomainEntryArraySize + 8L + 4L);
			}
		}

		public override void ReadData(IConfigurationSession session)
		{
			ADPagedReader<DomainContentConfig> adpagedReader = session.FindAllPaged<DomainContentConfig>();
			DomainContentConfig[] source = adpagedReader.ReadAllPages();
			RemoteDomainEntry[] array = (from domain in source
			select new RemoteDomainEntry(domain)).ToArray<RemoteDomainEntry>();
			this.remoteDomainMap = new RemoteDomainMap(array);
			int num = 0;
			foreach (RemoteDomainEntry remoteDomainEntry in array)
			{
				num += remoteDomainEntry.EstimateSize;
			}
			this.estimatedRemoteDomainEntryArraySize = (long)num;
		}

		internal RemoteDomainMap GetRemoteDomainMap(OrganizationId organizationId)
		{
			if (base.OrganizationId == organizationId)
			{
				return this.remoteDomainMap;
			}
			return null;
		}

		private const int FixedClrObjectOverhead = 18;

		private long estimatedRemoteDomainEntryArraySize;

		private RemoteDomainMap remoteDomainMap;
	}
}
