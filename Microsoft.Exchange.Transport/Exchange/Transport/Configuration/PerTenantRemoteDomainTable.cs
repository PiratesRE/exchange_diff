using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal sealed class PerTenantRemoteDomainTable : TenantConfigurationCacheableItem<DomainContentConfig>
	{
		public PerTenantRemoteDomainTable()
		{
		}

		public PerTenantRemoteDomainTable(RemoteDomainTable rdt) : base(true)
		{
			this.SetInternalData(rdt);
		}

		public RemoteDomainTable RemoteDomainTable
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.remoteDomainTable;
			}
		}

		public override long ItemSize
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return (long)(this.estimatedSize + 4);
			}
		}

		public override void ReadData(IConfigurationSession session)
		{
			this.estimatedSize = 0;
			ArgumentValidator.ThrowIfNull("session", session);
			ADPagedReader<DomainContentConfig> adpagedReader = session.FindAllPaged<DomainContentConfig>();
			List<RemoteDomainEntry> list = new List<RemoteDomainEntry>();
			if (adpagedReader != null)
			{
				foreach (DomainContentConfig domainContentConfig in adpagedReader)
				{
					if (domainContentConfig.DomainName != null)
					{
						RemoteDomainEntry remoteDomainEntry = new RemoteDomainEntry(domainContentConfig);
						list.Add(remoteDomainEntry);
						this.estimatedSize += remoteDomainEntry.EstimateSize;
					}
				}
			}
			this.SetInternalData(new RemoteDomainTable(list));
		}

		private void SetInternalData(RemoteDomainTable remoteDomainTable)
		{
			ArgumentValidator.ThrowIfNull("remoteDomainTable", remoteDomainTable);
			this.remoteDomainTable = remoteDomainTable;
		}

		private RemoteDomainTable remoteDomainTable;

		private int estimatedSize;
	}
}
