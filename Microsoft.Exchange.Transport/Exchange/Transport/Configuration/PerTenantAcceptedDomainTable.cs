using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal class PerTenantAcceptedDomainTable : TenantConfigurationCacheableItem<AcceptedDomain>
	{
		public PerTenantAcceptedDomainTable()
		{
		}

		public PerTenantAcceptedDomainTable(AcceptedDomainTable adt) : base(true)
		{
			this.SetInternalData(adt);
		}

		public virtual AcceptedDomainTable AcceptedDomainTable
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.acceptedDomainTable;
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
			ArgumentValidator.ThrowIfNull("session", session);
			ADPagedReader<AcceptedDomain> domains = session.FindAllPaged<AcceptedDomain>();
			List<AcceptedDomainEntry> entries;
			AcceptedDomainEntry defaultDomain;
			List<string> internalDomains;
			this.estimatedSize = AcceptedDomainTable.Builder.CreateAcceptedDomainEntries(domains, out entries, out defaultDomain, out internalDomains);
			this.SetInternalData(new AcceptedDomainTable(internalDomains, defaultDomain, entries));
		}

		private void SetInternalData(AcceptedDomainTable acceptedDomainTable)
		{
			ArgumentValidator.ThrowIfNull("acceptedDomainTable", acceptedDomainTable);
			this.acceptedDomainTable = acceptedDomainTable;
		}

		private AcceptedDomainTable acceptedDomainTable;

		private int estimatedSize;
	}
}
