using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters.Recurrence
{
	internal interface IWeekIndexConverter : IConverter<RecurrenceOrderType, WeekIndex>, IConverter<WeekIndex, RecurrenceOrderType>
	{
	}
}
