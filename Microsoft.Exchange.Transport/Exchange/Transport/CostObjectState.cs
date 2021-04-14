using System;

namespace Microsoft.Exchange.Transport
{
	internal enum CostObjectState
	{
		Init,
		Live,
		MarkedForDeletion,
		Deleted
	}
}
