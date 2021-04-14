using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Common
{
	public class SecureArray<T> : DisposeTrackableBase
	{
		public SecureArray(T[] array)
		{
			this.array = array;
			this.gcHandle = GCHandle.Alloc(this.array, GCHandleType.Pinned);
		}

		public SecureArray(int arraySize) : this(new T[arraySize])
		{
		}

		~SecureArray()
		{
			base.Dispose(false);
		}

		public T[] ArrayValue
		{
			get
			{
				return this.array;
			}
		}

		public int Length()
		{
			return this.array.Length;
		}

		protected override void InternalDispose(bool isDisposing)
		{
			Array.Clear(this.array, 0, this.array.Length);
			this.gcHandle.Free();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SecureArray<T>>(this);
		}

		private readonly T[] array;

		private GCHandle gcHandle;
	}
}
