using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets
{
	internal class EventReference : ItemReference<Event>, IEventReference, IItemReference<Event>, IEntityReference<Event>, ILocalCalendarReference, ICalendarReference, IEntityReference<Calendar>
	{
		public EventReference(IStorageEntitySetScope<IStoreSession> parentScope, string eventId) : base(parentScope, eventId.AssertNotNull("eventId"))
		{
		}

		internal EventReference(IXSOFactory xsoFactory, ICalendarItemBase calendarItem) : base(new StorageEntitySetScope<IStoreSession>(calendarItem.Session, calendarItem.Session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid), xsoFactory, null), calendarItem.Id.AssertNotNull("calendarItem.Id"), calendarItem.Session)
		{
		}

		internal EventReference(IXSOFactory xsoFactory, IStoreSession session, string id) : this(new StorageEntitySetScope<IStoreSession>(session, session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid), xsoFactory, null), id)
		{
		}

		public ICalendarReference Calendar
		{
			get
			{
				return this;
			}
		}

		public IEvents Events
		{
			get
			{
				IEvents result;
				if ((result = this.events) == null)
				{
					result = (this.events = new Events(this, this));
				}
				return result;
			}
		}

		public StoreId GetCalendarFolderId()
		{
			return base.GetContainingFolderId();
		}

		Calendar IEntityReference<Calendar>.Read(CommandContext context)
		{
			IMailboxSession storeSession = (IMailboxSession)base.StoreSession;
			StorageEntitySetScope<IMailboxSession> storageEntitySetScope = new StorageEntitySetScope<IMailboxSession>(storeSession, base.RecipientSession, base.XsoFactory, null);
			string key = this.Calendar.GetKey();
			MailboxCalendars mailboxCalendars = new MailboxCalendars(storageEntitySetScope, new CalendarGroups(storageEntitySetScope, null).MyCalendars);
			return mailboxCalendars.Read(key, context);
		}

		Event IEntityReference<Event>.Read(CommandContext context)
		{
			return this.Events.Read(base.ItemKey, context);
		}

		string IEntityReference<Calendar>.GetKey()
		{
			return base.IdConverter.ToStringId(base.GetContainingFolderId(), base.StoreSession);
		}

		protected override AttachmentDataProvider GetAttachmentDataProvider()
		{
			return new EventAttachmentDataProvider(this, base.IdConverter.ToStoreObjectId(base.ItemKey));
		}

		private IEvents events;
	}
}
