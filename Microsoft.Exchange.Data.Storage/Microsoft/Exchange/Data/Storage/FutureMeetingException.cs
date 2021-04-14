using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FutureMeetingException : CalendarCopyMoveException
	{
		public FutureMeetingException(string message) : base(message)
		{
		}
	}
}
