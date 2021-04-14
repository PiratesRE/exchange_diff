using System;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class ElcScheduleIntervalsConstraint : PropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (value == null)
			{
				return null;
			}
			ScheduleInterval[] array = value as ScheduleInterval[];
			if (array == null)
			{
				return new PropertyConstraintViolationError(DataStrings.ElcScheduleInvalidType(value.GetType().ToString()), propertyDefinition, value, this);
			}
			int num = array.Length;
			foreach (ScheduleInterval scheduleInterval in array)
			{
				if (scheduleInterval.StartDay == scheduleInterval.EndDay && scheduleInterval.StartHour == scheduleInterval.EndHour && scheduleInterval.StartMinute == scheduleInterval.EndMinute)
				{
					return new PropertyConstraintViolationError(DataStrings.ElcScheduleInvalidIntervals(scheduleInterval.ToString()), propertyDefinition, value, this);
				}
			}
			if (num == 1)
			{
				return null;
			}
			if (num == 0)
			{
				return null;
			}
			Array.Sort<ScheduleInterval>(array);
			int num2 = (num == 2) ? 1 : 2;
			for (int j = 0; j < num2; j++)
			{
				if (array[j].Overlaps(array[(j + 1) % num]) || array[j].ConjointWith(array[(j + 1) % num]))
				{
					return new PropertyConstraintViolationError(DataStrings.ElcScheduleInsufficientGap, propertyDefinition, value, this);
				}
			}
			return null;
		}
	}
}
