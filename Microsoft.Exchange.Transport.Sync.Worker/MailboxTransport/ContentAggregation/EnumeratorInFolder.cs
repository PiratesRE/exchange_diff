using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class EnumeratorInFolder<TValue, UValue> : DisposeTrackableBase, IEnumerator<TValue>, IDisposable, IEnumerator
	{
		internal EnumeratorInFolder(IEnumerator<UValue> underlyingEnumerator, object filter)
		{
			if (underlyingEnumerator == null)
			{
				throw new ArgumentNullException("underlyingEnumerator");
			}
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			this.filterHash = filter.GetHashCode();
			this.underlyingEnumerator = underlyingEnumerator;
		}

		public TValue Current
		{
			get
			{
				return this.current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.current;
			}
		}

		public bool MoveNext()
		{
			while (this.underlyingEnumerator.MoveNext())
			{
				UValue item = this.underlyingEnumerator.Current;
				if (!this.SkipCurrent(item))
				{
					TValue currentFolder = this.GetCurrentFolder(item);
					if (this.filterHash == currentFolder.GetHashCode())
					{
						this.current = this.GetCurrent(item);
						return true;
					}
				}
			}
			return false;
		}

		public void Reset()
		{
			this.underlyingEnumerator.Reset();
			this.current = default(TValue);
		}

		protected abstract bool SkipCurrent(UValue item);

		protected abstract TValue GetCurrent(UValue item);

		protected abstract TValue GetCurrentFolder(UValue item);

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.underlyingEnumerator != null)
				{
					this.underlyingEnumerator.Dispose();
					this.underlyingEnumerator = null;
				}
				this.current = default(TValue);
			}
		}

		private readonly int filterHash;

		private IEnumerator<UValue> underlyingEnumerator;

		private TValue current;
	}
}
