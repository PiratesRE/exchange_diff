using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Common
{
	public class DisposableFrame : DisposeTrackableBase
	{
		public DisposableFrame(Action onDispose)
		{
			this.onDispose = onDispose;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DisposableFrame>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.onDispose != null)
			{
				this.onDispose();
			}
		}

		private Action onDispose;
	}
}
