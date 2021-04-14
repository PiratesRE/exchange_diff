using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy
{
	internal class AsyncStateHolder : DisposeTrackableBase
	{
		public AsyncStateHolder(object owner)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			this.Owner = owner;
		}

		public object Owner { get; private set; }

		public static T Unwrap<T>(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			return (T)((object)((AsyncStateHolder)asyncResult.AsyncState).Owner);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AsyncStateHolder>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.Owner = null;
			}
		}
	}
}
