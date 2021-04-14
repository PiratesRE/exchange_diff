using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RecurrenceRange
	{
		public virtual ExDateTime StartDate
		{
			get
			{
				return this.startDate;
			}
			protected set
			{
				ExTimeZoneHelperForMigrationOnly.CheckValidationLevel(value.HasTimeZone, ExTimeZoneHelperForMigrationOnly.ValidationLevel.Mid, "RecurrenceRange.StartDate_set: value has no time zone", new object[0]);
				this.startDate = value.Date;
			}
		}

		public virtual bool Equals(RecurrenceRange value)
		{
			return value != null && value.StartDate == this.startDate;
		}

		private ExDateTime startDate = ExDateTime.MinValue;
	}
}
