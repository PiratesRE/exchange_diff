using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.Transport;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport
{
	internal class X400AuthoritativeDomainTable
	{
		private X400AuthoritativeDomainTable(List<X400AuthoritativeDomainEntry> domainEntries)
		{
			this.domainEntries = domainEntries;
		}

		public bool CheckAccepted(RoutingX400Address address)
		{
			X400AuthoritativeDomainEntry x400AuthoritativeDomainEntry = this.FindBestMatch(address);
			return x400AuthoritativeDomainEntry != null && !x400AuthoritativeDomainEntry.ExternalRelay;
		}

		public X400AuthoritativeDomainEntry FindBestMatch(RoutingX400Address address)
		{
			X400AuthoritativeDomainEntry x400AuthoritativeDomainEntry = null;
			int num = 0;
			foreach (X400AuthoritativeDomainEntry x400AuthoritativeDomainEntry2 in this.domainEntries)
			{
				if (x400AuthoritativeDomainEntry2.Domain.Match(address))
				{
					int count = x400AuthoritativeDomainEntry2.Domain.Components.Count;
					if (count == address.ComponentsCount)
					{
						return x400AuthoritativeDomainEntry2;
					}
					if (x400AuthoritativeDomainEntry == null || num < count)
					{
						x400AuthoritativeDomainEntry = x400AuthoritativeDomainEntry2;
						num = count;
					}
				}
			}
			return x400AuthoritativeDomainEntry;
		}

		private readonly List<X400AuthoritativeDomainEntry> domainEntries;

		public class Builder : ConfigurationLoader<X400AuthoritativeDomainTable, X400AuthoritativeDomainTable.Builder>.SimpleBuilder<X400AuthoritativeDomain>
		{
			protected override X400AuthoritativeDomainTable BuildCache(List<X400AuthoritativeDomain> domains)
			{
				List<X400AuthoritativeDomainEntry> list = new List<X400AuthoritativeDomainEntry>(domains.Count);
				foreach (X400AuthoritativeDomain x400AuthoritativeDomain in domains)
				{
					if (x400AuthoritativeDomain.X400DomainName != null)
					{
						list.Add(new X400AuthoritativeDomainEntry(x400AuthoritativeDomain));
					}
				}
				return new X400AuthoritativeDomainTable(list);
			}

			protected override ADOperationResult TryRegisterChangeNotification<TConfigObject>(Func<ADObjectId> rootIdGetter, out ADNotificationRequestCookie cookie)
			{
				return TransportADNotificationAdapter.TryRegisterNotifications(rootIdGetter, new ADNotificationCallback(base.Reload), new TransportADNotificationAdapter.TransportADNotificationRegister(TransportADNotificationAdapter.Instance.RegisterForAcceptedDomainNotifications), 3, out cookie);
			}

			public override void LoadData(ITopologyConfigurationSession session, QueryScope scope)
			{
				base.RootId = session.GetOrgContainerId().GetChildId("Transport Settings").GetChildId("Accepted Domains");
				base.LoadData(session, QueryScope.OneLevel);
			}
		}
	}
}
