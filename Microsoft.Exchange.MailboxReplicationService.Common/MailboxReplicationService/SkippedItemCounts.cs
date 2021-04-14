using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class SkippedItemCounts : XMLSerializableBase
	{
		public SkippedItemCounts()
		{
			this.counts = new Dictionary<SkippedItemCounts.CategoryKey, int>();
		}

		[XmlAttribute(AttributeName = "Corrupt")]
		public int CorruptCount { get; set; }

		[XmlAttribute(AttributeName = "Missing")]
		public int MissingCount { get; set; }

		[XmlAttribute(AttributeName = "Large")]
		public int LargeCount { get; set; }

		[XmlAttribute(AttributeName = "Other")]
		public int OtherCount { get; set; }

		[XmlElement(ElementName = "C")]
		public SkippedItemCounts.CategoryCount[] Counts
		{
			get
			{
				if (this.counts == null || this.counts.Count == 0)
				{
					return null;
				}
				List<SkippedItemCounts.CategoryCount> list = new List<SkippedItemCounts.CategoryCount>(this.counts.Count);
				foreach (KeyValuePair<SkippedItemCounts.CategoryKey, int> keyValuePair in this.counts)
				{
					list.Add(new SkippedItemCounts.CategoryCount(keyValuePair.Key, keyValuePair.Value));
				}
				return list.ToArray();
			}
			set
			{
				this.counts.Clear();
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						SkippedItemCounts.CategoryCount categoryCount = value[i];
						if (categoryCount != null)
						{
							this.counts[categoryCount.Key] = categoryCount.Count;
						}
					}
				}
			}
		}

		public void AddBadItem(BadMessageRec badItem)
		{
			this.AccountBadItem(badItem);
		}

		public void RecordMissingItems(List<FolderSizeRec> verificationResults)
		{
			this.MissingCount = 0;
			List<SkippedItemCounts.CategoryKey> list = new List<SkippedItemCounts.CategoryKey>();
			foreach (SkippedItemCounts.CategoryKey categoryKey in this.counts.Keys)
			{
				if (categoryKey.Kind == BadItemKind.MissingItem)
				{
					list.Add(categoryKey);
				}
			}
			foreach (SkippedItemCounts.CategoryKey key in list)
			{
				this.counts.Remove(key);
			}
			foreach (FolderSizeRec folderSizeRec in verificationResults)
			{
				if (folderSizeRec.MissingItems != null)
				{
					foreach (BadMessageRec item in folderSizeRec.MissingItems)
					{
						this.AccountBadItem(item);
					}
				}
			}
		}

		private void AccountBadItem(BadMessageRec item)
		{
			switch (item.Kind)
			{
			case BadItemKind.MissingItem:
				this.MissingCount++;
				break;
			case BadItemKind.CorruptItem:
				this.CorruptCount++;
				break;
			case BadItemKind.LargeItem:
				this.LargeCount++;
				break;
			default:
				this.OtherCount++;
				break;
			}
			SkippedItemCounts.CategoryKey key = new SkippedItemCounts.CategoryKey
			{
				Kind = item.Kind,
				Category = item.Category
			};
			int num;
			if (!this.counts.TryGetValue(key, out num))
			{
				num = 0;
			}
			this.counts[key] = num + 1;
		}

		private Dictionary<SkippedItemCounts.CategoryKey, int> counts;

		internal class CategoryKey : IEquatable<SkippedItemCounts.CategoryKey>
		{
			public BadItemKind Kind { get; set; }

			public string Category { get; set; }

			public override int GetHashCode()
			{
				return this.Kind.GetHashCode() ^ ((this.Category != null) ? this.Category.GetHashCode() : 0);
			}

			public override bool Equals(object o)
			{
				return ((IEquatable<SkippedItemCounts.CategoryKey>)this).Equals(o as SkippedItemCounts.CategoryKey);
			}

			bool IEquatable<SkippedItemCounts.CategoryKey>.Equals(SkippedItemCounts.CategoryKey other)
			{
				return other != null && other.Kind == this.Kind && StringComparer.InvariantCultureIgnoreCase.Equals(this.Category, other.Category);
			}
		}

		public sealed class CategoryCount : XMLSerializableBase
		{
			public CategoryCount()
			{
				this.Key = new SkippedItemCounts.CategoryKey();
			}

			internal CategoryCount(SkippedItemCounts.CategoryKey key, int count)
			{
				this.Key = key;
				this.Count = count;
			}

			[XmlIgnore]
			internal SkippedItemCounts.CategoryKey Key { get; private set; }

			[XmlAttribute(AttributeName = "Kind")]
			public string KindStr
			{
				get
				{
					return this.Key.Kind.ToString();
				}
				set
				{
				}
			}

			[XmlAttribute(AttributeName = "KindInt")]
			public int KindInt
			{
				get
				{
					return (int)this.Key.Kind;
				}
				set
				{
					this.Key.Kind = (BadItemKind)value;
				}
			}

			[XmlAttribute(AttributeName = "Cat")]
			public string Category
			{
				get
				{
					return this.Key.Category;
				}
				set
				{
					this.Key.Category = value;
				}
			}

			[XmlAttribute(AttributeName = "Num")]
			public int Count { get; set; }
		}
	}
}
