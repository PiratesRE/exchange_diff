using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class Subscriptions : IDisposable
	{
		public Subscriptions()
		{
			ExTraceGlobals.SubscriptionsTracer.TraceDebug((long)this.GetHashCode(), "Subscriptions constructor called");
			this.subscriptions = new Dictionary<string, SubscriptionBase>();
			this.subscriptionsByUser = new Dictionary<BudgetKey, int>();
			EventQueue.PollingInterval = new TimeSpan(0, 0, Global.EventQueuePollingInterval);
			PerformanceMonitor.UpdateActiveSubscriptionsCounter(0L);
		}

		public static void Initialize()
		{
			if (Subscriptions.Singleton == null)
			{
				Subscriptions.Singleton = new Subscriptions();
			}
		}

		public static Subscriptions Singleton { get; private set; }

		internal List<SubscriptionBase> SubscriptionsList
		{
			get
			{
				List<SubscriptionBase> result;
				lock (this.lockObject)
				{
					result = new List<SubscriptionBase>(this.subscriptions.Values);
				}
				return result;
			}
		}

		internal void Add(SubscriptionBase subscription)
		{
			ExTraceGlobals.SubscriptionsTracer.TraceDebug<string>((long)this.GetHashCode(), "Subscriptions.Add called. Before lock.  SubscriptionId: {0}", subscription.SubscriptionId);
			lock (this.lockObject)
			{
				ExTraceGlobals.SubscriptionsTracer.TraceDebug<string>((long)this.GetHashCode(), "Subscriptions.Add. After lock.  SubscriptionId: {0}", subscription.SubscriptionId);
				this.subscriptions.Add(subscription.SubscriptionId, subscription);
				this.IncrementSubscriptionsForUser(subscription);
				PerformanceMonitor.UpdateActiveSubscriptionsCounter((long)this.subscriptions.Count);
				this.InitializeDeleteExpiredSubscriptionsTimer();
			}
		}

		private void IncrementSubscriptionsForUser(SubscriptionBase subscription)
		{
			lock (this.lockObject)
			{
				int num = 0;
				this.subscriptionsByUser.TryGetValue(subscription.BudgetKey, out num);
				ExTraceGlobals.SubscriptionsTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "[Subscriptions::IncrementSubscriptionsForUser] User '{0}' had {1} subscriptions.  Incrementing to {2}.", subscription.BudgetKey.ToString(), num, num + 1);
				this.subscriptionsByUser[subscription.BudgetKey] = num + 1;
			}
		}

		private void DecrementSubscriptionsForUser(SubscriptionBase subscription)
		{
			lock (this.lockObject)
			{
				int num = 0;
				if (!this.subscriptionsByUser.TryGetValue(subscription.BudgetKey, out num))
				{
					ExTraceGlobals.SubscriptionsTracer.TraceError<string>((long)this.GetHashCode(), "[Subscriptions::DecrementSubscriptionsForUser] User {0} was not found in the SubscriptionsByUser dictionary.", subscription.BudgetKey.ToString());
					throw new InvalidOperationException(string.Format("[Subscriptions::DecrementSubscriptionsForUser] User {0} was not found in the SubscriptionsByUser dictionary.", subscription.BudgetKey.ToString()));
				}
				num--;
				if (num <= 0)
				{
					ExTraceGlobals.SubscriptionsTracer.TraceDebug<string>((long)this.GetHashCode(), "[Subscriptions::DecrementSubscriptionsForUser] User '{0}' decremented to 0.  Removing reference from cache.", subscription.BudgetKey.ToString());
					this.subscriptionsByUser.Remove(subscription.BudgetKey);
				}
				else
				{
					ExTraceGlobals.SubscriptionsTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "[Subscriptions::DecrementSubscriptionsForUser] User '{0}' had {1} subscriptions.  Decrementing to {2}.", subscription.BudgetKey.ToString(), num + 1, num);
					this.subscriptionsByUser[subscription.BudgetKey] = num;
				}
			}
		}

		internal int GetSubscriptionCountForUser(BudgetKey budgetKey)
		{
			int result;
			lock (this.lockObject)
			{
				int num = 0;
				this.subscriptionsByUser.TryGetValue(budgetKey, out num);
				result = num;
			}
			return result;
		}

		private void InitializeDeleteExpiredSubscriptionsTimer()
		{
			if (this.deleteExpiredSubscriptionsTimer == null)
			{
				ExTraceGlobals.SubscriptionsTracer.TraceDebug((long)this.GetHashCode(), "InitializeDeleteExpiredSubscriptionsTimer. Initializing timer.");
				TimerCallback callback = new TimerCallback(this.DeleteExpiredSubscriptions);
				this.deleteExpiredSubscriptionsTimer = new Timer(callback, null, 300000, 300000);
			}
		}

		private SubscriptionBase Remove(string subscriptionId)
		{
			ExTraceGlobals.SubscriptionsTracer.TraceDebug<string>((long)this.GetHashCode(), "Subscriptions.Remove called. Before lock.  SubscriptionId: {0}", subscriptionId);
			SubscriptionBase result;
			lock (this.lockObject)
			{
				ExTraceGlobals.SubscriptionsTracer.TraceDebug<string>((long)this.GetHashCode(), "Subscriptions.Remove. After lock.  SubscriptionId: {0}", subscriptionId);
				SubscriptionBase subscription = this.GetSubscription(subscriptionId);
				this.subscriptions.Remove(subscriptionId);
				this.DecrementSubscriptionsForUser(subscription);
				PerformanceMonitor.UpdateActiveSubscriptionsCounter((long)this.subscriptions.Count);
				result = subscription;
			}
			return result;
		}

		internal void Delete(string subscriptionId)
		{
			try
			{
				SubscriptionBase subscriptionBase = this.Remove(subscriptionId);
				subscriptionBase.Dispose();
			}
			catch (SubscriptionNotFoundException)
			{
			}
		}

		internal SubscriptionBase Get(string subscriptionId)
		{
			ExTraceGlobals.SubscriptionsTracer.TraceDebug<string>((long)this.GetHashCode(), "Subscriptions.Get called. Before lock.  SubscriptionId: {0}", subscriptionId);
			SubscriptionBase subscription;
			lock (this.lockObject)
			{
				ExTraceGlobals.SubscriptionsTracer.TraceDebug<string>((long)this.GetHashCode(), "Subscriptions.Get. After lock.  SubscriptionId: {0}", subscriptionId);
				subscription = this.GetSubscription(subscriptionId);
			}
			return subscription;
		}

		private SubscriptionBase GetSubscription(string subscriptionId)
		{
			ExTraceGlobals.SubscriptionsTracer.TraceDebug<string>((long)this.GetHashCode(), "Subscriptions.GetSubscription called. SubscriptionId: {0}", subscriptionId);
			SubscriptionBase result;
			lock (this.lockObject)
			{
				if (this.subscriptions == null || !this.subscriptions.TryGetValue(subscriptionId, out result))
				{
					throw new SubscriptionNotFoundException();
				}
			}
			return result;
		}

		internal void DeleteExpiredSubscriptions(object state)
		{
			ExTraceGlobals.SubscriptionsTracer.TraceDebug((long)this.GetHashCode(), "Subscriptions.DeleteExpiredSubscriptions called.");
			this.RemoveExpiredSubscriptions();
		}

		internal void RemoveExpiredSubscriptions()
		{
			ExTraceGlobals.SubscriptionsTracer.TraceDebug((long)this.GetHashCode(), "Subscriptions.RemoveExpiredSubscriptions called. Before lock.");
			List<SubscriptionBase> list;
			lock (this.lockObject)
			{
				ExTraceGlobals.SubscriptionsTracer.TraceDebug((long)this.GetHashCode(), "Subscriptions.RemoveExpiredSubscriptions. After lock.");
				list = new List<SubscriptionBase>(this.subscriptions.Values);
			}
			List<SubscriptionBase> list2 = new List<SubscriptionBase>();
			if (this.isDisposed)
			{
				return;
			}
			foreach (SubscriptionBase subscriptionBase in list)
			{
				if (subscriptionBase.IsExpired)
				{
					list2.Add(subscriptionBase);
					ExTraceGlobals.SubscriptionsTracer.TraceDebug<string>((long)this.GetHashCode(), "Subscriptions.RemoveExpiredSubscriptions. Expired SubscriptionId: {0}", subscriptionBase.SubscriptionId);
				}
			}
			ExTraceGlobals.SubscriptionsTracer.TraceDebug<int>((long)this.GetHashCode(), "Subscriptions.RemoveExpiredSubscriptions. Expired count: {0}", list2.Count);
			foreach (SubscriptionBase subscriptionBase2 in list2)
			{
				this.Delete(subscriptionBase2.SubscriptionId);
			}
			PerformanceMonitor.UpdateActiveSubscriptionsCounter((long)this.subscriptions.Count);
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		private void Dispose(bool isDisposing)
		{
			ExTraceGlobals.SubscriptionsTracer.TraceDebug<bool, bool>((long)this.GetHashCode(), "Subscriptions.Dispose called. Before lock. IsDisposed: {0} IsDisposing: {1}", this.isDisposed, isDisposing);
			lock (this.lockObject)
			{
				ExTraceGlobals.SubscriptionsTracer.TraceDebug<bool>((long)this.GetHashCode(), "Subscriptions.Dispose. After lock.  IsDisposed: {0}", this.isDisposed);
				if (!this.isDisposed)
				{
					if (isDisposing)
					{
						if (this.deleteExpiredSubscriptionsTimer != null)
						{
							this.deleteExpiredSubscriptionsTimer.Dispose();
							this.deleteExpiredSubscriptionsTimer = null;
							ExTraceGlobals.SubscriptionsTracer.TraceDebug((long)this.GetHashCode(), "Subscriptions.Dispose. After timer dispose.");
						}
						if (this.subscriptions != null)
						{
							foreach (SubscriptionBase subscriptionBase in this.subscriptions.Values)
							{
								subscriptionBase.Dispose();
							}
							this.subscriptions.Clear();
							this.subscriptionsByUser.Clear();
							PerformanceMonitor.UpdateActiveSubscriptionsCounter(0L);
							this.subscriptions = null;
							this.subscriptionsByUser = null;
							ExTraceGlobals.SubscriptionsTracer.TraceDebug((long)this.GetHashCode(), "Subscriptions.Dispose. After subscriptions dispose.");
						}
					}
					this.isDisposed = true;
				}
			}
		}

		private const int DeleteExpiredSubscriptionsTimerDueTime = 300000;

		private const int DeleteExpiredSubscriptionsTimerPeriod = 300000;

		private bool isDisposed;

		private object lockObject = new object();

		private Dictionary<string, SubscriptionBase> subscriptions;

		private Dictionary<BudgetKey, int> subscriptionsByUser;

		private Timer deleteExpiredSubscriptionsTimer;
	}
}
