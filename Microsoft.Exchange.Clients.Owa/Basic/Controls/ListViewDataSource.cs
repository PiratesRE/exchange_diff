using System;
using System.Collections;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal abstract class ListViewDataSource
	{
		public ListViewDataSource(Hashtable properties, Folder folder)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			this.properties = properties;
			this.folder = folder;
		}

		public ListViewDataSource(Hashtable properties)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			this.properties = properties;
		}

		public Folder Folder
		{
			get
			{
				return this.folder;
			}
			set
			{
				this.folder = value;
			}
		}

		public object[][] Items
		{
			get
			{
				return this.items;
			}
			set
			{
				this.items = value;
			}
		}

		public string Cookie
		{
			get
			{
				return this.cookie;
			}
			set
			{
				this.cookie = value;
			}
		}

		public int StartRange
		{
			get
			{
				return this.startRange;
			}
			set
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
			set
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

		public virtual int TotalCount
		{
			get
			{
				return this.folder.ItemCount;
			}
		}

		public int UnreadCount
		{
			get
			{
				if (this.folder.TryGetProperty(FolderSchema.UnreadCount) is int)
				{
					return (int)this.folder[FolderSchema.UnreadCount];
				}
				return 0;
			}
		}

		public abstract void LoadData(int startRange, int endRange);

		public virtual int LoadData(StoreObjectId storeObjectId, int itemsPerPage)
		{
			return 0;
		}

		public object GetItemProperty(int item, PropertyDefinition propertyDefinition)
		{
			int num = (int)this.propertyIndices[propertyDefinition];
			return this.items[item][num];
		}

		public VersionedId GetItemPropertyVersionedId(int item, PropertyDefinition propertyDefinition)
		{
			return this.GetItemProperty(item, propertyDefinition) as VersionedId;
		}

		public string GetItemPropertyString(int item, PropertyDefinition propertyDefinition)
		{
			string text = this.GetItemProperty(item, propertyDefinition) as string;
			if (text == null)
			{
				return string.Empty;
			}
			return text;
		}

		public ExDateTime GetItemPropertyExDateTime(int item, PropertyDefinition propertyDefinition)
		{
			object itemProperty = this.GetItemProperty(item, propertyDefinition);
			if (itemProperty is DateTime)
			{
				throw new OwaInvalidInputException("List view item property must be ExDateTime not DateTime");
			}
			if (itemProperty is ExDateTime)
			{
				return (ExDateTime)itemProperty;
			}
			return ExDateTime.MinValue;
		}

		public int GetItemPropertyInt(int item, PropertyDefinition propertyDefinition, int defaultValue)
		{
			object itemProperty = this.GetItemProperty(item, propertyDefinition);
			if (!(itemProperty is int))
			{
				return defaultValue;
			}
			return (int)itemProperty;
		}

		public bool GetItemPropertyBool(int item, PropertyDefinition propertyDefinition, bool defaultValue)
		{
			object itemProperty = this.GetItemProperty(item, propertyDefinition);
			if (!(itemProperty is bool))
			{
				return defaultValue;
			}
			return (bool)itemProperty;
		}

		protected PropertyDefinition[] CreateProperyTable()
		{
			this.propertyIndices = (Hashtable)this.properties.Clone();
			PropertyDefinition[] array = new PropertyDefinition[this.propertyIndices.Count];
			int num = 0;
			IDictionaryEnumerator enumerator = this.properties.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PropertyDefinition propertyDefinition = (PropertyDefinition)enumerator.Key;
				array[num] = propertyDefinition;
				this.propertyIndices[propertyDefinition] = num++;
			}
			return array;
		}

		private Hashtable properties;

		private Hashtable propertyIndices;

		private Folder folder;

		private int startRange = int.MinValue;

		private int endRange = int.MinValue;

		private object[][] items;

		private string cookie;
	}
}
