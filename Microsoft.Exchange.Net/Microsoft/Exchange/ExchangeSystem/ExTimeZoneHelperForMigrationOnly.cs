using System;

namespace Microsoft.Exchange.ExchangeSystem
{
	public static class ExTimeZoneHelperForMigrationOnly
	{
		public static object ToUtcIfDateTime(object value)
		{
			if (value is DateTime[])
			{
				DateTime[] array = value as DateTime[];
				ExDateTime[] array2 = new ExDateTime[array.Length];
				for (int num = 0; num != array.Length; num++)
				{
					array2[num] = ((ExDateTime)array[num]).ToUtc();
				}
				value = array2;
			}
			else if (value is ExDateTime[])
			{
				ExDateTime[] array3 = value as ExDateTime[];
				for (int num2 = 0; num2 != array3.Length; num2++)
				{
					array3[num2] = array3[num2].ToUtc();
				}
			}
			else if (value is ExDateTime)
			{
				value = ((ExDateTime)value).ToUtc();
			}
			else if (value is DateTime)
			{
				value = new ExDateTime(ExTimeZone.UtcTimeZone, (DateTime)value);
			}
			return value;
		}

		public static object ToExDateTimeIfObjectIsDateTime(object value)
		{
			return ExTimeZoneHelperForMigrationOnly.ToExDateTimeIfObjectIsDateTime(ExTimeZone.UnspecifiedTimeZone, value);
		}

		public static object ToExDateTimeIfObjectIsDateTime(ExTimeZone timeZone, object value)
		{
			if (value is DateTime)
			{
				return ExTimeZoneHelperForMigrationOnly.ToExDateTime(timeZone, (DateTime)value);
			}
			DateTime[] array = value as DateTime[];
			if (array != null)
			{
				return ExTimeZoneHelperForMigrationOnly.ToExDateTimeArray(timeZone, array);
			}
			return value;
		}

		public static ExDateTime[] ToExDateTimeArray(ExTimeZone timeZone, DateTime[] value)
		{
			ExDateTime[] array = new ExDateTime[value.Length];
			for (int i = 0; i < value.Length; i++)
			{
				array[i] = ExTimeZoneHelperForMigrationOnly.ToExDateTime(timeZone, value[i]);
			}
			return array;
		}

		private static ExDateTime ToExDateTime(ExTimeZone timeZone, DateTime value)
		{
			if (value < TimeLibConsts.MinSystemDateTimeValue)
			{
				value = TimeLibConsts.MinSystemDateTimeValue;
			}
			else if (value > TimeLibConsts.MaxSystemDateTimeValue)
			{
				value = TimeLibConsts.MaxSystemDateTimeValue;
			}
			return new ExDateTime(timeZone, value);
		}

		public static object ToLegacyUtcIfDateTime(object value)
		{
			return ExTimeZoneHelperForMigrationOnly.ToLegacyUtcIfDateTime(ExTimeZone.CurrentTimeZone, value);
		}

		public static object ToLegacyUtcIfDateTime(ExTimeZone timeZone, object value)
		{
			object result;
			if (value is ExDateTime)
			{
				result = (DateTime)((ExDateTime)value).ToUtc();
			}
			else if (value is ExDateTime[])
			{
				ExDateTime[] array = (ExDateTime[])value;
				DateTime[] array2 = new DateTime[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = (DateTime)array[i].ToUtc();
				}
				result = array2;
			}
			else if (value is DateTime)
			{
				ExTimeZoneHelperForMigrationOnly.CheckValidationLevel(false, ExTimeZoneHelperForMigrationOnly.ValidationLevel.Mid, "ExTimeZoneHelperForMigrationOnly.ToLegacyUtcIfDateTime: DateTime argument", new object[0]);
				ExDateTime exDateTime = new ExDateTime(timeZone, (DateTime)value);
				result = (DateTime)exDateTime.ToUtc();
			}
			else if (value is DateTime[])
			{
				ExTimeZoneHelperForMigrationOnly.CheckValidationLevel(false, ExTimeZoneHelperForMigrationOnly.ValidationLevel.Mid, "ExTimeZoneHelperForMigrationOnly.ToLegacyUtcIfDateTime: DateTime[] argument", new object[0]);
				DateTime[] array3 = (DateTime[])value;
				for (int j = 0; j < array3.Length; j++)
				{
					ExDateTime exDateTime2 = new ExDateTime(timeZone, array3[j]);
					array3[j] = (DateTime)exDateTime2.ToUtc();
				}
				result = array3;
			}
			else
			{
				result = value;
			}
			return result;
		}

		public static void CheckValidationLevel<Arg0>(bool condition, ExTimeZoneHelperForMigrationOnly.ValidationLevel level, string message, Arg0 arg0)
		{
			if (!condition && level >= ExTimeZoneHelperForMigrationOnly.CurrentValidationLevel)
			{
				throw new ExTimeLibException(string.Format(message, arg0));
			}
		}

		public static void CheckValidationLevel(bool condition, ExTimeZoneHelperForMigrationOnly.ValidationLevel level, string message, params object[] args)
		{
			if (!condition && level >= ExTimeZoneHelperForMigrationOnly.CurrentValidationLevel)
			{
				throw new ExTimeLibException(string.Format(message, args));
			}
		}

		private static ExTimeZoneHelperForMigrationOnly.ValidationLevel CurrentValidationLevel = ExTimeZoneHelperForMigrationOnly.ValidationLevel.Highest;

		public enum ValidationLevel
		{
			Lowest,
			VeryLow,
			Low,
			Mid,
			High,
			VeryHigh,
			Highest
		}
	}
}
