using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.Transport;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport
{
	internal class RemoteDomainTable : RemoteDomainMap
	{
		public RemoteDomainTable(IList<RemoteDomainEntry> entries) : base(entries)
		{
		}

		public class Builder : ConfigurationLoader<RemoteDomainTable, RemoteDomainTable.Builder>.SimpleBuilder<DomainContentConfig>
		{
			public override void LoadData(ITopologyConfigurationSession session, QueryScope scope)
			{
				base.RootId = session.GetOrgContainerId().GetChildId("Global Settings").GetChildId("Internet Message Formats");
				base.LoadData(session, QueryScope.OneLevel);
			}

			protected override RemoteDomainTable BuildCache(List<DomainContentConfig> domains)
			{
				List<RemoteDomainEntry> list = new List<RemoteDomainEntry>(domains.Count);
				foreach (DomainContentConfig domainContentConfig in domains)
				{
					if (domainContentConfig.DomainName != null)
					{
						RemoteDomainEntry item = new RemoteDomainEntry(domainContentConfig);
						list.Add(item);
					}
				}
				return new RemoteDomainTable(list);
			}

			protected override ADOperationResult TryRegisterChangeNotification<TConfigObject>(Func<ADObjectId> rootIdGetter, out ADNotificationRequestCookie cookie)
			{
				return TransportADNotificationAdapter.TryRegisterNotifications(new Func<ADObjectId>(ConfigurationLoader<RemoteDomainTable, RemoteDomainTable.Builder>.Builder.GetFirstOrgContainerId), new ADNotificationCallback(base.Reload), new TransportADNotificationAdapter.TransportADNotificationRegister(TransportADNotificationAdapter.Instance.RegisterForRemoteDomainNotifications), 3, out cookie);
			}
		}
	}
}
