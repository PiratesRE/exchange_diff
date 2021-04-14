using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.Implementation)]
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
				this.CheckReleased();
				return this.referencedObject;
			}
		}

		internal static ReferenceCount<T> Assign(T referencedObject)
		{
			bool flag = false;
			ReferenceCount<T> result;
			try
			{
				ReferenceCount<T> referenceCount = new ReferenceCount<T>(referencedObject);
				flag = true;
				result = referenceCount;
			}
			finally
			{
				if (!flag)
				{
					referencedObject.Dispose();
				}
			}
			return result;
		}

		internal static void ReleaseIfPresent(ReferenceCount<T> referenceCountInstanceOrNull)
		{
			if (referenceCountInstanceOrNull != null)
			{
				referenceCountInstanceOrNull.Release();
			}
		}

		internal int AddRef()
		{
			this.CheckReleased();
			return Interlocked.Increment(ref this.referenceCount);
		}

		internal int Release()
		{
			this.CheckReleased();
			int num = Interlocked.Decrement(ref this.referenceCount);
			if (num == 0)
			{
				this.referencedObject.Dispose();
			}
			return num;
		}

		internal bool HasReleased
		{
			get
			{
				return this.referenceCount <= 0;
			}
		}

		private void CheckReleased()
		{
			if (this.referenceCount <= 0)
			{
				throw new ObjectDisposedException(base.GetType().ToString() + " has already been released.");
			}
		}

		private int referenceCount;

		private T referencedObject = default(T);
	}
}
