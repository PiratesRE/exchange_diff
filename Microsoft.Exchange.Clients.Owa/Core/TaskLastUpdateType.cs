using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	public enum TaskLastUpdateType
	{
		None = 0,
		Accepted = 1,
		Declined = 2,
		Updated = 3,
		DueDateChanged = 4,
		Assigned = 5,
		Max = 6
	}
}
