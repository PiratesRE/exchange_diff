using System;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	[Flags]
	internal enum FreeBusyStatusEnum : uint
	{
		None = 0U,
		Free = 1U,
		Tentative = 2U,
		Busy = 4U,
		OutOfOffice = 8U,
		NotAvailable = 268435456U
	}
}
