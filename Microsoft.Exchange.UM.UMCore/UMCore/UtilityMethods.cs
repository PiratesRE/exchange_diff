using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCore
{
	internal static class UtilityMethods
	{
		internal static double GetTimeSpanFrom(ExDateTime time)
		{
			double result = 0.0;
			ExDateTime now = ExDateTime.GetNow(time.TimeZone);
			if (now > time)
			{
				result = (now - time).TotalSeconds;
			}
			return result;
		}

		internal static bool IsAnonymousNumber(string number)
		{
			return string.IsNullOrEmpty(number) || string.Equals(number, "Anonymous", StringComparison.OrdinalIgnoreCase);
		}

		internal static bool IsAnonymousAddress(PlatformTelephonyAddress address)
		{
			return (!string.IsNullOrEmpty(address.Name) && string.Equals(address.Name, "Anonymous", StringComparison.OrdinalIgnoreCase)) || UtilityMethods.IsAnonymousNumber(address.Uri.User);
		}
	}
}
