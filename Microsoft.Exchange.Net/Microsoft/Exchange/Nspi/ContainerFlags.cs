using System;

namespace Microsoft.Exchange.Nspi
{
	[Flags]
	internal enum ContainerFlags
	{
		None = 0,
		Recipients = 1,
		Subcontainers = 2,
		Unmodifiable = 8,
		ConfRooms = 512
	}
}
