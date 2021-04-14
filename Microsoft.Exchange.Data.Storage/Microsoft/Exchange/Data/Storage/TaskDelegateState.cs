using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum TaskDelegateState
	{
		NoMatch,
		OwnNew,
		Owned,
		Accepted,
		Declined,
		Max
	}
}
