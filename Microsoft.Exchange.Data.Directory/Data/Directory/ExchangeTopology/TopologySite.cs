using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.ExchangeTopology
{
	internal sealed class TopologySite : ADSite, ITopologySite
	{
		internal TopologySite(ADSite site)
		{
			this.propertyBag = site.propertyBag;
			this.SetIsReadOnly(site.IsReadOnly);
			this.m_Session = site.Session;
		}

		public ReadOnlyCollection<ITopologySiteLink> TopologySiteLinks
		{
			get
			{
				return this.topologySiteLinks;
			}
			internal set
			{
				this.topologySiteLinks = value;
			}
		}

		private ReadOnlyCollection<ITopologySiteLink> topologySiteLinks;
	}
}
