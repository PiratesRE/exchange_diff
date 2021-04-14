using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SiteLink
	{
		internal SiteLink(TopologySiteLink topologySiteLink, ServiceTopology.All all)
		{
			this.DistinguishedName = topologySiteLink.DistinguishedName;
			all.SiteLinks.Add(this.DistinguishedName, this);
			this.Name = topologySiteLink.Name;
			this.Cost = topologySiteLink.Cost;
			if (topologySiteLink.TopologySites == null || topologySiteLink.TopologySites.Count == 0)
			{
				this.Sites = Site.EmptyCollection;
				return;
			}
			List<Site> list = new List<Site>(topologySiteLink.TopologySites.Count);
			foreach (ITopologySite topologySite in topologySiteLink.TopologySites)
			{
				TopologySite topologySite2 = (TopologySite)topologySite;
				Site item = Site.Get(topologySite2, all);
				list.Add(item);
			}
			this.Sites = list.AsReadOnly();
		}

		public string DistinguishedName { get; private set; }

		public string Name { get; private set; }

		public int Cost { get; private set; }

		public ReadOnlyCollection<Site> Sites { get; internal set; }

		internal static SiteLink Get(TopologySiteLink topologySiteLink, ServiceTopology.All all)
		{
			SiteLink result;
			if (!all.SiteLinks.TryGetValue(topologySiteLink.DistinguishedName, out result))
			{
				result = new SiteLink(topologySiteLink, all);
			}
			return result;
		}

		internal static readonly ReadOnlyCollection<SiteLink> EmptyCollection = new List<SiteLink>().AsReadOnly();
	}
}
