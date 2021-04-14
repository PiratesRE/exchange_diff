using System;
using System.Security;

namespace System.Threading
{
	internal static class ThreadPoolGlobals
	{
		public static uint tpQuantum = 30U;

		public static int processorCount = Environment.ProcessorCount;

		public static bool tpHosted = ThreadPool.IsThreadPoolHosted();

		public static volatile bool vmTpInitialized;

		public static bool enableWorkerTracking;

		[SecurityCritical]
		public static ThreadPoolWorkQueue workQueue = new ThreadPoolWorkQueue();
	}
}
