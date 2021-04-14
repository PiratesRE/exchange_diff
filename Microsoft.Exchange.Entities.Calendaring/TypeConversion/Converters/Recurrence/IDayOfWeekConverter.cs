using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters.Recurrence
{
	internal interface IDayOfWeekConverter : IConverter<DaysOfWeek, ISet<DayOfWeek>>, IConverter<ISet<DayOfWeek>, DaysOfWeek>
	{
	}
}
