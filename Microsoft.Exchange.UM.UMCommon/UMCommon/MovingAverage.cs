using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class MovingAverage
	{
		internal MovingAverage(int size)
		{
			this.values = new Queue<long>(size);
			this.size = size;
		}

		internal long Value
		{
			get
			{
				long result;
				lock (this)
				{
					result = ((this.values.Count > 0) ? (this.sum / (long)this.values.Count) : 0L);
				}
				return result;
			}
		}

		internal long Update(long dataPoint)
		{
			long value;
			lock (this)
			{
				this.sum += dataPoint;
				this.values.Enqueue(dataPoint);
				while (this.values.Count > this.size)
				{
					this.sum -= this.values.Dequeue();
				}
				value = this.Value;
			}
			return value;
		}

		internal long Update(double dataPoint)
		{
			return this.Update((long)dataPoint);
		}

		private Queue<long> values;

		private long sum;

		private int size;
	}
}
