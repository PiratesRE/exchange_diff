using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DownCastEnumerator<TFrom, TTo> : IEnumerator<TTo>, IDisposable, IEnumerator where TTo : TFrom
	{
		internal DownCastEnumerator(IEnumerator<TFrom> fromEnumerator)
		{
			this.fromEnumerator = fromEnumerator;
		}

		public TTo Current
		{
			get
			{
				this.CheckDisposed("Current<T>::get");
				return (TTo)((object)this.fromEnumerator.Current);
			}
		}

		object IEnumerator.Current
		{
			get
			{
				this.CheckDisposed("Current<T>::get");
				return this.Current;
			}
		}

		public void Reset()
		{
			this.CheckDisposed("Current<T>::get");
			this.fromEnumerator.Reset();
		}

		public bool MoveNext()
		{
			this.CheckDisposed("Current<T>::get");
			return this.fromEnumerator.MoveNext();
		}

		private void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		private void InternalDispose(bool disposing)
		{
			if (disposing && this.fromEnumerator != null)
			{
				this.fromEnumerator.Dispose();
			}
		}

		private IEnumerator<TFrom> fromEnumerator;

		private bool isDisposed;
	}
}
