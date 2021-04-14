using System;

namespace Microsoft.Exchange.ExchangeSystem
{
	internal class TimeLibConsts
	{
		internal static TimeSpan MaxBias = TimeSpan.FromHours(24.0);

		internal static DateTime MinSystemDateTimeValue = DateTime.SpecifyKind(DateTime.MinValue.Add(TimeLibConsts.MaxBias), DateTimeKind.Utc);

		internal static DateTime MaxSystemDateTimeValue = DateTime.SpecifyKind(DateTime.MaxValue.Subtract(TimeLibConsts.MaxBias), DateTimeKind.Utc);

		internal static TimeSpan MaxIncValue;
	}
}
