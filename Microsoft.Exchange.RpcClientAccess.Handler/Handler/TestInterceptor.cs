using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class TestInterceptor
	{
		public static void DisableConditions()
		{
			TestInterceptor.location = TestInterceptorLocation.None;
			TestInterceptor.exceptionToThrow = null;
			TestInterceptor.countCondition = null;
			TestInterceptor.callback = null;
		}

		public static void EnableThrowOnCondition(TestInterceptorLocation location, Exception exceptionToThrow)
		{
			Util.ThrowOnNullArgument(exceptionToThrow, "exceptionToThrow");
			TestInterceptor.location = location;
			TestInterceptor.exceptionToThrow = exceptionToThrow;
		}

		public static void CallbackOnCondition(TestInterceptorLocation location, Action<object[]> callback)
		{
			Util.ThrowOnNullArgument(callback, "callback");
			TestInterceptor.location = location;
			TestInterceptor.callback = callback;
		}

		public static void OverrideOnCondition<T>(TestInterceptorLocation location, T newValue) where T : class
		{
			Util.ThrowOnNullArgument(newValue, "newValue");
			TestInterceptor.location = location;
			TestInterceptor.newValue = newValue;
		}

		public static void WaitForCountCondition(TestInterceptorLocation location, Semaphore countCondition)
		{
			TestInterceptor.location = location;
			TestInterceptor.countCondition = countCondition;
		}

		public static void Intercept(TestInterceptorLocation location, params object[] states)
		{
			if ((TestInterceptor.location & location) != location)
			{
				return;
			}
			if (TestInterceptor.exceptionToThrow != null)
			{
				throw TestInterceptor.exceptionToThrow;
			}
			if (TestInterceptor.countCondition != null)
			{
				TestInterceptor.countCondition.WaitOne();
				return;
			}
			if (TestInterceptor.callback != null)
			{
				TestInterceptor.callback(states);
				return;
			}
			throw new InvalidOperationException();
		}

		public static void InterceptValue<T>(TestInterceptorLocation location, ref T overrideValue)
		{
			if ((TestInterceptor.location & location) == location && TestInterceptor.newValue != null)
			{
				overrideValue = (T)((object)TestInterceptor.newValue);
			}
		}

		public static Semaphore CountCondition
		{
			get
			{
				return TestInterceptor.countCondition;
			}
		}

		private static Exception exceptionToThrow = null;

		private static Action<object[]> callback = null;

		private static TestInterceptorLocation location;

		private static object newValue = null;

		private static Semaphore countCondition = null;
	}
}
