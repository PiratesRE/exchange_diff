using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class Site : IComparable<Site>, IEquatable<Site>
	{
		public Site(TopologySite topologySite) : this(topologySite, new ServiceTopology.All(null))
		{
		}

		internal Site(TopologySite topologySite, ServiceTopology.All all)
		{
			this.DistinguishedName = topologySite.DistinguishedName;
			all.Sites.Add(this.DistinguishedName, this);
			this.Id = topologySite.Id;
			this.Guid = topologySite.Guid;
			this.Name = topologySite.Name;
			this.PartnerId = topologySite.PartnerId;
			this.MinorPartnerId = topologySite.MinorPartnerId;
			List<ADObjectId> list = new List<ADObjectId>(topologySite.ResponsibleForSites);
			this.ResponsibleForSites = list.AsReadOnly();
			if (topologySite.TopologySiteLinks == null || topologySite.TopologySiteLinks.Count == 0)
			{
				this.SiteLinks = SiteLink.EmptyCollection;
				return;
			}
			List<SiteLink> list2 = new List<SiteLink>(topologySite.TopologySiteLinks.Count);
			foreach (ITopologySiteLink topologySiteLink in topologySite.TopologySiteLinks)
			{
				TopologySiteLink topologySiteLink2 = (TopologySiteLink)topologySiteLink;
				SiteLink item = SiteLink.Get(topologySiteLink2, all);
				list2.Add(item);
			}
			this.SiteLinks = list2.AsReadOnly();
		}

		public string DistinguishedName { get; private set; }

		public ICollection<ADObjectId> ResponsibleForSites { get; private set; }

		public ADObjectId Id { get; private set; }

		public string Name { get; private set; }

		public Guid Guid { get; private set; }

		public int PartnerId { get; private set; }

		public int MinorPartnerId { get; private set; }

		public ReadOnlyCollection<SiteLink> SiteLinks { get; internal set; }

		public bool Equals(Site other)
		{
			return other != null && (object.ReferenceEquals(this, other) || this.DistinguishedName.Equals(other.DistinguishedName));
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as Site);
		}

		public override int GetHashCode()
		{
			return this.DistinguishedName.GetHashCode();
		}

		public override string ToString()
		{
			return this.DistinguishedName;
		}

		public int CompareTo(Site site)
		{
			if (site == null)
			{
				throw new ArgumentException();
			}
			int num = this.GetHashCode() - site.GetHashCode();
			if (num == 0)
			{
				num = this.DistinguishedName.CompareTo(site.DistinguishedName);
			}
			return num;
		}

		internal static Site Get(TopologySite topologySite, ServiceTopology.All all)
		{
			Site result;
			if (!all.Sites.TryGetValue(topologySite.DistinguishedName, out result))
			{
				result = new Site(topologySite, all);
			}
			return result;
		}

		internal static readonly ReadOnlyCollection<Site> EmptyCollection = new List<Site>().AsReadOnly();
	}
}
