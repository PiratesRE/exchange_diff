using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public abstract class Task : DisposableBase
	{
		public static IDisposable SetInvokeTestHook(Action action)
		{
			return Task.invokeTestHook.SetTestHook(action);
		}

		public static void TestDisableTaskExecution()
		{
			Task.testDisabledInvoke = true;
			while (Task.invokeCount != 0)
			{
				Thread.Yield();
			}
		}

		public static void TestReenableTaskExecution()
		{
			Task.testDisabledInvoke = false;
		}

		public abstract void Start();

		public abstract void Stop();

		public abstract bool Finished();

		public abstract bool WaitForCompletion(TimeSpan delay);

		public bool WaitForCompletion()
		{
			return this.WaitForCompletion(Task.InfiniteDelay);
		}

		protected static readonly TimeSpan NoDelay = TimeSpan.Zero;

		protected static readonly TimeSpan InfiniteDelay = TimeSpan.FromMilliseconds(-1.0);

		protected static int invokeCount;

		protected static bool testDisabledInvoke;

		protected static Hookable<Action> invokeTestHook = Hookable<Action>.Create(true, null);

		protected internal Action StartLockEnterTestHook;

		protected internal Action StartLockEnteredTestHook;

		protected internal Action StopLockEnterTestHook;

		protected internal Action StopLockEnteredTestHook;

		protected internal Action InvokeLock1EnterTestHook;

		protected internal Action InvokeLock1EnteredTestHook;

		protected internal Action InvokeLock2EnterTestHook;

		protected internal Action InvokeLock2EnteredTestHook;

		protected internal Action DisposeLock1EnterTestHook;

		protected internal Action DisposeLock1EnteredTestHook;

		protected internal Action DisposeLock2EnterTestHook;

		protected internal Action DisposeLock2EnteredTestHook;
	}
}
