using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32;

namespace System.Threading.NetCore
{
	internal class TimerQueue
	{
		public static TimerQueue[] Instances { get; } = TimerQueue.CreateTimerQueues();

		private TimerQueue(int id)
		{
			this.m_id = id;
		}

		private static TimerQueue[] CreateTimerQueues()
		{
			TimerQueue[] array = new TimerQueue[Environment.ProcessorCount];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new TimerQueue(i);
			}
			return array;
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
				this.m_appDomainTimer = TimerQueue.CreateAppDomainTimer(num, this.m_id);
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
			TimerQueue.Instances[id].FireNextTimers();
		}

		[SecurityCritical]
		internal static void PauseAll()
		{
			foreach (TimerQueue timerQueue in TimerQueue.Instances)
			{
				timerQueue.Pause();
			}
		}

		[SecurityCritical]
		internal static void ResumeAll()
		{
			foreach (TimerQueue timerQueue in TimerQueue.Instances)
			{
				timerQueue.Resume();
			}
		}

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
					TimerQueueTimer timerQueueTimer = this.m_shortTimers;
					for (int i = 0; i < 2; i++)
					{
						while (timerQueueTimer != null)
						{
							TimerQueueTimer next = timerQueueTimer.m_next;
							uint num3;
							if (num <= tickCount - timerQueueTimer.m_startTicks)
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
							if (!timerQueueTimer.m_short && timerQueueTimer.m_dueTime <= 333U)
							{
								this.MoveTimerToCorrectList(timerQueueTimer, true);
							}
							timerQueueTimer = next;
						}
						if (i == 0)
						{
							int num4 = this.m_currentAbsoluteThreshold - tickCount;
							if (num4 > 0)
							{
								if (this.m_shortTimers == null && this.m_longTimers != null)
								{
									num2 = (uint)(num4 + 1);
									flag2 = true;
									break;
								}
								break;
							}
							else
							{
								timerQueueTimer = this.m_longTimers;
								this.m_currentAbsoluteThreshold = tickCount + 333;
							}
						}
					}
					if (flag2)
					{
						this.EnsureAppDomainTimerFiresBy(num2);
					}
				}
			}
		}

		[SecuritySafeCritical]
		private void FireNextTimers()
		{
			TimerQueueTimer timerQueueTimer = null;
			List<TimerQueueTimer> list = TimerQueue.t_timersToQueueToFire;
			if (list == null)
			{
				list = (TimerQueue.t_timersToQueueToFire = new List<TimerQueueTimer>());
			}
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
					TimerQueueTimer timerQueueTimer2 = this.m_shortTimers;
					for (int i = 0; i < 2; i++)
					{
						while (timerQueueTimer2 != null)
						{
							TimerQueueTimer next = timerQueueTimer2.m_next;
							uint num2 = (uint)(tickCount - timerQueueTimer2.m_startTicks);
							int num3 = (int)(timerQueueTimer2.m_dueTime - num2);
							if (num3 <= 0)
							{
								if (timerQueueTimer2.m_period != 4294967295U)
								{
									timerQueueTimer2.m_startTicks = tickCount;
									uint num4 = num2 - timerQueueTimer2.m_dueTime;
									timerQueueTimer2.m_dueTime = ((num4 < timerQueueTimer2.m_period) ? (timerQueueTimer2.m_period - num4) : 1U);
									if (timerQueueTimer2.m_dueTime < num)
									{
										flag2 = true;
										num = timerQueueTimer2.m_dueTime;
									}
									bool flag3 = (long)tickCount + (long)((ulong)timerQueueTimer2.m_dueTime) - (long)this.m_currentAbsoluteThreshold <= 0L;
									if (timerQueueTimer2.m_short != flag3)
									{
										this.MoveTimerToCorrectList(timerQueueTimer2, flag3);
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
									list.Add(timerQueueTimer2);
								}
							}
							else
							{
								if ((long)num3 < (long)((ulong)num))
								{
									flag2 = true;
									num = (uint)num3;
								}
								if (!timerQueueTimer2.m_short && num3 <= 333)
								{
									this.MoveTimerToCorrectList(timerQueueTimer2, true);
								}
							}
							timerQueueTimer2 = next;
						}
						if (i == 0)
						{
							int num5 = this.m_currentAbsoluteThreshold - tickCount;
							if (num5 > 0)
							{
								if (this.m_shortTimers == null && this.m_longTimers != null)
								{
									num = (uint)(num5 + 1);
									flag2 = true;
									break;
								}
								break;
							}
							else
							{
								timerQueueTimer2 = this.m_longTimers;
								this.m_currentAbsoluteThreshold = tickCount + 333;
							}
						}
					}
					if (flag2)
					{
						this.EnsureAppDomainTimerFiresBy(num);
					}
				}
			}
			if (list.Count != 0)
			{
				foreach (TimerQueueTimer workItem in list)
				{
					ThreadPool.UnsafeQueueCustomWorkItem(workItem, true);
				}
				list.Clear();
			}
			if (timerQueueTimer != null)
			{
				timerQueueTimer.Fire();
			}
		}

		public bool UpdateTimer(TimerQueueTimer timer, uint dueTime, uint period)
		{
			int tickCount = TimerQueue.TickCount;
			int num = (int)((long)tickCount + (long)((ulong)dueTime));
			bool flag = this.m_currentAbsoluteThreshold - num >= 0;
			if (timer.m_dueTime == 4294967295U)
			{
				timer.m_short = flag;
				this.LinkTimer(timer);
			}
			else if (timer.m_short != flag)
			{
				this.UnlinkTimer(timer);
				timer.m_short = flag;
				this.LinkTimer(timer);
			}
			timer.m_dueTime = dueTime;
			timer.m_period = ((period == 0U) ? uint.MaxValue : period);
			timer.m_startTicks = tickCount;
			return this.EnsureAppDomainTimerFiresBy(dueTime);
		}

		public void MoveTimerToCorrectList(TimerQueueTimer timer, bool shortList)
		{
			this.UnlinkTimer(timer);
			timer.m_short = shortList;
			this.LinkTimer(timer);
		}

		private void LinkTimer(TimerQueueTimer timer)
		{
			timer.m_next = (timer.m_short ? this.m_shortTimers : this.m_longTimers);
			if (timer.m_next != null)
			{
				timer.m_next.m_prev = timer;
			}
			timer.m_prev = null;
			if (timer.m_short)
			{
				this.m_shortTimers = timer;
				return;
			}
			this.m_longTimers = timer;
		}

		private void UnlinkTimer(TimerQueueTimer timer)
		{
			TimerQueueTimer timerQueueTimer = timer.m_next;
			if (timerQueueTimer != null)
			{
				timerQueueTimer.m_prev = timer.m_prev;
			}
			if (this.m_shortTimers == timer)
			{
				this.m_shortTimers = timerQueueTimer;
			}
			else if (this.m_longTimers == timer)
			{
				this.m_longTimers = timerQueueTimer;
			}
			timerQueueTimer = timer.m_prev;
			if (timerQueueTimer != null)
			{
				timerQueueTimer.m_next = timer.m_next;
			}
		}

		public void DeleteTimer(TimerQueueTimer timer)
		{
			if (timer.m_dueTime != 4294967295U)
			{
				this.UnlinkTimer(timer);
				timer.m_prev = null;
				timer.m_next = null;
				timer.m_dueTime = uint.MaxValue;
				timer.m_period = uint.MaxValue;
				timer.m_startTicks = 0;
				timer.m_short = false;
			}
		}

		private readonly int m_id;

		[SecurityCritical]
		private TimerQueue.AppDomainTimerSafeHandle m_appDomainTimer;

		private bool m_isAppDomainTimerScheduled;

		private int m_currentAppDomainTimerStartTicks;

		private uint m_currentAppDomainTimerDuration;

		private TimerQueueTimer m_shortTimers;

		private TimerQueueTimer m_longTimers;

		private int m_currentAbsoluteThreshold = 333;

		private const int ShortTimersThresholdMilliseconds = 333;

		private volatile int m_pauseTicks;

		[ThreadStatic]
		private static List<TimerQueueTimer> t_timersToQueueToFire;
	}
}
