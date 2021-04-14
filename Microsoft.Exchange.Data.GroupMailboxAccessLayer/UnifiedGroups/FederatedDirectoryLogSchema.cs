using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UnifiedGroups
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class FederatedDirectoryLogSchema
	{
		internal enum AssertTag
		{
			ActivityId,
			Message
		}

		internal enum ExceptionTag
		{
			TaskName,
			ActivityId,
			ExceptionType,
			ExceptionDetail,
			CurrentAction,
			Message
		}

		internal enum ShipAssertTag
		{
			ActivityId,
			Message
		}

		internal enum TraceTag
		{
			TaskName,
			ActivityId,
			CurrentAction,
			Message
		}
	}
}
