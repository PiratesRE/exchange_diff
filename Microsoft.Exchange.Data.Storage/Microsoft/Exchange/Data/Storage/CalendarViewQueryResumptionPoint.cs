using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CalendarViewQueryResumptionPoint : QueryResumptionPoint<ExDateTime>
	{
		private CalendarViewQueryResumptionPoint(bool resumeToRecurringMeetings, byte[] instanceKey, ExDateTime sortKeyValue, bool hasSortKeyValue) : base(instanceKey, resumeToRecurringMeetings ? CalendarFolder.RecurringMeetingsSortKey : CalendarFolder.SingleInstanceMeetingsSortKey, sortKeyValue, hasSortKeyValue)
		{
			this.resumeToRecurringMeetings = resumeToRecurringMeetings;
		}

		public static string CurrentVersion
		{
			get
			{
				return QueryResumptionPoint<ExDateTime>.GetVersion("0");
			}
		}

		public bool ResumeToSingleInstanceMeetings
		{
			get
			{
				return !this.resumeToRecurringMeetings;
			}
		}

		public bool ResumeToRecurringMeetings
		{
			get
			{
				return this.resumeToRecurringMeetings;
			}
		}

		public override bool IsEmpty
		{
			get
			{
				return base.IsEmpty && !this.resumeToRecurringMeetings;
			}
		}

		public static CalendarViewQueryResumptionPoint CreateInstance(bool resumeToRecurringMeetings, byte[] instanceKey, ExDateTime? sortKeyValue)
		{
			return new CalendarViewQueryResumptionPoint(resumeToRecurringMeetings, instanceKey, sortKeyValue ?? default(ExDateTime), sortKeyValue != null);
		}

		protected override string MinorVersion
		{
			get
			{
				return "0";
			}
		}

		private const string CurrentMinorVersion = "0";

		private readonly bool resumeToRecurringMeetings;
	}
}
