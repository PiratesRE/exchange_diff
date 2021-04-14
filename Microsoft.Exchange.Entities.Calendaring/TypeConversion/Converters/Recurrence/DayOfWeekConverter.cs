using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters.Recurrence
{
	internal struct DayOfWeekConverter : IDayOfWeekConverter, IConverter<DaysOfWeek, ISet<DayOfWeek>>, IConverter<ISet<DayOfWeek>, DaysOfWeek>
	{
		public DaysOfWeek Convert(ISet<DayOfWeek> value)
		{
			if (value == null)
			{
				throw new ExArgumentNullException("value");
			}
			DaysOfWeek result = DaysOfWeek.None;
			foreach (Tuple<DaysOfWeek, DayOfWeek> tuple in DayOfWeekConverter.MappingTuples)
			{
				DayOfWeekConverter.AddDayToSetIfPresent(value, tuple.Item2, tuple.Item1, ref result);
			}
			return result;
		}

		public ISet<DayOfWeek> Convert(DaysOfWeek value)
		{
			HashSet<DayOfWeek> result = new HashSet<DayOfWeek>();
			foreach (Tuple<DaysOfWeek, DayOfWeek> tuple in DayOfWeekConverter.MappingTuples)
			{
				DayOfWeekConverter.AddDayToSetIfPresent(value, tuple.Item1, tuple.Item2, result);
			}
			return result;
		}

		private static void AddDayToSetIfPresent(DaysOfWeek daysOfWeek, DaysOfWeek valueToSearch, DayOfWeek dayToAdd, ISet<DayOfWeek> result)
		{
			if ((daysOfWeek & valueToSearch) == valueToSearch)
			{
				result.Add(dayToAdd);
			}
		}

		private static void AddDayToSetIfPresent(ISet<DayOfWeek> daysOfWeek, DayOfWeek valueToSearch, DaysOfWeek dayToAdd, ref DaysOfWeek result)
		{
			if (daysOfWeek.Contains(valueToSearch))
			{
				result |= dayToAdd;
			}
		}

		private static readonly Tuple<DaysOfWeek, DayOfWeek>[] MappingTuples = new Tuple<DaysOfWeek, DayOfWeek>[]
		{
			new Tuple<DaysOfWeek, DayOfWeek>(DaysOfWeek.Monday, DayOfWeek.Monday),
			new Tuple<DaysOfWeek, DayOfWeek>(DaysOfWeek.Tuesday, DayOfWeek.Tuesday),
			new Tuple<DaysOfWeek, DayOfWeek>(DaysOfWeek.Wednesday, DayOfWeek.Wednesday),
			new Tuple<DaysOfWeek, DayOfWeek>(DaysOfWeek.Thursday, DayOfWeek.Thursday),
			new Tuple<DaysOfWeek, DayOfWeek>(DaysOfWeek.Friday, DayOfWeek.Friday),
			new Tuple<DaysOfWeek, DayOfWeek>(DaysOfWeek.Saturday, DayOfWeek.Saturday),
			new Tuple<DaysOfWeek, DayOfWeek>(DaysOfWeek.Sunday, DayOfWeek.Sunday)
		};
	}
}
