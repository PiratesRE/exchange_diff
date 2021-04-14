using System;
using System.Linq;

namespace Microsoft.Exchange.Diagnostics
{
	public class AverageSlidingSequence : SlidingSequence<long>
	{
		public AverageSlidingSequence(TimeSpan slidingWindowLength, TimeSpan bucketLength) : base(slidingWindowLength, bucketLength, () => DateTime.UtcNow)
		{
		}

		public long CalculateAverage()
		{
			long result;
			lock (this.syncObject)
			{
				double num = 0.0;
				foreach (object obj2 in this)
				{
					long num2 = (long)obj2;
					num += (double)num2;
				}
				result = ((this.Count<long>() != 0) ? ((long)Math.Round(num / (double)this.Count<long>())) : 0L);
			}
			return result;
		}

		private object syncObject = new object();
	}
}
