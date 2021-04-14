using System;

namespace Microsoft.Exchange.Common.HA
{
	internal enum FailureNameSpace : uint
	{
		None,
		Store,
		Ese,
		ContentIndex,
		Replay,
		DagManagement,
		MA
	}
}
