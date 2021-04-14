using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	public enum TaskType
	{
		NoMatch = 0,
		Undelegated = 1,
		Delegated = 2,
		DelegatedAccepted = 3,
		DelegatedDeclined = 4,
		Max = 5
	}
}
