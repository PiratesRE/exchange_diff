using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PrimaryCalendarFolderException : CalendarCopyMoveException
	{
		public PrimaryCalendarFolderException(string message) : base(message)
		{
		}
	}
}
