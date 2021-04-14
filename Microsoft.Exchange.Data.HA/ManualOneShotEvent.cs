using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Data.HA
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ManualOneShotEvent : DisposeTrackableBase
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ReplayApiTracer;
			}
		}

		public ManualOneShotEvent(string debugName)
		{
			this.m_name = debugName;
		}

		public bool IsSignaled
		{
			get
			{
				return this.m_firstEventCompletedSignaled;
			}
		}

		public ManualOneShotEvent.Result WaitOne()
		{
			return this.WaitOne(InvokeWithTimeout.InfiniteTimeSpan);
		}

		public ManualOneShotEvent.Result WaitOne(TimeSpan timeout)
		{
			ManualOneShotEvent.IncrementResult incrementResult = this.IncrementRefCountIfNecessary();
			if (incrementResult == ManualOneShotEvent.IncrementResult.NotIncrementedAlreadySignaled)
			{
				return ManualOneShotEvent.Result.Success;
			}
			if (incrementResult == ManualOneShotEvent.IncrementResult.NotIncrementedShuttingDown)
			{
				return ManualOneShotEvent.Result.ShuttingDown;
			}
			ManualOneShotEvent.Result result;
			try
			{
				if (!this.m_firstEventCompleted.WaitOne(timeout))
				{
					ManualOneShotEvent.Tracer.TraceError<string, double>((long)this.GetHashCode(), "ManualOneShotEvent.WaitOne(): {0}: Waiting for event timed out after {1} msecs.", this.m_name, timeout.TotalMilliseconds);
					result = ManualOneShotEvent.Result.WaitTimedOut;
				}
				else
				{
					result = ManualOneShotEvent.Result.Success;
				}
			}
			finally
			{
				this.DecrementRefCountAndCloseIfNecessary();
			}
			return result;
		}

		public static int WaitAny(object[] waitHandles, TimeSpan timeout)
		{
			if (waitHandles == null)
			{
				throw new ArgumentNullException("waitHandles");
			}
			if (waitHandles.Length == 0)
			{
				throw new ArgumentException("Empty waitHandles should not be passed in.");
			}
			int num = -1;
			List<WaitHandle> list = new List<WaitHandle>(waitHandles.Length);
			ManualOneShotEvent.ManualOneShotInfo[] array = new ManualOneShotEvent.ManualOneShotInfo[waitHandles.Length];
			try
			{
				for (int i = 0; i < waitHandles.Length; i++)
				{
					object obj = waitHandles[i];
					if (obj == null)
					{
						throw new ArgumentNullException("An array element of waitHandles should not be null.");
					}
					array[i] = new ManualOneShotEvent.ManualOneShotInfo();
					if (obj is ManualOneShotEvent)
					{
						ManualOneShotEvent.ManualOneShotInfo manualOneShotInfo = array[i];
						ManualOneShotEvent manualOneShotEvent = obj as ManualOneShotEvent;
						manualOneShotInfo.IsManualOneShotEvent = true;
						manualOneShotInfo.ManualOneShotEvent = manualOneShotEvent;
						ManualOneShotEvent.IncrementResult incrementResult = manualOneShotEvent.IncrementRefCountIfNecessary();
						if (incrementResult == ManualOneShotEvent.IncrementResult.NotIncrementedShuttingDown)
						{
							manualOneShotInfo.IsClosed = true;
						}
						else if (incrementResult == ManualOneShotEvent.IncrementResult.NotIncrementedAlreadySignaled)
						{
							manualOneShotInfo.IsPreSignaled = true;
						}
						else
						{
							manualOneShotInfo.IsWaitRegistered = true;
							list.Add(manualOneShotEvent.m_firstEventCompleted);
						}
					}
					else
					{
						if (!(obj is WaitHandle))
						{
							throw new ArgumentException("An object of type {0} was passed in to the array. It must be either of type ManualOneShotEvent or WaitHandle.", obj.GetType().ToString());
						}
						list.Add(obj as WaitHandle);
					}
				}
				if (array.All((ManualOneShotEvent.ManualOneShotInfo info) => info.IsManualOneShotEvent && info.IsClosed))
				{
					ManualOneShotEvent.Tracer.TraceError(0L, "ManualOneShotEvent.WaitAny(): Every event is a ManualOneShotEvent and is already closed! Returning WaitTimeout.");
					return 258;
				}
				if (array.Any((ManualOneShotEvent.ManualOneShotInfo info) => info.IsManualOneShotEvent && info.IsPreSignaled))
				{
					int num2 = -1;
					for (int j = 0; j < array.Length; j++)
					{
						ManualOneShotEvent.ManualOneShotInfo manualOneShotInfo2 = array[j];
						if (manualOneShotInfo2.IsManualOneShotEvent && manualOneShotInfo2.IsPreSignaled && num2 == -1)
						{
							num2 = j;
						}
						if (manualOneShotInfo2.IsManualOneShotEvent && manualOneShotInfo2.IsWaitRegistered)
						{
							manualOneShotInfo2.ManualOneShotEvent.DecrementRefCountAndCloseIfNecessary();
							manualOneShotInfo2.IsWaitUnregistered = true;
						}
					}
					return num2;
				}
				num = WaitHandle.WaitAny(list.ToArray(), timeout);
			}
			finally
			{
				foreach (ManualOneShotEvent.ManualOneShotInfo manualOneShotInfo3 in array)
				{
					if (manualOneShotInfo3 != null && manualOneShotInfo3.IsManualOneShotEvent && manualOneShotInfo3.IsWaitRegistered && !manualOneShotInfo3.IsWaitUnregistered)
					{
						manualOneShotInfo3.ManualOneShotEvent.DecrementRefCountAndCloseIfNecessary();
						manualOneShotInfo3.IsWaitUnregistered = true;
					}
				}
			}
			if (num == 258)
			{
				ManualOneShotEvent.Tracer.TraceError<TimeSpan>(0L, "ManualOneShotEvent.WaitAny(): Wait timed out after {0}!", timeout);
				return num;
			}
			ManualOneShotEvent.Tracer.TraceDebug<int>(0L, "ManualOneShotEvent.WaitAny(): Event with *internal* index {0} was signaled.", num);
			int num3 = -1;
			int num4 = 0;
			for (int l = 0; l < array.Length; l++)
			{
				ManualOneShotEvent.ManualOneShotInfo manualOneShotInfo4 = array[l];
				if (!manualOneShotInfo4.IsManualOneShotEvent || !manualOneShotInfo4.IsClosed)
				{
					num4++;
				}
				if (num == num4 - 1)
				{
					num3 = l;
					break;
				}
			}
			ManualOneShotEvent.Tracer.TraceDebug<int>(0L, "ManualOneShotEvent.WaitAny(): Event with index {0} was signaled. Returning {0}.", num3);
			return num3;
		}

		public void Set()
		{
			if (this.m_firstEventCompletedSignaled)
			{
				return;
			}
			lock (this)
			{
				if (!this.m_firstEventCompletedSignaled && this.m_firstEventCompleted != null && !this.m_disposeCalled)
				{
					this.m_firstEventCompleted.Set();
					this.m_firstEventCompletedSignaled = true;
					ManualOneShotEvent.Tracer.TraceDebug<string>((long)this.GetHashCode(), "ManualOneShotEvent.Set(): {0}: First event completed!", this.m_name);
					this.Close();
				}
			}
		}

		public void Close()
		{
			lock (this)
			{
				if (this.m_disposeCalled)
				{
					return;
				}
				this.m_closeRequested = true;
				if (this.m_threadsWaiting > 0)
				{
					return;
				}
			}
			base.Dispose();
		}

		public new void Dispose()
		{
			this.Close();
		}

		protected override void InternalDispose(bool disposing)
		{
			lock (this)
			{
				if (!this.m_disposeCalled)
				{
					if (disposing)
					{
						this.m_firstEventCompleted.Close();
						this.m_firstEventCompleted = null;
					}
					this.m_disposeCalled = true;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ManualOneShotEvent>(this);
		}

		private ManualOneShotEvent.IncrementResult IncrementRefCountIfNecessary()
		{
			if (this.m_firstEventCompletedSignaled)
			{
				return ManualOneShotEvent.IncrementResult.NotIncrementedAlreadySignaled;
			}
			lock (this)
			{
				if (this.m_firstEventCompletedSignaled)
				{
					return ManualOneShotEvent.IncrementResult.NotIncrementedAlreadySignaled;
				}
				if (this.m_closeRequested)
				{
					ManualOneShotEvent.Tracer.TraceError<string>((long)this.GetHashCode(), "ManualOneShotEvent.IncrementRefCountIfNecessary(): {0}: m_closeRequested is true, which means the object is shutting down!", this.m_name);
					return ManualOneShotEvent.IncrementResult.NotIncrementedShuttingDown;
				}
				this.m_threadsWaiting++;
				ManualOneShotEvent.Tracer.TraceDebug<string, int>((long)this.GetHashCode(), "ManualOneShotEvent.IncrementRefCountIfNecessary(): {0}: Successfully registered a Waiter. Ref count is now: {1}", this.m_name, this.m_threadsWaiting);
			}
			return ManualOneShotEvent.IncrementResult.Incremented;
		}

		private void DecrementRefCountAndCloseIfNecessary()
		{
			lock (this)
			{
				this.m_threadsWaiting--;
				if (this.m_closeRequested)
				{
					this.Close();
				}
			}
		}

		private readonly string m_name;

		private ManualResetEvent m_firstEventCompleted = new ManualResetEvent(false);

		private bool m_firstEventCompletedSignaled;

		private bool m_closeRequested;

		private bool m_disposeCalled;

		private int m_threadsWaiting;

		internal enum Result
		{
			Success,
			WaitTimedOut,
			ShuttingDown
		}

		private enum IncrementResult
		{
			Incremented,
			NotIncrementedShuttingDown,
			NotIncrementedAlreadySignaled
		}

		private class ManualOneShotInfo
		{
			public ManualOneShotEvent ManualOneShotEvent { get; set; }

			public bool IsManualOneShotEvent { get; set; }

			public bool IsPreSignaled { get; set; }

			public bool IsWaitRegistered { get; set; }

			public bool IsWaitUnregistered { get; set; }

			public bool IsClosed { get; set; }
		}
	}
}
