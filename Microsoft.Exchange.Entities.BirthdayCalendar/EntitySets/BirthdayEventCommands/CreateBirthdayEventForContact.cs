using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.BirthdayCalendar;
using Microsoft.Exchange.Entities.DataModel.BirthdayCalendar;
using Microsoft.Exchange.Entities.EntitySets.Commands;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.BirthdayCalendar.EntitySets.BirthdayEventCommands
{
	internal class CreateBirthdayEventForContact : EntityCommand<IBirthdayEvents, IBirthdayEvent>, IBirthdayEventCommand
	{
		internal CreateBirthdayEventForContact(IBirthdayContact birthdayContact, IBirthdayEvents scope)
		{
			this.Trace.TraceDebug((long)this.GetHashCode(), "CreateBirthdayEventForContact:Constructor");
			this.Contact = birthdayContact;
			this.Scope = scope;
		}

		public IBirthdayContact Contact { get; private set; }

		protected override ITracer Trace
		{
			get
			{
				return CreateBirthdayEventForContact.CreateBirthdayEventTracer;
			}
		}

		public BirthdayEventCommandResult ExecuteAndGetResult()
		{
			BirthdayEventCommandResult birthdayEventCommandResult = new BirthdayEventCommandResult();
			IBirthdayEvent birthdayEvent = base.Execute(null);
			if (birthdayEvent != null)
			{
				birthdayEventCommandResult.CreatedEvents.Add(birthdayEvent);
			}
			return birthdayEventCommandResult;
		}

		protected override IBirthdayEvent OnExecute()
		{
			IBirthdayEvent result;
			try
			{
				this.Scope.BirthdayEventDataProvider.BeforeStoreObjectSaved += this.OnBeforeStoreObjectSaved;
				this.Scope.BirthdayEventDataProvider.StoreObjectSaved += CreateBirthdayEventForContact.OnStoreObjectSaved;
				result = this.CreateNewBirthdayEventForContact(this.Contact);
			}
			finally
			{
				this.Scope.BirthdayEventDataProvider.StoreObjectSaved -= CreateBirthdayEventForContact.OnStoreObjectSaved;
				this.Scope.BirthdayEventDataProvider.BeforeStoreObjectSaved -= this.OnBeforeStoreObjectSaved;
			}
			return result;
		}

		private static void OnStoreObjectSaved(object sender, ICalendarItemBase calendarItemBase)
		{
			CreateBirthdayEventForContact.CreateBirthdayEventTracer.TraceDebug<ICalendarItemBase>(0L, "CreateBirthdayEventForContact::OnStoreObjectSaved: The birthday event calendar item was saved successfully: {0}", calendarItemBase);
		}

		private void OnBeforeStoreObjectSaved(BirthdayEvent birthdayEvent, ICalendarItemBase calendarItemBase)
		{
			CreateBirthdayEventForContact.CreateBirthdayEventTracer.TraceDebug<BirthdayEvent, ICalendarItemBase>(0L, "CreateBirthdayEventForContact::OnBeforeStoreObjectSaved: The birthday event {0} was created, and the calendar item {1} will be saved", birthdayEvent, calendarItemBase);
			calendarItemBase.FreeBusyStatus = BusyType.Free;
			calendarItemBase.IsAllDayEvent = true;
			ExDateTime exDateTime = birthdayEvent.Birthday.ToUtc();
			RecurrencePattern pattern = new YearlyRecurrencePattern(exDateTime.Day, exDateTime.Month);
			int year = birthdayEvent.IsBirthYearKnown ? exDateTime.Year : ExDateTime.Today.Year;
			ExTimeZone timeZone = this.DetermineRecurrenceStartTimeZone();
			ExDateTime startDate = new ExDateTime(timeZone, year, exDateTime.Month, exDateTime.Day);
			RecurrenceRange range = new NoEndRecurrenceRange(startDate);
			ICalendarItem calendarItem = calendarItemBase as ICalendarItem;
			if (calendarItem == null)
			{
				throw new NotSupportedException("Must be able to cast base to calendar item to specify recurrence.");
			}
			calendarItem.Recurrence = new Recurrence(pattern, range);
			calendarItem.ReminderMinutesBeforeStart = 1080;
			CreateBirthdayEventForContact.CreateBirthdayEventTracer.TraceDebug<ExDateTime, ExTimeZone>(0L, "CreateBirthdayEventForContact::OnBeforeStoreObjectSaved: recurrence start date is {0}, read time zone is {1}", calendarItem.Recurrence.Range.StartDate, calendarItem.Recurrence.ReadExTimeZone);
		}

		private ExTimeZone DetermineRecurrenceStartTimeZone()
		{
			MailboxSession mailboxSession = this.Scope.StoreSession as MailboxSession;
			return (mailboxSession == null) ? ExTimeZone.UtcTimeZone : TimeZoneHelper.GetUserTimeZone(mailboxSession);
		}

		private BirthdayEvent CreateNewBirthdayEventForContact(IBirthdayContact contact)
		{
			if (contact == null || contact.Birthday == null)
			{
				this.Trace.TraceDebug<IBirthdayContact>((long)this.GetHashCode(), "CreateBirthdayEventForContact::CreateNewBirthdayEvent: don't need to create a birthday for contact {0}", this.Contact);
				return null;
			}
			ExDateTime value = contact.Birthday.Value;
			this.Trace.TraceDebug<ExDateTime, TimeSpan>((long)this.GetHashCode(), "CreateBirthdayEventForContact::CreateNewBirthdayEvent: birthday value is {0}, time zone bias is {1}", value, value.Bias);
			BirthdayEvent birthdayEvent = new BirthdayEvent
			{
				Birthday = value,
				Subject = contact.DisplayName,
				Attribution = contact.Attribution,
				IsWritable = contact.IsWritable
			};
			IBirthdayEventInternal birthdayEventInternal = birthdayEvent;
			IBirthdayContactInternal birthdayContactInternal = (IBirthdayContactInternal)this.Contact;
			birthdayEventInternal.PersonId = birthdayContactInternal.PersonId;
			birthdayEventInternal.ContactId = StoreId.GetStoreObjectId(birthdayContactInternal.StoreId);
			return this.Scope.BirthdayEventDataProvider.CreateBirthday(birthdayEvent);
		}

		private static readonly ITracer CreateBirthdayEventTracer = ExTraceGlobals.CreateBirthdayEventForContactTracer;
	}
}
