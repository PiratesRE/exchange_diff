using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets
{
	internal interface IBirthdayCalendars : IEntitySet<IBirthdayCalendar>, IStorageEntitySetScope<IMailboxSession>
	{
		CalendarFolderDataProvider CalendarFolderDataProvider { get; }

		IBirthdaysContainer ParentScope { get; }

		StoreId BirthdayCalendarFolderId { get; }
	}
}
