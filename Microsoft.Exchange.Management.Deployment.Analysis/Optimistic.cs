using System;
using System.Threading;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	internal sealed class Optimistic<T> where T : class
	{
		public Optimistic(T initialValue, Resolver<T> resolver)
		{
			this.value = initialValue;
			this.resolver = resolver;
		}

		public T UnsafeValue
		{
			get
			{
				return this.value;
			}
		}

		public T SafeValue
		{
			get
			{
				T result = this.value;
				Thread.MemoryBarrier();
				return result;
			}
		}

		public T Update(T originalValue, T updatedValue)
		{
			return this.Update(originalValue, updatedValue, this.resolver);
		}

		public T Update(T originalValue, T updatedValue, Resolver<T> resolver)
		{
			if (object.ReferenceEquals(originalValue, updatedValue))
			{
				return updatedValue;
			}
			SpinWait spinWait = default(SpinWait);
			T t = originalValue;
			for (;;)
			{
				t = Interlocked.CompareExchange<T>(ref this.value, updatedValue, t);
				if (object.ReferenceEquals(t, originalValue))
				{
					break;
				}
				updatedValue = resolver(originalValue, t, updatedValue);
				spinWait.SpinOnce();
			}
			return updatedValue;
		}

		private T value;

		private Resolver<T> resolver;
	}
}
