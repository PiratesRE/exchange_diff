using System;
using System.Threading;

namespace Microsoft.Exchange.Threading
{
	internal class FastReaderWriterLock
	{
		public void AcquireReaderLock(int millisecondsTimeout)
		{
			this.myLock.Enter();
			while (this.owners < 0 || this.numWriteWaiters != 0U)
			{
				if (this.readEvent == null)
				{
					this.LazyCreateEvent(ref this.readEvent, false);
				}
				else
				{
					this.WaitOnEvent(this.readEvent, ref this.numReadWaiters, millisecondsTimeout);
				}
			}
			this.owners++;
			this.myLock.Exit();
		}

		public void AcquireWriterLock(int millisecondsTimeout)
		{
			this.myLock.Enter();
			while (this.owners != 0)
			{
				if (this.writeEvent == null)
				{
					this.LazyCreateEvent(ref this.writeEvent, true);
				}
				else
				{
					this.WaitOnEvent(this.writeEvent, ref this.numWriteWaiters, millisecondsTimeout);
				}
			}
			this.owners = -1;
			this.myLock.Exit();
		}

		public void ReleaseReaderLock()
		{
			this.myLock.Enter();
			this.owners--;
			this.ExitAndWakeUpAppropriateWaiters();
		}

		public void ReleaseWriterLock()
		{
			this.myLock.Enter();
			this.owners = 0;
			this.ExitAndWakeUpAppropriateWaiters();
		}

		private void LazyCreateEvent(ref EventWaitHandle waitEvent, bool makeAutoResetEvent)
		{
			this.myLock.Exit();
			EventWaitHandle eventWaitHandle;
			if (makeAutoResetEvent)
			{
				eventWaitHandle = new AutoResetEvent(false);
			}
			else
			{
				eventWaitHandle = new ManualResetEvent(false);
			}
			this.myLock.Enter();
			if (waitEvent == null)
			{
				waitEvent = eventWaitHandle;
				return;
			}
			eventWaitHandle.Close();
		}

		private void WaitOnEvent(EventWaitHandle waitEvent, ref uint numWaiters, int millisecondsTimeout)
		{
			waitEvent.Reset();
			numWaiters += 1U;
			bool flag = false;
			this.myLock.Exit();
			try
			{
				if (!waitEvent.WaitOne(millisecondsTimeout, false))
				{
					throw new TimeoutException("ReaderWriterLock timeout expired");
				}
				flag = true;
			}
			finally
			{
				this.myLock.Enter();
				numWaiters -= 1U;
				if (!flag)
				{
					this.myLock.Exit();
				}
			}
		}

		private void ExitAndWakeUpAppropriateWaiters()
		{
			if (this.numWriteWaiters != 0U)
			{
				if (this.owners == 0)
				{
					this.myLock.Exit();
					this.writeEvent.Set();
					return;
				}
				this.myLock.Exit();
				return;
			}
			else
			{
				if (this.numReadWaiters == 0U)
				{
					this.myLock.Exit();
					return;
				}
				if (this.owners >= 0)
				{
					this.myLock.Exit();
					this.readEvent.Set();
					return;
				}
				this.myLock.Exit();
				return;
			}
		}

		private SpinLock myLock;

		private int owners;

		private uint numWriteWaiters;

		private uint numReadWaiters;

		private EventWaitHandle writeEvent;

		private EventWaitHandle readEvent;
	}
}
