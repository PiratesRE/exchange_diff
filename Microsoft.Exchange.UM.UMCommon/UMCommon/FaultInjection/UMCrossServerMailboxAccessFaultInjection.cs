using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon.FaultInjection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UMCrossServerMailboxAccessFaultInjection
	{
		internal static bool TryCreateException(string exceptionType, ref Exception exception)
		{
			return false;
		}

		internal const uint UseEWSForLocalMailboxAccess = 3576048957U;
	}
}
