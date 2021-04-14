using System;
using System.Threading;

namespace Microsoft.Exchange.Server.Storage.Common
{
	internal sealed class ConcurrentLookAside<T> where T : class
	{
		private ConcurrentLookAside()
		{
			this.lookAsideList = new T[64];
		}

		public T Get()
		{
			int currentManagedThreadId = Environment.CurrentManagedThreadId;
			for (int i = 0; i < this.lookAsideList.Length; i++)
			{
				int num = (i + currentManagedThreadId) % this.lookAsideList.Length;
				T t = this.lookAsideList[num];
				if (t != null && object.ReferenceEquals(t, Interlocked.CompareExchange<T>(ref this.lookAsideList[num], default(T), t)))
				{
					return t;
				}
			}
			return default(T);
		}

		public bool Put(T unusedObject)
		{
			if (unusedObject != null)
			{
				int currentManagedThreadId = Environment.CurrentManagedThreadId;
				for (int i = 0; i < this.lookAsideList.Length; i++)
				{
					int num = (i + currentManagedThreadId) % this.lookAsideList.Length;
					if (this.lookAsideList[num] == null && Interlocked.CompareExchange<T>(ref this.lookAsideList[num], unusedObject, default(T)) == null)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal const int DefaultLookAsideSize = 64;

		private readonly T[] lookAsideList;

		internal static readonly ConcurrentLookAside<T> Pool = new ConcurrentLookAside<T>();
	}
}
