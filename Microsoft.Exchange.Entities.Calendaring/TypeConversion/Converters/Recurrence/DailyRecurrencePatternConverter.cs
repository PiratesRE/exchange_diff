using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters.Recurrence
{
	internal struct DailyRecurrencePatternConverter
	{
		public DailyRecurrencePattern ConvertEntitiesToStorage(DailyRecurrencePattern value)
		{
			if (value != null)
			{
				return new DailyRecurrencePattern(value.Interval);
			}
			return null;
		}

		public RecurrencePattern ConvertStorageToEntities(DailyRecurrencePattern value)
		{
			if (value != null)
			{
				return new DailyRecurrencePattern
				{
					Interval = value.RecurrenceInterval
				};
			}
			return null;
		}
	}
}
