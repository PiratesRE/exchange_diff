using System;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal static class OnlineMeetingHelper
	{
		internal static string GetSipDomain(string sipAddress)
		{
			if (string.IsNullOrWhiteSpace(sipAddress))
			{
				return string.Empty;
			}
			int num = sipAddress.LastIndexOf("@");
			if (num <= 0)
			{
				return string.Empty;
			}
			return sipAddress.Substring(num + 1);
		}
	}
}
