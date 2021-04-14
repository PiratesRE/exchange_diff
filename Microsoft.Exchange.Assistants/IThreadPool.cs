using System;
using System.Threading;

namespace Microsoft.Exchange.Assistants
{
	internal interface IThreadPool
	{
		void QueueUserWorkItem(WaitCallback callback, object state);
	}
}
