using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class Average
	{
		internal Average()
		{
		}

		internal long Update(long dataPoint)
		{
			long result;
			lock (this)
			{
				this.sum += dataPoint;
				result = this.sum / (this.count += 1L);
			}
			return result;
		}

		private long sum;

		private long count;
	}
}
