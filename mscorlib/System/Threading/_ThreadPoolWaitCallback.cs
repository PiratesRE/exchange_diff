using System;
using System.Security;

namespace System.Threading
{
	internal static class _ThreadPoolWaitCallback
	{
		[SecurityCritical]
		internal static bool PerformWaitCallback()
		{
			return ThreadPoolWorkQueue.Dispatch();
		}
	}
}
