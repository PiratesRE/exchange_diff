using System;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal abstract class AddressBookItemEventHandler : OwaEventHandlerBase
	{
		protected abstract void BindToData();

		protected abstract void LoadRecipientsToItem(Item item, AddressBookItemEventHandler.ItemTypeToPeople itemType);

		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(ContactItemEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(DirectoryItemEventHandler));
		}

		[OwaEventParameter("it", typeof(AddressBookItemEventHandler.ItemTypeToPeople), false)]
		[OwaEvent("MsgToPeople")]
		[OwaEventParameter("id", typeof(ObjectId), true)]
		public void NewItemToPeople()
		{
			ExTraceGlobals.ContactsCallTracer.TraceDebug((long)this.GetHashCode(), "AddressBookEventHandler.NewItemToPeople");
			this.BindToData();
			if (base.UserContext.UserOptions.ViewRowCount < this.itemIds.Length)
			{
				throw new OwaInvalidOperationException(string.Format("Sending message to more than {0} item(s) in a single request is not supported", this.itemIds.Length));
			}
			AddressBookItemEventHandler.ItemTypeToPeople itemTypeToPeople = (AddressBookItemEventHandler.ItemTypeToPeople)base.GetParameter("it");
			if (!Enum.IsDefined(typeof(AddressBookItemEventHandler.ItemTypeToPeople), itemTypeToPeople))
			{
				throw new OwaInvalidOperationException(string.Format("Invalid item type '{0}' passed in.", itemTypeToPeople));
			}
			using (Item item = (itemTypeToPeople == AddressBookItemEventHandler.ItemTypeToPeople.Meeting) ? CalendarItem.Create(base.UserContext.MailboxSession, base.UserContext.CalendarFolderId) : MessageItem.Create(base.UserContext.MailboxSession, base.UserContext.DraftsFolderId))
			{
				item[ItemSchema.ConversationIndexTracking] = true;
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.ItemsCreated.Increment();
				}
				this.LoadRecipientsToItem(item, itemTypeToPeople);
				if (itemTypeToPeople == AddressBookItemEventHandler.ItemTypeToPeople.Meeting)
				{
					CalendarItemBase calendarItemBase = (CalendarItemBase)item;
					calendarItemBase[ItemSchema.ReminderIsSet] = base.UserContext.UserOptions.EnableReminders;
					calendarItemBase[ItemSchema.ReminderMinutesBeforeStart] = base.UserContext.CalendarSettings.DefaultReminderTime;
					calendarItemBase.Save(SaveMode.ResolveConflicts);
					calendarItemBase.Load();
					this.Writer.Write("?ae=Item&a=New&t=IPM.Appointment&exdltdrft=1&id=");
					this.Writer.Write(HttpUtility.UrlEncode(calendarItemBase.Id.ObjectId.ToBase64String()));
				}
				else
				{
					MessageItem messageItem = (MessageItem)item;
					if (itemTypeToPeople == AddressBookItemEventHandler.ItemTypeToPeople.TextMessage)
					{
						messageItem[StoreObjectSchema.ItemClass] = "IPM.Note.Mobile.SMS";
					}
					messageItem.Save(SaveMode.ResolveConflicts);
					messageItem.Load();
					this.Writer.Write((itemTypeToPeople == AddressBookItemEventHandler.ItemTypeToPeople.TextMessage) ? "?ae=Item&a=Reply&t=IPM.Note.Mobile.SMS&exdltdrft=1&id=" : "?ae=Item&a=New&t=IPM.Note&exdltdrft=1&id=");
					this.Writer.Write(HttpUtility.UrlEncode(messageItem.Id.ObjectId.ToBase64String()));
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AddressBookItemEventHandler>(this);
		}

		public const string MethodNewItemToPeople = "MsgToPeople";

		public const string Id = "id";

		public const string ItemType = "it";

		protected ObjectId[] itemIds;

		internal enum ItemTypeToPeople
		{
			Message,
			TextMessage,
			Meeting
		}
	}
}
