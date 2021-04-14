using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon.FaultInjection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UMReportingFaultInjection
	{
		internal static bool TryCreateException(string exceptionType, ref Exception exception)
		{
			return false;
		}

		public const uint UMReportingAssert = 3945147709U;
	}
}
