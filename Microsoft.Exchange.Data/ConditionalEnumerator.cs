using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	internal sealed class ConditionalEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposeTrackable, IDisposable
	{
		public ConditionalEnumerator(IEnumerable<T> conditionalEnumerable, IEnumerable<T> secondEnumerable)
		{
			this.conditionalEnumerable = conditionalEnumerable;
			this.secondEnumerable = secondEnumerable;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public T Current
		{
			get
			{
				if (this.mainEnum == null)
				{
					throw new InvalidOperationException("Cannot call Current without calling MoveNext first.");
				}
				return this.mainEnum.Current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				if (this.mainEnum == null)
				{
					throw new InvalidOperationException("Cannot call Current without calling MoveNext first.");
				}
				return this.mainEnum.Current;
			}
		}

		public bool MoveNext()
		{
			if (this.mainEnum == null)
			{
				EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(this.conditionalEnumerable);
				if (wrapper.HasElements())
				{
					this.mainEnum = wrapper.GetEnumerator();
					this.secondEnumerable = null;
				}
				else
				{
					this.mainEnum = this.secondEnumerable.GetEnumerator();
					this.conditionalEnumerable = null;
				}
			}
			return this.mainEnum.MoveNext();
		}

		public void Reset()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				if (this.mainEnum != null)
				{
					this.mainEnum.Dispose();
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
			this.disposed = true;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ConditionalEnumerator<T>>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private IEnumerable<T> conditionalEnumerable;

		private IEnumerable<T> secondEnumerable;

		private IEnumerator<T> mainEnum;

		private bool disposed;

		private DisposeTracker disposeTracker;
	}
}
