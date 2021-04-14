using System;
using System.Threading;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Server.Storage.Common
{
	internal class CompletionNotificationTask<T> : Task<T>
	{
		public CompletionNotificationTask(CompletionNotificationTask<T>.CompletionNotificationCallback callback, T context, IoCompletionPort completionPort, uint idleTimeout, bool autoStart) : base(null, context, ThreadPriority.Normal, 0, autoStart ? TaskFlags.AutoStart : TaskFlags.None)
		{
			base.CallbackDelegate = new Task<T>.TaskCallback(this.NotificationListener);
			this.notificationCallback = callback;
			this.completionPort = completionPort;
			this.idleTimeout = idleTimeout;
		}

		public override void Stop()
		{
			base.CheckDisposed();
			base.Stop();
			this.completionPort.PostQueuedCompletionStatus(uint.MaxValue, uint.MaxValue, IntPtr.Zero);
		}

		private void NotificationListener(TaskExecutionDiagnosticsProxy diagnosticsContext, T context, Func<bool> shouldCallbackContinue)
		{
			ThreadManager.MarkCurrentThreadAsLongRunning();
			while (shouldCallbackContinue())
			{
				uint num = 0U;
				UIntPtr uintPtr = new UIntPtr(0U);
				int data = 0;
				bool queuedCompletionStatus = this.completionPort.GetQueuedCompletionStatus(out num, out uintPtr, out data, this.idleTimeout);
				if (queuedCompletionStatus)
				{
					this.notificationCallback(context, shouldCallbackContinue, num, uintPtr.ToUInt32(), data);
				}
				else
				{
					this.notificationCallback(context, shouldCallbackContinue, 4294967294U, uint.MaxValue, 0);
				}
				if (uintPtr.ToUInt32() == 4294967295U && num == 4294967294U)
				{
					return;
				}
			}
		}

		public const uint TaskCompletionKey = 4294967295U;

		private IoCompletionPort completionPort;

		private uint idleTimeout;

		private CompletionNotificationTask<T>.CompletionNotificationCallback notificationCallback;

		public enum TaksNotifications : uint
		{
			Timeout = 4294967294U,
			Exit
		}

		public delegate void CompletionNotificationCallback(T context, Func<bool> shouldCallbackContinue, uint notification, uint completionKey, int data);
	}
}
