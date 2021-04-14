using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EndDateRecurrenceRange : RecurrenceRange
	{
		public ExDateTime EndDate
		{
			get
			{
				return this.endDate;
			}
			private set
			{
				ExTimeZoneHelperForMigrationOnly.CheckValidationLevel(value.HasTimeZone, ExTimeZoneHelperForMigrationOnly.ValidationLevel.Mid, "EndDateRecurrenceRange.EndDate_set: value has no time zone", new object[0]);
				if (value < this.StartDate)
				{
					throw new ArgumentException(ServerStrings.ExEndDateEarlierThanStartDate);
				}
				this.endDate = value.Date;
			}
		}

		public override ExDateTime StartDate
		{
			get
			{
				return base.StartDate;
			}
			protected set
			{
				if (value > this.EndDate)
				{
					throw new ArgumentException(ServerStrings.ExStartDateLaterThanEndDate);
				}
				base.StartDate = value;
			}
		}

		public EndDateRecurrenceRange(ExDateTime startDate, ExDateTime endDate)
		{
			ExTimeZoneHelperForMigrationOnly.CheckValidationLevel(startDate.TimeZone == endDate.TimeZone, ExTimeZoneHelperForMigrationOnly.ValidationLevel.Mid, "EndDateRecurrenceRange constructor.\nstartDate.TimeZone={0}\nendDateTime.TimeZone={1}", new object[]
			{
				startDate.TimeZone,
				endDate.TimeZone
			});
			this.StartDate = startDate;
			this.EndDate = endDate;
		}

		public override bool Equals(RecurrenceRange value)
		{
			if (!(value is EndDateRecurrenceRange))
			{
				return false;
			}
			EndDateRecurrenceRange endDateRecurrenceRange = (EndDateRecurrenceRange)value;
			return endDateRecurrenceRange.EndDate == this.endDate && base.Equals(value);
		}

		public override string ToString()
		{
			return string.Format("Starts {0}, ends {1}", this.StartDate.ToString(DateTimeFormatInfo.InvariantInfo), this.EndDate.ToString(DateTimeFormatInfo.InvariantInfo));
		}

		private ExDateTime endDate = ExDateTime.MaxValue;
	}
}
