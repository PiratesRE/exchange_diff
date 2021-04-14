using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal enum ResourceId : byte
	{
		Primary,
		SoftDeletedPrimary,
		Secondary = 7,
		Calendar = 4,
		SoftDeletedCalendar = 8
	}
}
