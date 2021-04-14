using System;
using System.Collections.Generic;
using System.Configuration;

namespace Microsoft.Exchange.LogUploader
{
	[ConfigurationCollection(typeof(PartitionElement), AddItemName = "Partition")]
	public class PartitionsCollection : ConfigurationElementCollection
	{
		public PartitionsCollection() : base(StringComparer.InvariantCultureIgnoreCase)
		{
		}

		public void Add(PartitionElement element)
		{
			this.InsertElementList(element);
			this.BaseAdd(element);
		}

		public PartitionElement Get(object key)
		{
			return (PartitionElement)base.BaseGet(key);
		}

		public PartitionElement Get(int copyId, int partitionId)
		{
			if (this.elements.ContainsKey(copyId))
			{
				Dictionary<int, PartitionElement> dictionary = this.elements[copyId];
				if (dictionary.ContainsKey(partitionId))
				{
					return dictionary[partitionId];
				}
			}
			return (PartitionElement)base.BaseGet("Default");
		}

		public void BuildSearchList()
		{
			foreach (object obj in this)
			{
				PartitionElement e = (PartitionElement)obj;
				this.InsertElementList(e);
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new PartitionElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((PartitionElement)element).Name;
		}

		private void InsertElementList(PartitionElement e)
		{
			if (string.Compare(e.Name, "Default", true) != 0)
			{
				Dictionary<int, PartitionElement> dictionary;
				if (this.elements.ContainsKey(e.CopyId))
				{
					dictionary = this.elements[e.CopyId];
				}
				else
				{
					dictionary = new Dictionary<int, PartitionElement>();
					this.elements.Add(e.CopyId, dictionary);
				}
				dictionary.Add(e.PartitionId, e);
			}
		}

		private Dictionary<int, Dictionary<int, PartitionElement>> elements = new Dictionary<int, Dictionary<int, PartitionElement>>();
	}
}
