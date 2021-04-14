using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarItemExistsException : CalendarCopyMoveException
	{
		public CalendarItemExistsException(string message) : base(message)
		{
		}
	}
}
