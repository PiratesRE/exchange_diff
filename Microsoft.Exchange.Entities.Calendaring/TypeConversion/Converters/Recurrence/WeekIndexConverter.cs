using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters.Recurrence
{
	internal struct WeekIndexConverter : IWeekIndexConverter, IConverter<RecurrenceOrderType, WeekIndex>, IConverter<WeekIndex, RecurrenceOrderType>
	{
		public WeekIndex Convert(RecurrenceOrderType value)
		{
			return WeekIndexConverter.mappingConverter.Convert(value);
		}

		public RecurrenceOrderType Convert(WeekIndex value)
		{
			return WeekIndexConverter.mappingConverter.Reverse(value);
		}

		private static SimpleMappingConverter<RecurrenceOrderType, WeekIndex> mappingConverter = SimpleMappingConverter<RecurrenceOrderType, WeekIndex>.CreateStrictConverter(new Tuple<RecurrenceOrderType, WeekIndex>[]
		{
			new Tuple<RecurrenceOrderType, WeekIndex>(RecurrenceOrderType.Last, WeekIndex.Last),
			new Tuple<RecurrenceOrderType, WeekIndex>(RecurrenceOrderType.First, WeekIndex.First),
			new Tuple<RecurrenceOrderType, WeekIndex>(RecurrenceOrderType.Second, WeekIndex.Second),
			new Tuple<RecurrenceOrderType, WeekIndex>(RecurrenceOrderType.Third, WeekIndex.Third),
			new Tuple<RecurrenceOrderType, WeekIndex>(RecurrenceOrderType.Fourth, WeekIndex.Fourth)
		});
	}
}
