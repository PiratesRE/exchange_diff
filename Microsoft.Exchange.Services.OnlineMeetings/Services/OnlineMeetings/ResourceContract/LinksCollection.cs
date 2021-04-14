using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	internal class LinksCollection : ICollection<Link>, IEnumerable<Link>, IEnumerable
	{
		internal LinksCollection(List<Link> links)
		{
			this.links = links;
		}

		public int Count
		{
			get
			{
				return this.links.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Add(Link item)
		{
			string relationship = item.Relationship;
			if (relationship == "related")
			{
				item = new Link(item)
				{
					Relationship = item.Token
				};
			}
			if (relationship == "self")
			{
				this.links.RemoveAll((Link link) => link.Relationship == "self");
				item = new Link(item)
				{
					Token = "self"
				};
			}
			this.links.Add(item);
		}

		public void Clear()
		{
			this.links.Clear();
		}

		public bool Contains(Link item)
		{
			return this.links.Contains(item);
		}

		public void CopyTo(Link[] array, int arrayIndex)
		{
			this.links.CopyTo(array, arrayIndex);
		}

		public bool Remove(Link item)
		{
			return this.links.Remove(item);
		}

		public IEnumerator<Link> GetEnumerator()
		{
			return this.links.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly List<Link> links;
	}
}
