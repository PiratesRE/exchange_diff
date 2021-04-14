using System;

namespace Microsoft.Exchange.Connections.Common
{
	public enum SyncPhase
	{
		Initial,
		Incremental,
		Finalization,
		Completed,
		Delete
	}
}
