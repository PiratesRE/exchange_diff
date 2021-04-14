using System;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaAsyncTimer : BaseUMAsyncTimer
	{
		public UcmaAsyncTimer(BaseUMCallSession session, BaseUMAsyncTimer.UMTimerCallback callback, int dueTime) : base(session, callback, dueTime)
		{
		}

		internal override void TimerExpired(object state)
		{
			UcmaCallSession session = (UcmaCallSession)state;
			if (base.IsActive)
			{
				base.Callback(session);
			}
		}
	}
}
