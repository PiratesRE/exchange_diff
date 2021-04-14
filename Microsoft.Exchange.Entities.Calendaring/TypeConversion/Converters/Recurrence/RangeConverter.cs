using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters.Recurrence
{
	internal class RangeConverter : IConverter<RecurrenceRange, RecurrenceRange>, IConverter<RecurrenceRange, RecurrenceRange>
	{
		public RecurrenceRange Convert(RecurrenceRange value)
		{
			if (value == null)
			{
				return null;
			}
			NoEndRecurrenceRange noEndRecurrenceRange = value as NoEndRecurrenceRange;
			if (noEndRecurrenceRange != null)
			{
				return new NoEndRecurrenceRange
				{
					StartDate = noEndRecurrenceRange.StartDate
				};
			}
			EndDateRecurrenceRange endDateRecurrenceRange = value as EndDateRecurrenceRange;
			if (endDateRecurrenceRange != null)
			{
				return new EndDateRecurrenceRange
				{
					StartDate = endDateRecurrenceRange.StartDate,
					EndDate = endDateRecurrenceRange.EndDate
				};
			}
			NumberedRecurrenceRange numberedRecurrenceRange = value as NumberedRecurrenceRange;
			if (numberedRecurrenceRange != null)
			{
				return new NumberedRecurrenceRange
				{
					StartDate = numberedRecurrenceRange.StartDate,
					NumberOfOccurrences = numberedRecurrenceRange.NumberOfOccurrences
				};
			}
			throw new ArgumentValueCannotBeParsedException("value", value.GetType().FullName, typeof(RecurrenceRange).FullName);
		}

		public RecurrenceRange Convert(RecurrenceRange value)
		{
			if (value == null)
			{
				return null;
			}
			switch (value.Type)
			{
			case RecurrenceRangeType.EndDate:
			{
				EndDateRecurrenceRange endDateRecurrenceRange = (EndDateRecurrenceRange)value;
				return new EndDateRecurrenceRange(endDateRecurrenceRange.StartDate, endDateRecurrenceRange.EndDate);
			}
			case RecurrenceRangeType.NoEnd:
			{
				NoEndRecurrenceRange noEndRecurrenceRange = (NoEndRecurrenceRange)value;
				return new NoEndRecurrenceRange(noEndRecurrenceRange.StartDate);
			}
			case RecurrenceRangeType.Numbered:
			{
				NumberedRecurrenceRange numberedRecurrenceRange = (NumberedRecurrenceRange)value;
				return new NumberedRecurrenceRange(numberedRecurrenceRange.StartDate, numberedRecurrenceRange.NumberOfOccurrences);
			}
			default:
				throw new ArgumentValueCannotBeParsedException("value", value.Type.ToString(), value.GetType().FullName);
			}
		}
	}
}
