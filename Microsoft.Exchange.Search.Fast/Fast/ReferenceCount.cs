using System;
using System.Threading;

namespace Microsoft.Exchange.Search.Fast
{
	internal class ReferenceCount<T> where T : IDisposable
	{
		internal ReferenceCount(T referencedObject)
		{
			if (referencedObject == null)
			{
				throw new ArgumentNullException("referencedObject");
			}
			this.referencedObject = referencedObject;
			this.referenceCount = 1;
		}

		public T ReferencedObject
		{
			get
			{
				return this.referencedObject;
			}
		}

		internal int AddRef()
		{
			if (this.referenceCount <= 0)
			{
				throw new InvalidOperationException("AddRef");
			}
			return Interlocked.Increment(ref this.referenceCount);
		}

		internal int Release()
		{
			if (this.referenceCount <= 0)
			{
				throw new InvalidOperationException("Release");
			}
			int num = Interlocked.Decrement(ref this.referenceCount);
			if (num == 0)
			{
				this.referencedObject.Dispose();
			}
			return num;
		}

		private int referenceCount;

		private T referencedObject = default(T);
	}
}
