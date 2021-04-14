using System;

namespace Microsoft.Exchange.Management.SystemManager
{
	internal abstract class FilterNodeSynchronizer
	{
		public abstract void Synchronize(FilterNode sourceNode, FilterNode targetNode);
	}
}
