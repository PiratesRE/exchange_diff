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
	internal interface IBirthdayEvents : IEntitySet<IBirthdayEvent>, IStorageEntitySetScope<IStoreSession>
	{
		BirthdayEventDataProvider BirthdayEventDataProvider { get; set; }

		IBirthdayCalendars ParentScope { get; }

		IEnumerable<BirthdayEvent> GetBirthdayCalendarView(ExDateTime rangeStart, ExDateTime rangeEnd);

		BirthdayEventCommandResult CreateBirthdayEventForContact(IBirthdayContact contact);

		BirthdayEventCommandResult DeleteBirthdayEventForContactId(StoreObjectId birthdayContactStoreObjectId);

		BirthdayEventCommandResult DeleteBirthdayEventForContact(IBirthdayContact birthdayContact);

		BirthdayEventCommandResult UpdateBirthdayEventForContact(IBirthdayEvent birthdayEvent, IBirthdayContact birthdayContact);

		BirthdayEventCommandResult UpdateBirthdaysForLinkedContacts(IEnumerable<IBirthdayContact> linkedContacts);
	}
}
