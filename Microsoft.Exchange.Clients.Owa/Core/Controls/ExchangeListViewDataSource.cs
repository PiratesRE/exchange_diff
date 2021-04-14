using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	public abstract class ExchangeListViewDataSource
	{
		protected ExchangeListViewDataSource(Hashtable properties)
		{
			this.propertyMap = new Dictionary<PropertyDefinition, int>(properties.Count);
			IDictionaryEnumerator enumerator = properties.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PropertyDefinition key = (PropertyDefinition)enumerator.Key;
				this.propertyMap[key] = 0;
			}
		}

		public abstract int TotalCount { get; }

		public virtual int TotalItemCount
		{
			get
			{
				return this.TotalCount;
			}
		}

		protected virtual bool IsPreviousItemLoaded
		{
			get
			{
				return false;
			}
		}

		internal virtual QueryResult QueryResult
		{
			get
			{
				throw new NotImplementedException("Not implemented by default. Implement in the derived class, if needed");
			}
		}

		internal PropertyDefinition[] GetRequestedProperties()
		{
			Dictionary<PropertyDefinition, int> dictionary = null;
			return this.GetRequestedProperties(false, ref dictionary);
		}

		internal PropertyDefinition[] GetRequestedProperties(bool getPropertyMap, ref Dictionary<PropertyDefinition, int> outPropertyMap)
		{
			PropertyDefinition[] array = new PropertyDefinition[this.propertyMap.Count];
			int num = 0;
			using (Dictionary<PropertyDefinition, int>.Enumerator enumerator = this.propertyMap.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PropertyDefinition[] array2 = array;
					int num2 = num;
					KeyValuePair<PropertyDefinition, int> keyValuePair = enumerator.Current;
					array2[num2] = keyValuePair.Key;
					num++;
				}
			}
			for (int i = 0; i < num; i++)
			{
				this.propertyMap[array[i]] = i;
			}
			if (getPropertyMap)
			{
				outPropertyMap = new Dictionary<PropertyDefinition, int>(this.propertyMap);
			}
			return array;
		}

		protected int PropertyIndex(PropertyDefinition propertyDefinition)
		{
			return this.propertyMap[propertyDefinition];
		}

		public virtual T GetItemProperty<T>(PropertyDefinition propertyDefinition) where T : class
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			if (!this.propertyMap.ContainsKey(propertyDefinition))
			{
				return default(T);
			}
			int num = this.IsPreviousItemLoaded ? (this.currentItem + 1) : this.currentItem;
			int num2 = this.propertyMap[propertyDefinition];
			return this.items[num][num2] as T;
		}

		public virtual T GetItemProperty<T>(PropertyDefinition propertyDefinition, T defaultValue)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			if (!this.propertyMap.ContainsKey(propertyDefinition))
			{
				return defaultValue;
			}
			int num = this.IsPreviousItemLoaded ? (this.currentItem + 1) : this.currentItem;
			int num2 = this.propertyMap[propertyDefinition];
			object obj = this.items[num][num2];
			if (obj == null || !(obj is T))
			{
				return defaultValue;
			}
			return (T)((object)obj);
		}

		public int GetPropertyIndex(PropertyDefinition propertyDefinition)
		{
			return this.propertyMap[propertyDefinition];
		}

		public int StartRange
		{
			get
			{
				return this.startRange;
			}
			protected set
			{
				this.startRange = value;
			}
		}

		public int EndRange
		{
			get
			{
				return this.endRange;
			}
			protected set
			{
				this.endRange = value;
			}
		}

		public int RangeCount
		{
			get
			{
				if (this.endRange < this.startRange || this.startRange == -2147483648 || this.endRange == -2147483648)
				{
					return 0;
				}
				return this.endRange - this.startRange + 1;
			}
		}

		public virtual int CurrentItem
		{
			get
			{
				return this.currentItem;
			}
		}

		public virtual bool MoveNext()
		{
			this.currentItem++;
			return this.currentItem < this.RangeCount;
		}

		public virtual void MoveToItem(int itemIndex)
		{
			if (itemIndex < -1 || (!this.IsPreviousItemLoaded && itemIndex < 0) || this.RangeCount <= itemIndex)
			{
				throw new ArgumentException("itemIndex=" + itemIndex.ToString(CultureInfo.CurrentCulture) + " is out of range.");
			}
			this.currentItem = itemIndex;
		}

		protected object[][] Items
		{
			set
			{
				this.items = value;
			}
		}

		protected void SetIndexer(int index)
		{
			this.currentItem = index;
		}

		public virtual object GetCurrentItem()
		{
			throw new NotImplementedException();
		}

		private Dictionary<PropertyDefinition, int> propertyMap;

		private object[][] items;

		private int startRange = int.MinValue;

		private int endRange = int.MinValue;

		private int currentItem = -1;
	}
}
