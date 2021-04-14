using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ClientIntentExtensions
	{
		internal static bool Includes(this ClientIntentFlags flags, ClientIntentFlags desiredFlags)
		{
			return (flags & desiredFlags) == desiredFlags;
		}
	}
}
