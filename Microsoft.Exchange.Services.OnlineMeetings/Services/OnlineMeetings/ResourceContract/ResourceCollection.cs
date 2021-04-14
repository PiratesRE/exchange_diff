using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	internal class ResourceCollection<T> : Resource, IEnumerable<T>, IEnumerable, IResourceCollection where T : Resource
	{
		public ResourceCollection() : base("collection")
		{
		}

		public ResourceCollection(IEnumerable<Resource> items) : this()
		{
			foreach (Resource target in items)
			{
				this.AddItem(target);
			}
		}

		public override bool CanBeEmbedded
		{
			get
			{
				return this.items.All((Link link) => link.CanBeEmbedded);
			}
		}

		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		public void AddItem(object target = null)
		{
			this.items.Add(new Link("ignore-this-token", "ignore-this-relationship", "ignore")
			{
				Target = target
			});
		}

		public void Add(object target = null)
		{
			this.AddItem(target);
		}

		public override object ToDictionary(List<EmbeddedPart> mimeParts)
		{
			if (this.CanBeEmbedded)
			{
				return (from link in this.items
				select ((Resource)link.Target).ToDictionary(mimeParts)).ToArray<object>();
			}
			return (from link in this.items
			select new Dictionary<string, string>
			{
				{
					"href",
					link.Href
				}
			}).ToArray<Dictionary<string, string>>();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return (from link in this.items
			select link.Target).Cast<T>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly List<Link> items = new List<Link>();
	}
}
