using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct Properties : IEnumerable<Property>, IEnumerable
	{
		public Properties(int initialCapacity)
		{
			this.properties = new List<Property>(initialCapacity);
		}

		public Properties(Properties properties)
		{
			this.properties = new List<Property>(properties.Count);
			foreach (Property prop in properties)
			{
				this.Add(prop);
			}
		}

		public Properties(IList<StorePropTag> tags, IList<object> values)
		{
			if (tags == null)
			{
				this.properties = new List<Property>(10);
				return;
			}
			this.properties = new List<Property>(tags.Count);
			for (int i = 0; i < tags.Count; i++)
			{
				this.Add(tags[i], (values != null) ? values[i] : null);
			}
		}

		public Properties(IEnumerable<Property> properties)
		{
			this.properties = new List<Property>(10);
			foreach (Property prop in properties)
			{
				this.Add(prop);
			}
		}

		public int Count
		{
			get
			{
				if (this.properties != null)
				{
					return this.properties.Count;
				}
				return 0;
			}
		}

		public Property this[int i]
		{
			get
			{
				return this.properties[i];
			}
			set
			{
				this.properties[i] = value;
			}
		}

		public static void RemoveFrom(List<Property> list, StorePropTag tag)
		{
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].Tag == tag)
					{
						list.RemoveAt(i);
						return;
					}
				}
			}
		}

		public static bool Contains(List<Property> list, StorePropTag propTag)
		{
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].Tag == propTag)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void Add(StorePropTag tag, object value)
		{
			this.properties.Add(new Property(tag, value));
		}

		public void Add(Property prop)
		{
			this.properties.Add(prop);
		}

		public void AddOrReplace(StorePropTag tag, object value)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (tag == this.properties[i].Tag)
				{
					this.properties[i] = new Property(tag, value);
					return;
				}
			}
			this.Add(tag, value);
		}

		public void AddOrReplace(Property prop)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (prop.Tag == this.properties[i].Tag)
				{
					this.properties[i] = prop;
					return;
				}
			}
			this.Add(prop);
		}

		public void AddOrReplace(IEnumerable<Property> properties)
		{
			foreach (Property prop in properties)
			{
				this.AddOrReplace(prop);
			}
		}

		public void Remove(StorePropTag tag)
		{
			Properties.RemoveFrom(this.properties, tag);
		}

		public void Remove(IEnumerable<StorePropTag> tags)
		{
			foreach (StorePropTag tag in tags)
			{
				this.Remove(tag);
			}
		}

		public void RemoveAt(int index)
		{
			this.properties.RemoveAt(index);
		}

		public void Clear()
		{
			this.properties.Clear();
		}

		public bool Contains(StorePropTag propTag)
		{
			return Properties.Contains(this.properties, propTag);
		}

		public Property GetProperty(StorePropTag propTag)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this.properties[i].Tag == propTag)
				{
					return this.properties[i];
				}
			}
			return Property.NotFoundError(propTag);
		}

		public object GetValue(StorePropTag propTag)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this.properties[i].Tag == propTag)
				{
					return this.properties[i].Value;
				}
			}
			return null;
		}

		public StorePropTag[] GetPropTags()
		{
			StorePropTag[] array = new StorePropTag[this.Count];
			for (int i = 0; i < this.Count; i++)
			{
				array[i] = this.properties[i].Tag;
			}
			return array;
		}

		public List<Property>.Enumerator GetEnumerator()
		{
			if (this.properties == null)
			{
				return Properties.emptyProperties.GetEnumerator();
			}
			return this.properties.GetEnumerator();
		}

		IEnumerator<Property> IEnumerable<Property>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void AppendToString(StringBuilder sb)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (i != 0)
				{
					sb.Append(" ");
				}
				this.properties[i].AppendToString(sb);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(this.Count * 30);
			this.AppendToString(stringBuilder);
			return stringBuilder.ToString();
		}

		private const int DefaultCapacity = 10;

		public static readonly Properties Empty = default(Properties);

		private static List<Property> emptyProperties = new List<Property>(0);

		private List<Property> properties;
	}
}
