using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.EseRepl
{
	internal class Pool<T> : IPool<T> where T : class, IPoolableObject
	{
		public Pool(int expectedSize)
		{
			this.freeStack = new Stack<T>();
		}

		public virtual int FreeObjectCount
		{
			get
			{
				return this.freeStack.Count;
			}
		}

		public virtual bool TryReturnObject(T o)
		{
			bool result;
			lock (this.lockObj)
			{
				if (this.freeStack.Count == 0)
				{
					this.freeStack.Push(o);
				}
				else
				{
					if (!o.Preallocated)
					{
						return false;
					}
					IPoolableObject poolableObject = this.freeStack.Peek();
					if (!poolableObject.Preallocated)
					{
						T t = this.freeStack.Pop();
						IDisposable disposable = t as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
					this.freeStack.Push(o);
				}
				result = true;
			}
			return result;
		}

		public virtual T TryGetObject()
		{
			lock (this.lockObj)
			{
				if (this.freeStack.Count > 0)
				{
					return this.freeStack.Pop();
				}
			}
			return default(T);
		}

		private object lockObj = new object();

		private Stack<T> freeStack;
	}
}
