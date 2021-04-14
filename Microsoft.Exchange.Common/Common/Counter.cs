using System;
using System.Threading;

namespace Microsoft.Exchange.Common
{
	internal sealed class Counter
	{
		internal Counter()
		{
		}

		internal Counter(int value)
		{
			this.counter = value;
		}

		internal int Value
		{
			get
			{
				return this.counter;
			}
			set
			{
				Interlocked.Exchange(ref this.counter, value);
			}
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}

		internal void Increment()
		{
			Interlocked.Increment(ref this.counter);
		}

		internal void Decrement()
		{
			Interlocked.Decrement(ref this.counter);
		}

		internal void IncrementBy(int value)
		{
			Interlocked.Add(ref this.counter, value);
		}

		internal void DecrementBy(int value)
		{
			Interlocked.Add(ref this.counter, -value);
		}

		private int counter;
	}
}
