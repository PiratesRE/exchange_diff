using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class EventLogExtensionMethods
	{
		internal static string TruncateToUseInEventLog(this string originalString)
		{
			if (originalString.Length > 30720)
			{
				return originalString.Substring(0, 30720);
			}
			return originalString;
		}

		private const int MaxEventLogStringSize = 30720;
	}
}
