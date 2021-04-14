using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.BirthdayCalendar;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BirthdayCalendars : StorageEntitySet<IBirthdayCalendars, IBirthdayCalendar, IMailboxSession>, IBirthdayCalendars, IEntitySet<IBirthdayCalendar>, IStorageEntitySetScope<IMailboxSession>
	{
		public BirthdayCalendars(IBirthdaysContainer parentScope, IEntityCommandFactory<IBirthdayCalendars, IBirthdayCalendar> commandFactory = null) : base(parentScope, "BirthdayCalendars", commandFactory)
		{
			this.ParentScope = parentScope;
		}

		public CalendarFolderDataProvider CalendarFolderDataProvider
		{
			get
			{
				CalendarFolderDataProvider result;
				if ((result = this.birthdayCalendarFolderDataProvider) == null)
				{
					result = (this.birthdayCalendarFolderDataProvider = new CalendarFolderDataProvider(this, base.Session.GetDefaultFolderId(DefaultFolderType.Calendar)));
				}
				return result;
			}
		}

		public IBirthdaysContainer ParentScope { get; private set; }

		public StoreId BirthdayCalendarFolderId
		{
			get
			{
				ExTraceGlobals.BirthdayCalendarsTracer.TraceDebug<Guid>((long)this.GetHashCode(), "BirthdayCalendars::GetBirthdayCalendarFolderId. GetDefaultFolderId. MailboxGuid:{0}", base.StoreSession.MailboxGuid);
				StoreObjectId defaultFolderId = base.StoreSession.GetDefaultFolderId(DefaultFolderType.BirthdayCalendar);
				ExTraceGlobals.BirthdayCalendarsTracer.TraceDebug<StoreObjectId, Guid>((long)this.GetHashCode(), "BirthdayCalendars::GetBirthdayCalendarFolderId. FolderId: {0} MailboxGuid:{1}", defaultFolderId, base.StoreSession.MailboxGuid);
				return defaultFolderId;
			}
		}

		private CalendarFolderDataProvider birthdayCalendarFolderDataProvider;
	}
}
