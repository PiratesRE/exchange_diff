using System;
using System.Threading;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class UnThrottledThreadPool : IThreadPool
	{
		public void QueueUserWorkItem(WaitCallback waitCallback, object state)
		{
			ThreadPool.QueueUserWorkItem(waitCallback, state);
		}
	}
}
