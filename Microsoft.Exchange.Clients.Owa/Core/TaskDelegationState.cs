using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	public enum TaskDelegationState
	{
		None = 0,
		Unknown = 1,
		Accepted = 2,
		Declined = 3,
		Max = 4
	}
}
