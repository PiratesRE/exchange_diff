using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class DispatchPool : IDisposable
	{
		public DispatchPool(string threadPoolDescription, int maxTaskQueueSize, int maxThreads, int minThreads, IExPerformanceCounter counterDispatchTaskQueueLength, IExPerformanceCounter counterDispatchTaskThreads, IExPerformanceCounter counterDispatchTaskActiveThreads, IExPerformanceCounter counterDispatchTaskRate)
		{
			if (maxTaskQueueSize < 1)
			{
				throw new ArgumentOutOfRangeException("maxTaskQueueSize");
			}
			if (maxThreads < 1)
			{
				throw new ArgumentOutOfRangeException("maxThreads");
			}
			if (minThreads < 0)
			{
				throw new ArgumentOutOfRangeException("minThreads");
			}
			if (minThreads > maxThreads)
			{
				minThreads = maxThreads;
			}
			this.threadPoolDescription = threadPoolDescription;
			this.counterDispatchTaskQueueLength = counterDispatchTaskQueueLength;
			DispatchPool.SetPerformanceCounter(this.counterDispatchTaskQueueLength, 0);
			this.counterDispatchTaskThreads = counterDispatchTaskThreads;
			DispatchPool.SetPerformanceCounter(this.counterDispatchTaskThreads, 0);
			this.counterDispatchTaskActiveThreads = counterDispatchTaskActiveThreads;
			DispatchPool.SetPerformanceCounter(this.counterDispatchTaskActiveThreads, 0);
			this.counterDispatchTaskRate = counterDispatchTaskRate;
			this.maxTaskQueueSize = maxTaskQueueSize;
			this.minThreads = minThreads;
			this.taskQueue = new Queue<DispatchTask>(this.maxTaskQueueSize);
			this.threadInfoArray = new DispatchPool.DispatchThreadInfo[maxThreads];
			for (int i = 0; i < maxThreads; i++)
			{
				this.threadInfoArray[i] = new DispatchPool.DispatchThreadInfo();
			}
			this.threadStateCount[0] = maxThreads;
			lock (this.poolLock)
			{
				for (int j = 0; j < this.minThreads; j++)
				{
					DispatchPool.UpdateState(this.threadInfoArray[j], this.threadStateCount, DispatchPool.DispatchThreadState.StartingUp);
					this.threadInfoArray[j].Wakeup();
					this.SpinUpThread(j);
				}
			}
		}

		public int ActiveThreads
		{
			get
			{
				return this.threadStateCount[4];
			}
		}

		public bool SubmitTask(DispatchTask task)
		{
			DispatchPool.IncrementPerformanceCounter(this.counterDispatchTaskQueueLength);
			bool flag = false;
			try
			{
				lock (this.poolLock)
				{
					if (this.isDisposed || this.taskQueue.Count >= this.maxTaskQueueSize)
					{
						return false;
					}
					this.taskQueue.Enqueue(task);
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					DispatchPool.DecrementPerformanceCounter(this.counterDispatchTaskQueueLength);
				}
			}
			this.ProcessThreads();
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void IncrementPerformanceCounter(IExPerformanceCounter counter)
		{
			if (counter != null)
			{
				counter.Increment();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void DecrementPerformanceCounter(IExPerformanceCounter counter)
		{
			if (counter != null)
			{
				counter.Decrement();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void SetPerformanceCounter(IExPerformanceCounter counter, int value)
		{
			if (counter != null)
			{
				counter.RawValue = (long)value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void UpdateState(DispatchPool.DispatchThreadInfo threadInfo, int[] threadStateCount, DispatchPool.DispatchThreadState newState)
		{
			threadStateCount[(int)threadInfo.State]--;
			threadInfo.State = newState;
			threadStateCount[(int)newState] = threadStateCount[(int)newState] + 1;
		}

		private bool TryGetTask(DispatchPool.DispatchThreadInfo threadInfo, out bool shutdown, out DispatchTask task)
		{
			task = null;
			shutdown = false;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			lock (this.poolLock)
			{
				if (this.isDisposed || threadInfo.State == DispatchPool.DispatchThreadState.ShuttingDown)
				{
					DispatchPool.UpdateState(threadInfo, this.threadStateCount, DispatchPool.DispatchThreadState.None);
					shutdown = true;
					return false;
				}
				if (this.taskQueue.Count == 0)
				{
					if (threadInfo.State != DispatchPool.DispatchThreadState.Inactive)
					{
						if (threadInfo.State == DispatchPool.DispatchThreadState.Active)
						{
							flag2 = true;
						}
						DispatchPool.UpdateState(threadInfo, this.threadStateCount, DispatchPool.DispatchThreadState.Inactive);
					}
				}
				else
				{
					if (threadInfo.State != DispatchPool.DispatchThreadState.Active)
					{
						DispatchPool.UpdateState(threadInfo, this.threadStateCount, DispatchPool.DispatchThreadState.Active);
						flag = true;
					}
					task = this.taskQueue.Dequeue();
					flag3 = true;
				}
			}
			if (flag3)
			{
				DispatchPool.DecrementPerformanceCounter(this.counterDispatchTaskQueueLength);
			}
			if (flag)
			{
				DispatchPool.IncrementPerformanceCounter(this.counterDispatchTaskActiveThreads);
			}
			else if (flag2)
			{
				DispatchPool.DecrementPerformanceCounter(this.counterDispatchTaskActiveThreads);
			}
			this.ProcessThreads();
			return task != null;
		}

		private bool DispatchTasks(DispatchPool.DispatchThreadInfo threadInfo)
		{
			DispatchTask dispatchTask = null;
			bool flag = false;
			while (this.TryGetTask(threadInfo, out flag, out dispatchTask))
			{
				dispatchTask.Execute();
				dispatchTask = null;
				DispatchPool.IncrementPerformanceCounter(this.counterDispatchTaskRate);
			}
			return !flag;
		}

		private void DispatchWorkerThread(object obj)
		{
			DispatchPool.DispatchThreadInfo dispatchThreadInfo = (DispatchPool.DispatchThreadInfo)obj;
			DispatchPool.IncrementPerformanceCounter(this.counterDispatchTaskThreads);
			try
			{
				while (this.DispatchTasks(dispatchThreadInfo))
				{
					while (!dispatchThreadInfo.Wait())
					{
						this.ProcessThreads();
					}
				}
			}
			finally
			{
				DispatchPool.DecrementPerformanceCounter(this.counterDispatchTaskThreads);
			}
		}

		internal static void CheckThreads(int taskQueueSize, int[] threadStateCount, DispatchPool.DispatchThreadInfo[] threadInfoArray, int minThreads, out int startupThread, out int shutdownThread, out int activateThread)
		{
			startupThread = -1;
			shutdownThread = -1;
			activateThread = -1;
			int num = threadStateCount[3] + threadStateCount[1];
			if (threadStateCount[4] + num >= threadInfoArray.Length)
			{
				return;
			}
			if (taskQueueSize > num)
			{
				if (threadStateCount[2] + threadStateCount[5] > 0)
				{
					for (int i = 0; i < threadInfoArray.Length; i++)
					{
						DispatchPool.DispatchThreadInfo dispatchThreadInfo = threadInfoArray[i];
						if (dispatchThreadInfo.State == DispatchPool.DispatchThreadState.Inactive || dispatchThreadInfo.State == DispatchPool.DispatchThreadState.ShuttingDown)
						{
							DispatchPool.UpdateState(dispatchThreadInfo, threadStateCount, DispatchPool.DispatchThreadState.Activating);
							activateThread = i;
							break;
						}
					}
				}
				if (activateThread == -1 && threadStateCount[0] > 0)
				{
					for (int j = 0; j < threadInfoArray.Length; j++)
					{
						DispatchPool.DispatchThreadInfo dispatchThreadInfo = threadInfoArray[j];
						if (dispatchThreadInfo.State == DispatchPool.DispatchThreadState.None)
						{
							DispatchPool.UpdateState(dispatchThreadInfo, threadStateCount, DispatchPool.DispatchThreadState.StartingUp);
							startupThread = j;
							break;
						}
					}
				}
			}
			if (startupThread == -1 && threadStateCount[2] > 0)
			{
				DateTime utcNow = DateTime.UtcNow;
				for (int k = threadInfoArray.Length - 1; k >= minThreads; k--)
				{
					DispatchPool.DispatchThreadInfo dispatchThreadInfo = threadInfoArray[k];
					if (dispatchThreadInfo.State == DispatchPool.DispatchThreadState.Inactive && utcNow - dispatchThreadInfo.InactiveTime > DispatchPool.DispatchThreadInfo.InactiveThreadShutdownDelay)
					{
						DispatchPool.UpdateState(dispatchThreadInfo, threadStateCount, DispatchPool.DispatchThreadState.ShuttingDown);
						shutdownThread = k;
						return;
					}
				}
			}
		}

		private void ProcessThreads()
		{
			if (Interlocked.CompareExchange(ref this.processThreadsLock, 1, 0) == 1)
			{
				return;
			}
			bool flag = true;
			try
			{
				int num;
				int num2;
				int num3;
				do
				{
					lock (this.poolLock)
					{
						if (this.isDisposed)
						{
							break;
						}
						DispatchPool.CheckThreads(this.taskQueue.Count, this.threadStateCount, this.threadInfoArray, this.minThreads, out num, out num2, out num3);
						if (num == -1 && num2 == -1 && num3 == -1)
						{
							flag = false;
							this.processThreadsLock = 0;
						}
					}
					if (num != -1)
					{
						this.threadInfoArray[num].Wakeup();
						this.SpinUpThread(num);
					}
					if (num2 != -1)
					{
						this.threadInfoArray[num2].Wakeup();
					}
					if (num3 != -1)
					{
						this.threadInfoArray[num3].Wakeup();
					}
				}
				while (num != -1 || num2 != -1 || num3 != -1);
			}
			finally
			{
				if (flag)
				{
					this.processThreadsLock = 0;
				}
			}
		}

		private void SpinUpThread(int threadId)
		{
			new Thread(new ParameterizedThreadStart(this.DispatchWorkerThread))
			{
				IsBackground = true,
				Name = string.Format("{0}{1}", this.threadPoolDescription, threadId)
			}.Start(this.threadInfoArray[threadId]);
		}

		public void Dispose()
		{
			if (this.isDisposed)
			{
				return;
			}
			Queue<DispatchTask> queue = null;
			lock (this.poolLock)
			{
				if (this.isDisposed)
				{
					return;
				}
				this.isDisposed = true;
				queue = this.taskQueue;
				this.taskQueue.Clear();
			}
			if (this.threadInfoArray != null)
			{
				for (int i = 0; i < this.threadInfoArray.Length; i++)
				{
					this.threadInfoArray[i].Wakeup();
				}
			}
			if (queue != null)
			{
				while (queue.Count > 0)
				{
					DispatchTask dispatchTask = queue.Dequeue();
					dispatchTask.Cancel();
				}
			}
		}

		private readonly int maxTaskQueueSize;

		private readonly int minThreads;

		private readonly object poolLock = new object();

		private readonly Queue<DispatchTask> taskQueue;

		private readonly DispatchPool.DispatchThreadInfo[] threadInfoArray;

		private readonly int[] threadStateCount = new int[6];

		private IExPerformanceCounter counterDispatchTaskQueueLength;

		private IExPerformanceCounter counterDispatchTaskThreads;

		private IExPerformanceCounter counterDispatchTaskActiveThreads;

		private IExPerformanceCounter counterDispatchTaskRate;

		private string threadPoolDescription;

		private int processThreadsLock;

		private bool isDisposed;

		internal enum DispatchThreadState
		{
			None,
			StartingUp,
			Inactive,
			Activating,
			Active,
			ShuttingDown,
			Max
		}

		internal class DispatchThreadInfo
		{
			internal DispatchThreadInfo()
			{
				this.state = DispatchPool.DispatchThreadState.None;
				this.inactiveTime = DateTime.MinValue;
			}

			internal DispatchThreadInfo(DispatchPool.DispatchThreadState state, DateTime inactiveTime)
			{
				this.state = state;
				this.inactiveTime = inactiveTime;
			}

			public AutoResetEvent WaitEvent
			{
				get
				{
					return this.waitEvent;
				}
			}

			public DispatchPool.DispatchThreadState State
			{
				get
				{
					return this.state;
				}
				set
				{
					if (value == DispatchPool.DispatchThreadState.Inactive && (this.state == DispatchPool.DispatchThreadState.Active || this.state == DispatchPool.DispatchThreadState.StartingUp))
					{
						this.inactiveTime = DateTime.UtcNow;
					}
					this.state = value;
				}
			}

			public DateTime InactiveTime
			{
				get
				{
					return this.inactiveTime;
				}
			}

			public void Wakeup()
			{
				this.waitEvent.Set();
			}

			public bool Wait()
			{
				return this.waitEvent.WaitOne(DispatchPool.DispatchThreadInfo.periodicWakeupDelay);
			}

			private static readonly TimeSpan periodicWakeupDelay = TimeSpan.FromMinutes(1.0);

			public static readonly TimeSpan InactiveThreadShutdownDelay = TimeSpan.FromMinutes(5.0);

			private readonly AutoResetEvent waitEvent = new AutoResetEvent(false);

			private DispatchPool.DispatchThreadState state;

			private DateTime inactiveTime;
		}
	}
}
