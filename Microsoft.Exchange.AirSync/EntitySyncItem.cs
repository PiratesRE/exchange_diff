using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.AirSync
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EntitySyncItem : MailboxSyncItem
	{
		protected EntitySyncItem(Item item) : base(item)
		{
		}

		protected EntitySyncItem(IItem item) : base(null)
		{
			this.Item = item;
		}

		public IItem Item
		{
			get
			{
				base.CheckDisposed("get_Item");
				if (this.item == null)
				{
					Item item = base.NativeItem as Item;
					if (item == null)
					{
						return null;
					}
					this.item = EntitySyncItem.GetItem(item);
				}
				return this.item;
			}
			private set
			{
				if (!object.ReferenceEquals(this.item, value))
				{
					IDisposable disposable = this.item as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
					this.item = value;
				}
			}
		}

		public new static EntitySyncItem Bind(Item item)
		{
			return new EntitySyncItem(item);
		}

		public static EntitySyncItem Bind(IItem item)
		{
			return new EntitySyncItem(item);
		}

		public static IItem GetItem(Item xsoItem)
		{
			string key = EntitySyncItem.GetKey(xsoItem.Session.MailboxGuid, xsoItem.Id.ObjectId);
			IEvents events = EntitySyncItem.GetEvents(xsoItem.Session, xsoItem.Id.ObjectId);
			return events.Read(key, null);
		}

		public static string GetKey(Guid mailboxGuid, StoreId itemId)
		{
			return StoreId.StoreIdToEwsId(mailboxGuid, itemId);
		}

		public static IEvents GetEvents(IStoreSession storeSession, StoreObjectId itemId)
		{
			return EntitySyncItem.GetEvents(new CalendaringContainer(storeSession, null), storeSession, itemId);
		}

		public static IEvents GetEvents(ICalendaringContainer calendaringContainer, IStoreSession storeSession, StoreObjectId itemId)
		{
			StoreObjectId parentFolderId = storeSession.GetParentFolderId(itemId);
			IdConverter instance = IdConverter.Instance;
			string calendarId = instance.ToStringId(parentFolderId, storeSession);
			return calendaringContainer.Calendars[calendarId].Events;
		}

		public void Reload()
		{
			Item item = base.NativeItem as Item;
			if (item != null)
			{
				StoreObjectId objectId = item.Id.ObjectId;
				StoreSession session = item.Session;
				item.Dispose();
				base.NativeItem = Microsoft.Exchange.Data.Storage.Item.Bind(session, objectId, EntitySyncItem.WatermarkProperties);
			}
			this.Item = null;
		}

		public void UpdateId(EntitySyncProviderFactory factory, string itemId)
		{
			base.CheckDisposed("get_NativeItem");
			if (base.NativeItem != null)
			{
				throw new InvalidOperationException("The sync item already has NativeItem");
			}
			StoreObjectId storeId = StoreId.EwsIdToFolderStoreObjectId(itemId);
			base.NativeItem = Microsoft.Exchange.Data.Storage.Item.Bind(factory.StoreSession, storeId, EntitySyncItem.WatermarkProperties);
			this.Item = null;
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.Item = null;
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		internal static readonly PropertyDefinition[] WatermarkProperties = new PropertyDefinition[]
		{
			ItemSchema.ArticleId,
			MessageItemSchema.IsRead
		};

		private IItem item;
	}
}
