using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Win32;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class Throttle : Base, IThreadPool
	{
		public Throttle(string name, int initialThrottleValue) : this(null, name, initialThrottleValue, Throttle.unThrottledThreadPool)
		{
		}

		public static Throttle CreateParentThrottle(string name, int initialThrottleValue)
		{
			return new Throttle(Configuration.ParametersRegistryKeyPath, name, initialThrottleValue, Throttle.unThrottledThreadPool);
		}

		public Throttle(string name, int initialThrottleValue, Throttle baseThreadPool) : this(Configuration.ParametersRegistryKeyPath, name, initialThrottleValue, baseThreadPool)
		{
		}

		private Throttle(string registryKeyPath, string name, int initialThrottleValue, IThreadPool baseThreadPool)
		{
			this.name = name;
			this.baseThreadPool = baseThreadPool;
			this.currentThrottle = Throttle.GetInitialThrottleValue(registryKeyPath, name, initialThrottleValue);
			this.openThrottle = this.currentThrottle;
			base.TracePfd("PFD AIS {0} {1}: constructed", new object[]
			{
				24151,
				this
			});
		}

		public int ThrottleValue
		{
			get
			{
				return this.currentThrottle;
			}
		}

		public int OpenThrottleValue
		{
			get
			{
				return this.openThrottle;
			}
		}

		public bool IsOverThrottle
		{
			get
			{
				return this.activeWorkItems > this.currentThrottle || (this.baseThreadPool is Throttle && ((Throttle)this.baseThreadPool).IsOverThrottle);
			}
		}

		public void QueueUserWorkItem(WaitCallback callback, object state)
		{
			lock (this.queue)
			{
				this.queue.Enqueue(new Throttle.WorkItemState(callback, state));
				ExTraceGlobals.ThrottleTracer.TraceDebug<Throttle>((long)this.GetHashCode(), "{0}: Enqueued new workitem", this);
				this.TraceState();
				this.AddPendingWorkItemsIfNecessary();
			}
		}

		public void SetThrottle(int newThrottleValue)
		{
			lock (this.queue)
			{
				this.currentThrottle = newThrottleValue;
				base.TracePfd("PFD AIS {0} {1}: New throttle value: {2}", new object[]
				{
					28247,
					this,
					this.currentThrottle
				});
				this.TraceState();
				this.AddPendingWorkItemsIfNecessary();
			}
		}

		public void OpenThrottle()
		{
			this.SetThrottle(this.openThrottle);
		}

		public void CloseThrottle()
		{
			this.SetThrottle(0);
		}

		public override void ExportToQueryableObject(QueryableObject queryableObject)
		{
			base.ExportToQueryableObject(queryableObject);
			QueryableThrottle queryableThrottle = queryableObject as QueryableThrottle;
			if (queryableThrottle != null)
			{
				queryableThrottle.ThrottleName = this.name;
				queryableThrottle.CurrentThrottle = this.currentThrottle;
				queryableThrottle.ActiveWorkItems = this.activeWorkItems;
				queryableThrottle.PendingWorkItemsOnBase = this.pendingWorkItemsOnBase;
				queryableThrottle.QueueLength = this.queue.Count;
				queryableThrottle.OverThrottle = this.IsOverThrottle;
			}
		}

		private static int GetInitialThrottleValue(string registryKeyPath, string throttleName, int defaultValue)
		{
			if (string.IsNullOrEmpty(registryKeyPath))
			{
				return defaultValue;
			}
			lock (Throttle.registryCache)
			{
				Dictionary<string, object> dictionary = null;
				if (Throttle.registryCache.TryGetValue(registryKeyPath, out dictionary))
				{
					if (dictionary == null)
					{
						return defaultValue;
					}
					object obj2 = null;
					string key = throttleName + "Throttle";
					if (dictionary.TryGetValue(key, out obj2))
					{
						if (obj2 == null || !(obj2 is int))
						{
							return defaultValue;
						}
						return (int)obj2;
					}
				}
			}
			RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(registryKeyPath);
			if (registryKey == null)
			{
				lock (Throttle.registryCache)
				{
					Throttle.registryCache[registryKeyPath] = null;
				}
				return defaultValue;
			}
			using (registryKey)
			{
				string key2 = throttleName + "Throttle";
				object value = registryKey.GetValue(key2);
				lock (Throttle.registryCache)
				{
					Dictionary<string, object> dictionary2 = null;
					if (!Throttle.registryCache.TryGetValue(registryKeyPath, out dictionary2))
					{
						dictionary2 = new Dictionary<string, object>();
						Throttle.registryCache[registryKeyPath] = dictionary2;
					}
					if (value is int)
					{
						dictionary2[key2] = value;
						return (int)value;
					}
					dictionary2[key2] = null;
				}
			}
			return defaultValue;
		}

		private void AddPendingWorkItemsIfNecessary()
		{
			ExTraceGlobals.ThrottleTracer.TraceDebug<Throttle>((long)this.GetHashCode(), "{0}: AddPendingWorkItemsIfNecessary", this);
			while (this.pendingWorkItemsOnBase < Math.Min(this.currentThrottle - this.activeWorkItems, this.queue.Count))
			{
				this.baseThreadPool.QueueUserWorkItem(Throttle.waitCallback, this);
				this.pendingWorkItemsOnBase++;
			}
			this.TraceState();
		}

		private static void WaitCallback(object state)
		{
			((Throttle)state).InternalWaitCallback();
		}

		private void InternalWaitCallback()
		{
			Throttle.WorkItemState workItemState;
			lock (this.queue)
			{
				ExTraceGlobals.ThrottleTracer.TraceDebug<Throttle>((long)this.GetHashCode(), "{0}: WaitCallback", this);
				this.TraceState();
				this.pendingWorkItemsOnBase--;
				if (this.queue.Count <= 0)
				{
					ExTraceGlobals.ThrottleTracer.TraceDebug<Throttle>((long)this.GetHashCode(), "{0}: Queue is empty.  Doing nothing", this);
					return;
				}
				if (this.activeWorkItems >= this.currentThrottle)
				{
					ExTraceGlobals.ThrottleTracer.TraceDebug<Throttle>((long)this.GetHashCode(), "{0}: ActiveWorkItems already meets or exceeds throttle.  Doing nothing.", this);
					this.TraceState();
					return;
				}
				workItemState = this.queue.Dequeue();
				this.activeWorkItems++;
				ExTraceGlobals.ThrottleTracer.TraceDebug<Throttle>((long)this.GetHashCode(), "{0}: Calling workitem...", this);
				this.TraceState();
			}
			try
			{
				workItemState.WorkItem(workItemState.State);
			}
			finally
			{
				lock (this.queue)
				{
					this.activeWorkItems--;
					ExTraceGlobals.ThrottleTracer.TraceDebug<Throttle>((long)this.GetHashCode(), "{0}: Workitem has returned.", this);
					this.TraceState();
					this.AddPendingWorkItemsIfNecessary();
				}
			}
		}

		private void TraceState()
		{
			ExTraceGlobals.ThrottleTracer.TraceDebug((long)this.GetHashCode(), "{0}: currentThrottle: {1}; pendingWorkItemsOnBase: {2}; activeWorkItems: {3}; queue.Count: {4}", new object[]
			{
				this,
				this.currentThrottle,
				this.pendingWorkItemsOnBase,
				this.activeWorkItems,
				this.queue.Count
			});
		}

		private const string ThrottleSuffix = "Throttle";

		private static IThreadPool unThrottledThreadPool = new UnThrottledThreadPool();

		private static WaitCallback waitCallback = new WaitCallback(Throttle.WaitCallback);

		private static Dictionary<string, Dictionary<string, object>> registryCache = new Dictionary<string, Dictionary<string, object>>();

		private readonly string name;

		private int currentThrottle = int.MaxValue;

		private int openThrottle = int.MaxValue;

		private int pendingWorkItemsOnBase;

		private int activeWorkItems;

		private Queue<Throttle.WorkItemState> queue = new Queue<Throttle.WorkItemState>();

		private IThreadPool baseThreadPool;

		private struct WorkItemState
		{
			public WorkItemState(WaitCallback workItem, object state)
			{
				this.WorkItem = workItem;
				this.State = state;
			}

			public WaitCallback WorkItem;

			public object State;
		}
	}
}
