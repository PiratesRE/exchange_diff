using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class BaseUMAsyncTimer : DisposableBase
	{
		internal BaseUMAsyncTimer(BaseUMCallSession session, BaseUMAsyncTimer.UMTimerCallback callback, int dueTime)
		{
			this.callback = callback;
			this.timer = new Timer(new TimerCallback(this.TimerExpired), session, dueTime * 1000, -1);
		}

		internal bool IsActive
		{
			get
			{
				return this.timer != null;
			}
		}

		internal BaseUMAsyncTimer.UMTimerCallback Callback
		{
			get
			{
				return this.callback;
			}
			set
			{
				this.callback = value;
			}
		}

		internal abstract void TimerExpired(object state);

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.timer.Dispose();
				this.timer = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<BaseUMAsyncTimer>(this);
		}

		private Timer timer;

		private BaseUMAsyncTimer.UMTimerCallback callback;

		internal delegate void UMTimerCallback(BaseUMCallSession session);
	}
}
