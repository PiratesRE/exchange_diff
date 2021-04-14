using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum AppointmentStateFlags
	{
		None = 0,
		Meeting = 1,
		Received = 2,
		Cancelled = 4,
		Forward = 8
	}
}
