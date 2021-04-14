using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Search
{
	internal class StopClass
	{
		internal void SetStop()
		{
			lock (this)
			{
				TestSearch.TestSearchTracer.TraceDebug((long)this.GetHashCode(), "SetStop is called");
				this.shouldStop = true;
			}
		}

		internal void CheckStop()
		{
			lock (this)
			{
				if (this.shouldStop)
				{
					throw new TestSearchOperationAbortedException();
				}
			}
		}

		private bool shouldStop;
	}
}
