using System;
using System.Threading;

namespace Microsoft.Exchange.Cluster.Shared
{
	public sealed class SynchronizedAction : IDisposable
	{
		private Action ProtectedAction { get; set; }

		public SynchronizedAction(Action action)
		{
			this.ProtectedAction = action;
			this.cycles = new SynchronizedAction.CycleData[2];
			for (int i = 0; i < 2; i++)
			{
				this.cycles[i] = new SynchronizedAction.CycleData();
			}
		}

		public bool TryAction(TimeSpan timeout)
		{
			bool flag = false;
			SynchronizedAction.CycleData cycleData = null;
			lock (this.lockObj)
			{
				if (this.disposed)
				{
					return false;
				}
				SynchronizedAction.CycleData cycleData2 = this.cycles[this.activeCycleIndex];
				if (cycleData2.state == SynchronizedAction.CycleState.Idle)
				{
					flag = true;
					cycleData2.state = SynchronizedAction.CycleState.Running;
				}
				else
				{
					if (timeout == TimeSpan.Zero)
					{
						return false;
					}
					cycleData = this.cycles[(this.activeCycleIndex + 1) % 2];
					if (cycleData.state == SynchronizedAction.CycleState.Idle)
					{
						cycleData.waitEvent.Reset();
						cycleData.state = SynchronizedAction.CycleState.Waiting;
						cycleData.waiterCount = 1;
					}
					else if (cycleData.state == SynchronizedAction.CycleState.Waiting)
					{
						cycleData.waiterCount++;
					}
				}
			}
			if (flag)
			{
				try
				{
					this.ProtectedAction();
					return flag;
				}
				finally
				{
					if (this.FinishCurrentCycle())
					{
						this.StartWorker();
					}
				}
			}
			if (cycleData != null)
			{
				return cycleData.waitEvent.WaitOne(timeout) && !this.disposed;
			}
			return flag;
		}

		private bool FinishCurrentCycle()
		{
			bool result;
			lock (this.lockObj)
			{
				if (this.disposed)
				{
					result = false;
				}
				else
				{
					SynchronizedAction.CycleData cycleData = this.cycles[this.activeCycleIndex];
					cycleData.state = SynchronizedAction.CycleState.Idle;
					cycleData.waiterCount = 0;
					cycleData.waitEvent.Set();
					this.activeCycleIndex = (this.activeCycleIndex + 1) % 2;
					cycleData = this.cycles[this.activeCycleIndex];
					if (cycleData.state == SynchronizedAction.CycleState.Waiting)
					{
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		private void StartWorker()
		{
			ThreadPool.QueueUserWorkItem(delegate(object param0)
			{
				this.WorkerEntryPoint();
			});
		}

		private void WorkerEntryPoint()
		{
			do
			{
				lock (this.lockObj)
				{
					if (this.disposed)
					{
						break;
					}
					SynchronizedAction.CycleData cycleData = this.cycles[this.activeCycleIndex];
					cycleData.state = SynchronizedAction.CycleState.Running;
				}
				this.ProtectedAction();
			}
			while (this.FinishCurrentCycle());
		}

		public void Dispose()
		{
			lock (this.lockObj)
			{
				if (!this.disposed)
				{
					this.disposed = true;
					foreach (SynchronizedAction.CycleData cycleData in this.cycles)
					{
						cycleData.waitEvent.Set();
						cycleData.waitEvent.Dispose();
					}
				}
			}
		}

		private int activeCycleIndex;

		private SynchronizedAction.CycleData[] cycles;

		private object lockObj = new object();

		private bool disposed;

		private enum CycleState
		{
			Idle,
			Running,
			Waiting
		}

		private class CycleData
		{
			public SynchronizedAction.CycleState state;

			public int waiterCount;

			public ManualResetEvent waitEvent = new ManualResetEvent(true);
		}
	}
}
