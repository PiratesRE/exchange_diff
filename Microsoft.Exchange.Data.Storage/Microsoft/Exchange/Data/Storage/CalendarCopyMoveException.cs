using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarCopyMoveException : Exception
	{
		public CalendarCopyMoveException(string message) : base(message)
		{
		}

		public CalendarCopyMoveException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
