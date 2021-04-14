using System;
using System.Diagnostics.Tracing;
using System.Security;
using Microsoft.Win32;

namespace System.Threading
{
	internal sealed class TimerQueueTimer
	{
		[SecurityCritical]
		internal TimerQueueTimer(TimerCallback timerCallback, object state, uint dueTime, uint period, ref StackCrawlMark stackMark)
		{
			this.m_timerCallback = timerCallback;
			this.m_state = state;
			this.m_dueTime = uint.MaxValue;
			this.m_period = uint.MaxValue;
			if (!ExecutionContext.IsFlowSuppressed())
			{
				this.m_executionContext = ExecutionContext.Capture(ref stackMark, ExecutionContext.CaptureOptions.IgnoreSyncCtx | ExecutionContext.CaptureOptions.OptimizeDefaultCase);
			}
			if (dueTime != 4294967295U)
			{
				this.Change(dueTime, period);
			}
		}

		internal bool Change(uint dueTime, uint period)
		{
			TimerQueue instance = TimerQueue.Instance;
			bool result;
			lock (instance)
			{
				if (this.m_canceled)
				{
					throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_Generic"));
				}
				try
				{
				}
				finally
				{
					this.m_period = period;
					if (dueTime == 4294967295U)
					{
						TimerQueue.Instance.DeleteTimer(this);
						result = true;
					}
					else
					{
						if (FrameworkEventSource.IsInitialized && FrameworkEventSource.Log.IsEnabled(EventLevel.Informational, (EventKeywords)16L))
						{
							FrameworkEventSource.Log.ThreadTransferSendObj(this, 1, string.Empty, true);
						}
						result = TimerQueue.Instance.UpdateTimer(this, dueTime, period);
					}
				}
			}
			return result;
		}

		public void Close()
		{
			TimerQueue instance = TimerQueue.Instance;
			lock (instance)
			{
				try
				{
				}
				finally
				{
					if (!this.m_canceled)
					{
						this.m_canceled = true;
						TimerQueue.Instance.DeleteTimer(this);
					}
				}
			}
		}

		public bool Close(WaitHandle toSignal)
		{
			bool flag = false;
			TimerQueue instance = TimerQueue.Instance;
			bool result;
			lock (instance)
			{
				try
				{
				}
				finally
				{
					if (this.m_canceled)
					{
						result = false;
					}
					else
					{
						this.m_canceled = true;
						this.m_notifyWhenNoCallbacksRunning = toSignal;
						TimerQueue.Instance.DeleteTimer(this);
						if (this.m_callbacksRunning == 0)
						{
							flag = true;
						}
						result = true;
					}
				}
			}
			if (flag)
			{
				this.SignalNoCallbacksRunning();
			}
			return result;
		}

		internal void Fire()
		{
			bool flag = false;
			TimerQueue instance = TimerQueue.Instance;
			lock (instance)
			{
				try
				{
				}
				finally
				{
					flag = this.m_canceled;
					if (!flag)
					{
						this.m_callbacksRunning++;
					}
				}
			}
			if (flag)
			{
				return;
			}
			this.CallCallback();
			bool flag3 = false;
			TimerQueue instance2 = TimerQueue.Instance;
			lock (instance2)
			{
				try
				{
				}
				finally
				{
					this.m_callbacksRunning--;
					if (this.m_canceled && this.m_callbacksRunning == 0 && this.m_notifyWhenNoCallbacksRunning != null)
					{
						flag3 = true;
					}
				}
			}
			if (flag3)
			{
				this.SignalNoCallbacksRunning();
			}
		}

		[SecuritySafeCritical]
		internal void SignalNoCallbacksRunning()
		{
			Win32Native.SetEvent(this.m_notifyWhenNoCallbacksRunning.SafeWaitHandle);
		}

		[SecuritySafeCritical]
		internal void CallCallback()
		{
			if (FrameworkEventSource.IsInitialized && FrameworkEventSource.Log.IsEnabled(EventLevel.Informational, (EventKeywords)16L))
			{
				FrameworkEventSource.Log.ThreadTransferReceiveObj(this, 1, string.Empty);
			}
			if (this.m_executionContext == null)
			{
				this.m_timerCallback(this.m_state);
				return;
			}
			using (ExecutionContext executionContext = this.m_executionContext.IsPreAllocatedDefault ? this.m_executionContext : this.m_executionContext.CreateCopy())
			{
				ContextCallback contextCallback = TimerQueueTimer.s_callCallbackInContext;
				if (contextCallback == null)
				{
					contextCallback = (TimerQueueTimer.s_callCallbackInContext = new ContextCallback(TimerQueueTimer.CallCallbackInContext));
				}
				ExecutionContext.Run(executionContext, contextCallback, this, true);
			}
		}

		[SecurityCritical]
		private static void CallCallbackInContext(object state)
		{
			TimerQueueTimer timerQueueTimer = (TimerQueueTimer)state;
			timerQueueTimer.m_timerCallback(timerQueueTimer.m_state);
		}

		internal TimerQueueTimer m_next;

		internal TimerQueueTimer m_prev;

		internal int m_startTicks;

		internal uint m_dueTime;

		internal uint m_period;

		private readonly TimerCallback m_timerCallback;

		private readonly object m_state;

		private readonly ExecutionContext m_executionContext;

		private int m_callbacksRunning;

		private volatile bool m_canceled;

		private volatile WaitHandle m_notifyWhenNoCallbacksRunning;

		[SecurityCritical]
		private static ContextCallback s_callCallbackInContext;
	}
}
