using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.NetCore;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System.Threading
{
	internal class TimerQueue
	{
		public static TimerQueue Instance
		{
			get
			{
				return TimerQueue.s_queue;
			}
		}

		private TimerQueue()
		{
		}

		private static int TickCount
		{
			[SecuritySafeCritical]
			get
			{
				if (!Environment.IsWindows8OrAbove)
				{
					return Environment.TickCount;
				}
				ulong num;
				if (!Win32Native.QueryUnbiasedInterruptTime(out num))
				{
					throw Marshal.GetExceptionForHR(Marshal.GetLastWin32Error());
				}
				return (int)((uint)(num / 10000UL));
			}
		}

		[SecuritySafeCritical]
		private bool EnsureAppDomainTimerFiresBy(uint requestedDuration)
		{
			uint num = Math.Min(requestedDuration, 268435455U);
			if (this.m_isAppDomainTimerScheduled)
			{
				uint num2 = (uint)(TimerQueue.TickCount - this.m_currentAppDomainTimerStartTicks);
				if (num2 >= this.m_currentAppDomainTimerDuration)
				{
					return true;
				}
				uint num3 = this.m_currentAppDomainTimerDuration - num2;
				if (num >= num3)
				{
					return true;
				}
			}
			if (this.m_pauseTicks != 0)
			{
				return true;
			}
			if (this.m_appDomainTimer == null || this.m_appDomainTimer.IsInvalid)
			{
				this.m_appDomainTimer = TimerQueue.CreateAppDomainTimer(num, 0);
				if (!this.m_appDomainTimer.IsInvalid)
				{
					this.m_isAppDomainTimerScheduled = true;
					this.m_currentAppDomainTimerStartTicks = TimerQueue.TickCount;
					this.m_currentAppDomainTimerDuration = num;
					return true;
				}
				return false;
			}
			else
			{
				if (TimerQueue.ChangeAppDomainTimer(this.m_appDomainTimer, num))
				{
					this.m_isAppDomainTimerScheduled = true;
					this.m_currentAppDomainTimerStartTicks = TimerQueue.TickCount;
					this.m_currentAppDomainTimerDuration = num;
					return true;
				}
				return false;
			}
		}

		[SecuritySafeCritical]
		internal static void AppDomainTimerCallback(int id)
		{
			if (Timer.UseNetCoreTimer)
			{
				TimerQueue.AppDomainTimerCallback(id);
				return;
			}
			TimerQueue.Instance.FireNextTimers();
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern TimerQueue.AppDomainTimerSafeHandle CreateAppDomainTimer(uint dueTime, int id);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern bool ChangeAppDomainTimer(TimerQueue.AppDomainTimerSafeHandle handle, uint dueTime);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern bool DeleteAppDomainTimer(IntPtr handle);

		[SecurityCritical]
		internal void Pause()
		{
			lock (this)
			{
				if (this.m_appDomainTimer != null && !this.m_appDomainTimer.IsInvalid)
				{
					this.m_appDomainTimer.Dispose();
					this.m_appDomainTimer = null;
					this.m_isAppDomainTimerScheduled = false;
					this.m_pauseTicks = TimerQueue.TickCount;
				}
			}
		}

		[SecurityCritical]
		internal void Resume()
		{
			lock (this)
			{
				try
				{
				}
				finally
				{
					int pauseTicks = this.m_pauseTicks;
					this.m_pauseTicks = 0;
					int tickCount = TimerQueue.TickCount;
					int num = tickCount - pauseTicks;
					bool flag2 = false;
					uint num2 = uint.MaxValue;
					for (TimerQueueTimer timerQueueTimer = this.m_timers; timerQueueTimer != null; timerQueueTimer = timerQueueTimer.m_next)
					{
						uint num3;
						if (timerQueueTimer.m_startTicks <= pauseTicks)
						{
							num3 = (uint)(pauseTicks - timerQueueTimer.m_startTicks);
						}
						else
						{
							num3 = (uint)(tickCount - timerQueueTimer.m_startTicks);
						}
						timerQueueTimer.m_dueTime = ((timerQueueTimer.m_dueTime > num3) ? (timerQueueTimer.m_dueTime - num3) : 0U);
						timerQueueTimer.m_startTicks = tickCount;
						if (timerQueueTimer.m_dueTime < num2)
						{
							flag2 = true;
							num2 = timerQueueTimer.m_dueTime;
						}
					}
					if (flag2)
					{
						this.EnsureAppDomainTimerFiresBy(num2);
					}
				}
			}
		}

		private void FireNextTimers()
		{
			TimerQueueTimer timerQueueTimer = null;
			lock (this)
			{
				try
				{
				}
				finally
				{
					this.m_isAppDomainTimerScheduled = false;
					bool flag2 = false;
					uint num = uint.MaxValue;
					int tickCount = TimerQueue.TickCount;
					TimerQueueTimer timerQueueTimer2 = this.m_timers;
					while (timerQueueTimer2 != null)
					{
						uint num2 = (uint)(tickCount - timerQueueTimer2.m_startTicks);
						if (num2 >= timerQueueTimer2.m_dueTime)
						{
							TimerQueueTimer next = timerQueueTimer2.m_next;
							if (timerQueueTimer2.m_period != 4294967295U)
							{
								timerQueueTimer2.m_startTicks = tickCount;
								timerQueueTimer2.m_dueTime = timerQueueTimer2.m_period;
								if (timerQueueTimer2.m_dueTime < num)
								{
									flag2 = true;
									num = timerQueueTimer2.m_dueTime;
								}
							}
							else
							{
								this.DeleteTimer(timerQueueTimer2);
							}
							if (timerQueueTimer == null)
							{
								timerQueueTimer = timerQueueTimer2;
							}
							else
							{
								TimerQueue.QueueTimerCompletion(timerQueueTimer2);
							}
							timerQueueTimer2 = next;
						}
						else
						{
							uint num3 = timerQueueTimer2.m_dueTime - num2;
							if (num3 < num)
							{
								flag2 = true;
								num = num3;
							}
							timerQueueTimer2 = timerQueueTimer2.m_next;
						}
					}
					if (flag2)
					{
						this.EnsureAppDomainTimerFiresBy(num);
					}
				}
			}
			if (timerQueueTimer != null)
			{
				timerQueueTimer.Fire();
			}
		}

		[SecuritySafeCritical]
		private static void QueueTimerCompletion(TimerQueueTimer timer)
		{
			WaitCallback waitCallback = TimerQueue.s_fireQueuedTimerCompletion;
			if (waitCallback == null)
			{
				waitCallback = (TimerQueue.s_fireQueuedTimerCompletion = new WaitCallback(TimerQueue.FireQueuedTimerCompletion));
			}
			ThreadPool.UnsafeQueueUserWorkItem(waitCallback, timer);
		}

		private static void FireQueuedTimerCompletion(object state)
		{
			((TimerQueueTimer)state).Fire();
		}

		public bool UpdateTimer(TimerQueueTimer timer, uint dueTime, uint period)
		{
			if (timer.m_dueTime == 4294967295U)
			{
				timer.m_next = this.m_timers;
				timer.m_prev = null;
				if (timer.m_next != null)
				{
					timer.m_next.m_prev = timer;
				}
				this.m_timers = timer;
			}
			timer.m_dueTime = dueTime;
			timer.m_period = ((period == 0U) ? uint.MaxValue : period);
			timer.m_startTicks = TimerQueue.TickCount;
			return this.EnsureAppDomainTimerFiresBy(dueTime);
		}

		public void DeleteTimer(TimerQueueTimer timer)
		{
			if (timer.m_dueTime != 4294967295U)
			{
				if (timer.m_next != null)
				{
					timer.m_next.m_prev = timer.m_prev;
				}
				if (timer.m_prev != null)
				{
					timer.m_prev.m_next = timer.m_next;
				}
				if (this.m_timers == timer)
				{
					this.m_timers = timer.m_next;
				}
				timer.m_dueTime = uint.MaxValue;
				timer.m_period = uint.MaxValue;
				timer.m_startTicks = 0;
				timer.m_prev = null;
				timer.m_next = null;
			}
		}

		private static TimerQueue s_queue = new TimerQueue();

		[SecurityCritical]
		private TimerQueue.AppDomainTimerSafeHandle m_appDomainTimer;

		private bool m_isAppDomainTimerScheduled;

		private int m_currentAppDomainTimerStartTicks;

		private uint m_currentAppDomainTimerDuration;

		private TimerQueueTimer m_timers;

		private volatile int m_pauseTicks;

		private static WaitCallback s_fireQueuedTimerCompletion;

		[SecurityCritical]
		internal class AppDomainTimerSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			public AppDomainTimerSafeHandle() : base(true)
			{
			}

			[SecurityCritical]
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			protected override bool ReleaseHandle()
			{
				return TimerQueue.DeleteAppDomainTimer(this.handle);
			}
		}
	}
}
