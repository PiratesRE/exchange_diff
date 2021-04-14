using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data
{
	public class CurrentOperationCounter : ICurrentOperationCounter, IDisposable
	{
		public CurrentOperationCounter(ExPerformanceCounter counter, bool autoIncrement = true)
		{
			if (counter == null)
			{
				throw new ArgumentNullException("counter");
			}
			this.counter = counter;
			if (autoIncrement)
			{
				this.Increment();
			}
		}

		public void Increment()
		{
			this.counter.Increment();
		}

		public void Decrement()
		{
			this.counter.Decrement();
		}

		void IDisposable.Dispose()
		{
			this.Decrement();
		}

		private ExPerformanceCounter counter;
	}
}
