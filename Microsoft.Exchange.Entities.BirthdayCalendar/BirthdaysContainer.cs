using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.BirthdayCalendar
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class BirthdaysContainer : StorageEntitySetScope<IMailboxSession>, IBirthdaysContainer, IStorageEntitySetScope<IMailboxSession>
	{
		public BirthdaysContainer(IStorageEntitySetScope<IMailboxSession> parentScope) : base(parentScope)
		{
		}

		public BirthdaysContainer(IStoreSession session, IXSOFactory xsoFactory = null) : this(new StorageEntitySetScope<IMailboxSession>((IMailboxSession)session, session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid), xsoFactory ?? XSOFactory.Default, null))
		{
		}

		public IBirthdayCalendars Calendars
		{
			get
			{
				BirthdayCalendars result;
				if ((result = this.calendars) == null)
				{
					result = (this.calendars = new BirthdayCalendars(this, null));
				}
				return result;
			}
		}

		public virtual IBirthdayEvents Events
		{
			get
			{
				IBirthdayEvents result;
				if ((result = this.events) == null)
				{
					result = (this.events = new BirthdayEvents(this.Calendars));
				}
				return result;
			}
		}

		public IBirthdayContacts Contacts
		{
			get
			{
				IBirthdayContacts result;
				if ((result = this.contacts) == null)
				{
					result = (this.contacts = new BirthdayContacts(this));
				}
				return result;
			}
		}

		private BirthdayCalendars calendars;

		private IBirthdayEvents events;

		private IBirthdayContacts contacts;
	}
}
