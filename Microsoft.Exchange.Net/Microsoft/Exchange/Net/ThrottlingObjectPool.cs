using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ThrottlingObjectPool<DataType> : DisposeTrackableBase where DataType : new()
	{
		public ThrottlingObjectPool() : this(Environment.ProcessorCount)
		{
		}

		public ThrottlingObjectPool(int initialAllocatedObjects) : this(initialAllocatedObjects, int.MaxValue)
		{
		}

		public ThrottlingObjectPool(int initialAllocatedObjects, int maximumPoolSize)
		{
			if (initialAllocatedObjects < 0)
			{
				throw new ArgumentOutOfRangeException("initialAllocatedObjects", initialAllocatedObjects, "Should not be negative.");
			}
			if (maximumPoolSize < 1)
			{
				throw new ArgumentOutOfRangeException("maximumPoolSize", maximumPoolSize, "Needs be be >= 1.");
			}
			if (maximumPoolSize < initialAllocatedObjects)
			{
				throw new ArgumentOutOfRangeException("maximumPoolSize", maximumPoolSize, "Needs to be >= initialAllocatedObjects.");
			}
			this.poolCapacity = maximumPoolSize;
			int val = Math.Max(16, initialAllocatedObjects);
			int capacity = Math.Min(maximumPoolSize, val);
			this.bufferPool = new Stack<DataType>(capacity);
			for (int i = 0; i < initialAllocatedObjects; i++)
			{
				this.Release(this.AllocateNewObject());
			}
		}

		public int TotalBuffersAllocated { get; private set; }

		public DataType Acquire()
		{
			return this.Acquire(0);
		}

		public DataType Acquire(int waitCycle)
		{
			base.CheckDisposed();
			DataType result = default(DataType);
			bool flag = false;
			try
			{
				while (!flag)
				{
					flag = Monitor.TryEnter(this.acquireMonitor, waitCycle);
					if (flag)
					{
						if (this.poolNotEmptyEvent.WaitOne(waitCycle))
						{
							lock (this.bufferPool)
							{
								result = this.bufferPool.Pop();
								if (this.bufferPool.Count == 0)
								{
									this.poolNotEmptyEvent.Reset();
								}
								continue;
							}
						}
						return this.AllocateNewObject();
					}
					if (!this.throttlingObjectCreation)
					{
						lock (this.createMonitor)
						{
							if (!this.throttlingObjectCreation)
							{
								try
								{
									this.throttlingObjectCreation = true;
									return this.AllocateNewObject();
								}
								finally
								{
									if (waitCycle > 0)
									{
										this.timer = new System.Timers.Timer((double)waitCycle);
										this.timer.AutoReset = false;
										this.timer.Elapsed += this.ReleaseObjectCreationThrottle;
										this.timer.Enabled = true;
									}
									else
									{
										this.throttlingObjectCreation = false;
									}
								}
							}
						}
					}
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(this.acquireMonitor);
				}
			}
			return result;
		}

		public void Release(DataType value)
		{
			base.CheckDisposed();
			if (this.bufferPool.Count < this.poolCapacity)
			{
				lock (this.bufferPool)
				{
					this.bufferPool.Push(value);
					if (1 == this.bufferPool.Count)
					{
						this.poolNotEmptyEvent.Set();
					}
					return;
				}
			}
			IDisposable disposable = value as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ThrottlingObjectPool<DataType>>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				this.ReleaseObjectCreationThrottle(null, null);
				if (this.poolNotEmptyEvent != null)
				{
					this.poolNotEmptyEvent.Close();
				}
				lock (this.bufferPool)
				{
					while (this.bufferPool.Count > 0)
					{
						DataType dataType = this.bufferPool.Pop();
						IDisposable disposable = dataType as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
			}
		}

		private DataType AllocateNewObject()
		{
			this.TotalBuffersAllocated++;
			if (default(DataType) != null)
			{
				return default(DataType);
			}
			return Activator.CreateInstance<DataType>();
		}

		private void ReleaseObjectCreationThrottle(object source, ElapsedEventArgs eventArgs)
		{
			lock (this.createMonitor)
			{
				this.throttlingObjectCreation = false;
				if (this.timer != null)
				{
					this.timer.Dispose();
					this.timer = null;
				}
			}
		}

		private readonly Stack<DataType> bufferPool;

		private readonly object acquireMonitor = new object();

		private readonly object createMonitor = new object();

		private readonly ManualResetEvent poolNotEmptyEvent = new ManualResetEvent(false);

		private bool throttlingObjectCreation;

		private int poolCapacity;

		private System.Timers.Timer timer;
	}
}
