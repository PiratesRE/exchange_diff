using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class ListViewNotificationDataSource : IListViewDataSource
	{
		public ListViewNotificationDataSource(UserContext userContext, StoreObjectId folderId, bool conversationMode, Dictionary<PropertyDefinition, int> propertyIndices, SortBy[] sortBy, QueryNotification notification)
		{
			this.userContext = userContext;
			this.propertyIndices = propertyIndices;
			this.notification = notification;
			this.folderId = folderId;
			this.sortBy = sortBy;
			this.conversationMode = conversationMode;
		}

		public Folder Folder
		{
			get
			{
				return null;
			}
		}

		public SortBy[] SortBy
		{
			get
			{
				return this.sortBy;
			}
		}

		public virtual int TotalCount
		{
			get
			{
				return 1;
			}
		}

		public virtual int TotalItemCount
		{
			get
			{
				return this.TotalCount;
			}
		}

		public int UnreadCount
		{
			get
			{
				return 0;
			}
		}

		public string ContainerId
		{
			get
			{
				return this.folderId.ToBase64String();
			}
		}

		public bool UserHasRightToLoad
		{
			get
			{
				return true;
			}
		}

		public void Load(int startRange, int itemCount)
		{
		}

		public void Load(ObjectId seekToObjectId, SeekDirection seekDirection, int itemCount)
		{
		}

		public void Load(string seekValue, int itemCount)
		{
		}

		public bool LoadAdjacent(ObjectId adjacentObjectId, SeekDirection seekDirection, int itemCount)
		{
			return true;
		}

		public object GetItemProperty(int item, PropertyDefinition propertyDefinition)
		{
			int num = this.propertyIndices[propertyDefinition];
			return this.notification.Row[num];
		}

		public virtual T GetItemProperty<T>(PropertyDefinition propertyDefinition, T defaultValue)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			if (!this.propertyIndices.ContainsKey(propertyDefinition))
			{
				return defaultValue;
			}
			int num = this.propertyIndices[propertyDefinition];
			object obj = this.notification.Row[num];
			if (obj == null || !(obj is T))
			{
				return defaultValue;
			}
			if (obj is ExDateTime)
			{
				obj = this.userContext.TimeZone.ConvertDateTime((ExDateTime)obj);
			}
			return (T)((object)obj);
		}

		public virtual T GetItemProperty<T>(PropertyDefinition propertyDefinition) where T : class
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			int num = this.propertyIndices[propertyDefinition];
			return this.notification.Row[num] as T;
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

		public string GetChangeKey()
		{
			VersionedId itemProperty = this.GetItemProperty<VersionedId>(ItemSchema.Id);
			return itemProperty.ChangeKeyAsBase64String();
		}

		public string GetItemClass()
		{
			if (this.conversationMode)
			{
				return "IPM.Conversation";
			}
			string text = this.GetItemProperty<string>(StoreObjectSchema.ItemClass, null);
			if (string.IsNullOrEmpty(text))
			{
				text = "IPM.Unknown";
			}
			return text;
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
				return 1;
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
			return true;
		}

		public virtual void MoveToItem(int itemIndex)
		{
		}

		protected void SetIndexer(int index)
		{
			this.currentItem = index;
		}

		public virtual object GetCurrentItem()
		{
			throw new NotImplementedException();
		}

		public string GetItemId()
		{
			if (this.conversationMode)
			{
				ConversationId itemProperty = this.GetItemProperty<ConversationId>(ConversationItemSchema.ConversationId);
				byte[] itemProperty2 = this.GetItemProperty<byte[]>(ItemSchema.InstanceKey);
				if (itemProperty != null)
				{
					return OwaStoreObjectId.CreateFromConversationIdForListViewNotification(itemProperty, this.folderId, itemProperty2).ToString();
				}
				return null;
			}
			else
			{
				VersionedId itemProperty3 = this.GetItemProperty<VersionedId>(ItemSchema.Id);
				if (itemProperty3 != null)
				{
					return OwaStoreObjectId.CreateFromItemId(itemProperty3.ObjectId, this.folderId, OwaStoreObjectIdType.MailBoxObject, null).ToString();
				}
				return null;
			}
		}

		private const string ConversationItemType = "IPM.Conversation";

		private const int TotalCountValue = 1;

		private UserContext userContext;

		private Dictionary<PropertyDefinition, int> propertyIndices;

		private StoreObjectId folderId;

		private int startRange = int.MinValue;

		private int endRange = int.MinValue;

		private int currentItem;

		private bool conversationMode;

		private SortBy[] sortBy;

		private QueryNotification notification;
	}
}
