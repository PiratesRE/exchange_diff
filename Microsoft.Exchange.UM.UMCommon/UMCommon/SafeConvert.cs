using System;
using System.Globalization;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class SafeConvert
	{
		internal static int ToInt32(string value, int min, int max, int defaultValue)
		{
			if (max < min)
			{
				throw new ArgumentException("Min is less than Max");
			}
			int num = defaultValue;
			if (!string.IsNullOrEmpty(value) && (!int.TryParse(value, out num) || num < min || num > max))
			{
				num = defaultValue;
			}
			return num;
		}

		internal static TimeSpan ToTimeSpan(string value, TimeSpan min, TimeSpan max, TimeSpan defaultValue)
		{
			if (max < min)
			{
				throw new ArgumentException("Min is less than Max");
			}
			TimeSpan timeSpan = defaultValue;
			if (!string.IsNullOrEmpty(value) && (!TimeSpan.TryParse(value, out timeSpan) || timeSpan < min || timeSpan > max))
			{
				timeSpan = defaultValue;
			}
			return timeSpan;
		}

		internal static double ToDouble(string value, double min, double max, double defaultValue)
		{
			if (max < min)
			{
				throw new ArgumentException("Min is less than Max");
			}
			double num = defaultValue;
			if (!string.IsNullOrEmpty(value) && (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out num) || num < min || num > max))
			{
				num = defaultValue;
			}
			return num;
		}

		internal static bool ToBoolean(string value, bool defaultValue)
		{
			bool result = defaultValue;
			if (!string.IsNullOrEmpty(value) && !bool.TryParse(value, out result))
			{
				int num = 0;
				if (!int.TryParse(value, out num) || num > 1 || num < 0)
				{
					result = defaultValue;
				}
				else
				{
					result = (1 == num);
				}
			}
			return result;
		}

		internal static string ToString(string value, string defaultValue)
		{
			string result = defaultValue;
			if (!string.IsNullOrEmpty(value))
			{
				result = value;
			}
			return result;
		}

		internal static T ToEnum<T>(string value, T defaultValue) where T : struct
		{
			T result = defaultValue;
			if (!string.IsNullOrEmpty(value) && !EnumValidator<T>.TryParse(value, EnumParseOptions.IgnoreCase, out result))
			{
				result = defaultValue;
			}
			return result;
		}
	}
}
