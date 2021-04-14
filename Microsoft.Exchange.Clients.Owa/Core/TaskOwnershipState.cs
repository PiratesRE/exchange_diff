using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	public enum TaskOwnershipState
	{
		New = 0,
		Delegate = 1,
		Me = 2,
		Max = 3
	}
}
