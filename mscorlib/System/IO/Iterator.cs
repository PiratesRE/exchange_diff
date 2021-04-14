using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace System.IO
{
	internal abstract class Iterator<TSource> : IEnumerable<!0>, IEnumerable, IEnumerator<TSource>, IDisposable, IEnumerator
	{
		public Iterator()
		{
			this.threadId = Thread.CurrentThread.ManagedThreadId;
		}

		public TSource Current
		{
			get
			{
				return this.current;
			}
		}

		protected abstract Iterator<TSource> Clone();

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			this.current = default(TSource);
			this.state = -1;
		}

		public IEnumerator<TSource> GetEnumerator()
		{
			if (this.threadId == Thread.CurrentThread.ManagedThreadId && this.state == 0)
			{
				this.state = 1;
				return this;
			}
			Iterator<TSource> iterator = this.Clone();
			iterator.state = 1;
			return iterator;
		}

		public abstract bool MoveNext();

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		private int threadId;

		internal int state;

		internal TSource current;
	}
}
