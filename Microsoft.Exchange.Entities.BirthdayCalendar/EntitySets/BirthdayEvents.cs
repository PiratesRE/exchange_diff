using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.BirthdayCalendar.DataProviders;
using Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets.BirthdayEventCommands;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.BirthdayCalendar;
using Microsoft.Exchange.Entities.EntitySets;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets
{
	internal class BirthdayEvents : StorageEntitySet<IBirthdayEvents, IBirthdayEvent, IStoreSession>, IBirthdayEvents, IEntitySet<IBirthdayEvent>, IStorageEntitySetScope<IStoreSession>
	{
		public BirthdayEvents(IBirthdayCalendars parentScope) : base(parentScope, "BirthdayEvents", new SimpleCrudNotSupportedCommandFactory<IBirthdayEvents, IBirthdayEvent>())
		{
			this.ParentScope = parentScope;
		}

		public BirthdayEventDataProvider BirthdayEventDataProvider
		{
			get
			{
				BirthdayEventDataProvider result;
				if ((result = this.eventDataProvider) == null)
				{
					result = (this.eventDataProvider = new BirthdayEventDataProvider(this, this.ParentScope.BirthdayCalendarFolderId));
				}
				return result;
			}
			set
			{
				this.eventDataProvider = value;
			}
		}

		public IBirthdayCalendars ParentScope { get; private set; }

		public IEnumerable<BirthdayEvent> GetBirthdayCalendarView(ExDateTime rangeStart, ExDateTime rangeEnd)
		{
			return new GetBirthdayCalendarView(this, rangeStart, rangeEnd).Execute(null);
		}

		public BirthdayEventCommandResult CreateBirthdayEventForContact(IBirthdayContact contact)
		{
			return new CreateBirthdayEventForContact(contact, this).ExecuteAndGetResult();
		}

		public BirthdayEventCommandResult DeleteBirthdayEventForContactId(StoreObjectId birthdayContactStoreObjectId)
		{
			return new DeleteBirthdayEventForContact(birthdayContactStoreObjectId, this).ExecuteAndGetResult();
		}

		public BirthdayEventCommandResult DeleteBirthdayEventForContact(IBirthdayContact birthdayContact)
		{
			return new DeleteBirthdayEventForContact(birthdayContact, this).ExecuteAndGetResult();
		}

		public BirthdayEventCommandResult UpdateBirthdayEventForContact(IBirthdayEvent birthdayEvent, IBirthdayContact birthdayContact)
		{
			return new UpdateBirthdayEventForContact(birthdayEvent, birthdayContact, this).ExecuteAndGetResult();
		}

		public BirthdayEventCommandResult UpdateBirthdaysForLinkedContacts(IEnumerable<IBirthdayContact> linkedContacts)
		{
			return new UpdateBirthdaysForLinkedContacts(linkedContacts, this).ExecuteAndGetResult();
		}

		private BirthdayEventDataProvider eventDataProvider;
	}
}
