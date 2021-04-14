using System;
using System.Threading;

namespace Microsoft.Exchange.Threading
{
	internal struct SpinLock
	{
		public void Enter()
		{
			if (Interlocked.CompareExchange(ref this.lockHeld, 1, 0) != 0)
			{
				this.EnterSpin();
			}
		}

		private void EnterSpin()
		{
			int num = 0;
			while (this.lockHeld != 0 || Interlocked.CompareExchange(ref this.lockHeld, 1, 0) != 0)
			{
				if (num < 20 && SpinLock.processorCount > 1)
				{
					Thread.SpinWait(100);
				}
				else if (num < 25)
				{
					Thread.Sleep(0);
				}
				else
				{
					Thread.Sleep(1);
				}
				num++;
			}
		}

		public void Exit()
		{
			this.lockHeld = 0;
		}

		public bool IsHeld
		{
			get
			{
				return this.lockHeld != 0;
			}
		}

		private volatile int lockHeld;

		private static readonly int processorCount = Environment.ProcessorCount;
	}
}
