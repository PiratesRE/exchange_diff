using System;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal class Referenced<T> : IDisposable where T : IDisposable
	{
		private Referenced(T value)
		{
			this.value = value;
		}

		internal int Reference
		{
			get
			{
				return this.reference;
			}
			set
			{
				this.reference = value;
			}
		}

		internal T Value
		{
			get
			{
				return this.value;
			}
		}

		public static implicit operator T(Referenced<T> refObj)
		{
			return refObj.Value;
		}

		public void Dispose()
		{
			lock (this)
			{
				this.Release();
			}
		}

		internal static Referenced<T> Acquire(T value)
		{
			Referenced<T> referenced = new Referenced<T>(value);
			referenced.AddRef();
			return referenced;
		}

		internal Referenced<T> Reacquire()
		{
			lock (this)
			{
				if (this.Reference == 0)
				{
					return null;
				}
				this.AddRef();
			}
			return this;
		}

		private int AddRef()
		{
			return ++this.reference;
		}

		private int Release()
		{
			if (--this.reference == 0)
			{
				T t = this.Value;
				t.Dispose();
			}
			return this.reference;
		}

		private T value;

		private int reference;
	}
}
