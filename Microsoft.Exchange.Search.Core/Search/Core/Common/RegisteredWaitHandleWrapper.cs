using System;
using System.Threading;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class RegisteredWaitHandleWrapper
	{
		public static void RegisterWaitForSingleObject(WaitHandle waitObject, WaitOrTimerCallback callback, object state, uint millisecondsTimeOutInterval, bool executeOnlyOnce)
		{
			RegisteredWaitHandleWrapper.RegisterWaitForSingleObject(waitObject, callback, state, TimeSpan.FromMilliseconds(millisecondsTimeOutInterval), executeOnlyOnce);
		}

		public static void RegisterWaitForSingleObject(WaitHandle waitObject, WaitOrTimerCallback callback, object state, TimeSpan timeout, bool executeOnlyOnce)
		{
			RegisteredWaitHandleWrapper registeredWaitHandleWrapper = new RegisteredWaitHandleWrapper();
			registeredWaitHandleWrapper.userCallback = callback;
			registeredWaitHandleWrapper.userExecuteOnlyOnce = executeOnlyOnce;
			lock (registeredWaitHandleWrapper.lockObject)
			{
				registeredWaitHandleWrapper.handle = ThreadPool.RegisterWaitForSingleObject(waitObject, CallbackWrapper.WaitOrTimerCallback(new WaitOrTimerCallback(registeredWaitHandleWrapper.UnregisterHandle)), state, timeout, executeOnlyOnce);
			}
		}

		private void UnregisterHandle(object state, bool timedOut)
		{
			if (this.userExecuteOnlyOnce || !timedOut)
			{
				lock (this.lockObject)
				{
					this.handle.Unregister(null);
				}
			}
			this.userCallback(state, timedOut);
		}

		private WaitOrTimerCallback userCallback;

		private RegisteredWaitHandle handle;

		private bool userExecuteOnlyOnce;

		private object lockObject = new object();
	}
}
