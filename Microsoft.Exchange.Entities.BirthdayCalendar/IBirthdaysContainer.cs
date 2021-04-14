using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.BirthdayCalendar
{
	internal interface IBirthdaysContainer : IStorageEntitySetScope<IMailboxSession>
	{
		IBirthdayCalendars Calendars { get; }

		IBirthdayContacts Contacts { get; }

		IBirthdayEvents Events { get; }
	}
}
