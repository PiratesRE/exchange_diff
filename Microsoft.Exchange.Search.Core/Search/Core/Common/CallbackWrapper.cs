using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class CallbackWrapper
	{
		protected CallbackWrapper()
		{
		}

		internal static Hookable<CallbackWrapper> HookableCallbackWrapper
		{
			get
			{
				return CallbackWrapper.hookableCallbackWrapper;
			}
		}

		public static WaitCallback WaitCallback(WaitCallback callback)
		{
			return CallbackWrapper.HookableCallbackWrapper.Value.WrapWaitCallback(callback);
		}

		public static WaitOrTimerCallback WaitOrTimerCallback(WaitOrTimerCallback callback)
		{
			return CallbackWrapper.HookableCallbackWrapper.Value.WrapWaitOrTimerCallback(callback);
		}

		protected virtual WaitCallback WrapWaitCallback(WaitCallback callback)
		{
			return callback;
		}

		protected virtual WaitOrTimerCallback WrapWaitOrTimerCallback(WaitOrTimerCallback callback)
		{
			return callback;
		}

		private static readonly Hookable<CallbackWrapper> hookableCallbackWrapper = Hookable<CallbackWrapper>.Create(true, new CallbackWrapper());
	}
}
