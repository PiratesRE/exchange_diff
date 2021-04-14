using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class DisposableWrapper<T> : DisposeTrackableBase where T : class, IDisposable
	{
		public DisposableWrapper(T wrappedObject, bool ownsObject)
		{
			this.wrappedObject = wrappedObject;
			this.ownsObject = ownsObject;
		}

		public T WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}

		public bool OwnsObject
		{
			get
			{
				return this.ownsObject;
			}
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.ownsObject && this.wrappedObject != null)
				{
					this.wrappedObject.Dispose();
				}
				this.wrappedObject = default(T);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DisposableWrapper<T>>(this);
		}

		private T wrappedObject;

		private bool ownsObject;
	}
}
