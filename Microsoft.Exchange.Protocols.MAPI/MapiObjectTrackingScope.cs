using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	[Flags]
	public enum MapiObjectTrackingScope : uint
	{
		Session = 1U,
		Service = 2U,
		User = 4U,
		All = 7U
	}
}
