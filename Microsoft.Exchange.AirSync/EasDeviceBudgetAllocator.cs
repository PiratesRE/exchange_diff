using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	internal class EasDeviceBudgetAllocator
	{
		public static EasDeviceBudgetAllocator GetAllocator(SecurityIdentifier userSid)
		{
			return EasDeviceBudgetAllocator.allocators.GetOrAdd(userSid, (SecurityIdentifier sid) => new EasDeviceBudgetAllocator(sid));
		}

		public static void Clear()
		{
			EasDeviceBudgetAllocator.allocators.Clear();
		}

		private EasDeviceBudgetAllocator(SecurityIdentifier userSid)
		{
			this.UserSid = userSid;
			this.activeBudgets = new Dictionary<EasDeviceBudgetKey, EasDeviceBudget>();
			this.lastUpdate = TimeProvider.UtcNow.Add(-EasDeviceBudgetAllocator.UpdateInterval);
		}

		public SecurityIdentifier UserSid { get; private set; }

		public DateTime LastUpdate
		{
			get
			{
				return this.lastUpdate;
			}
		}

		public int Count
		{
			get
			{
				int count;
				lock (this.instanceLock)
				{
					this.UpdateIfNecessary(false);
					count = this.activeBudgets.Count;
				}
				return count;
			}
		}

		public void Add(EasDeviceBudget budget)
		{
			EasDeviceBudgetKey easDeviceBudgetKey = budget.Owner as EasDeviceBudgetKey;
			SecurityIdentifier sid = easDeviceBudgetKey.Sid;
			if (!sid.Equals(this.UserSid))
			{
				throw new InvalidOperationException(string.Format("[EasDeviceBudgetAllocator.Add] Attempted to add a budget for user {0} to allocator for user {1}. That is very naughty.", sid, this.UserSid));
			}
			bool flag = false;
			lock (this.instanceLock)
			{
				if (!this.activeBudgets.ContainsKey(easDeviceBudgetKey))
				{
					this.activeBudgets[easDeviceBudgetKey] = budget;
					flag = true;
				}
			}
			if (flag)
			{
				this.UpdateIfNecessary(true);
			}
		}

		public string GetActiveBudgetsString()
		{
			if (this.Count > 1)
			{
				lock (this.instanceLock)
				{
					if (this.Count > 1)
					{
						bool flag2 = true;
						StringBuilder stringBuilder = new StringBuilder();
						foreach (KeyValuePair<EasDeviceBudgetKey, EasDeviceBudget> keyValuePair in this.activeBudgets)
						{
							string value = string.Concat(new object[]
							{
								flag2 ? string.Empty : ",",
								keyValuePair.Key,
								"(",
								(keyValuePair.Value.Percentage * 100f).ToString("###"),
								"%)"
							});
							stringBuilder.Append(value);
							flag2 = false;
						}
						return stringBuilder.ToString();
					}
				}
			}
			return string.Empty;
		}

		internal void Remove(EasDeviceBudget budget)
		{
			bool flag = false;
			lock (this.instanceLock)
			{
				EasDeviceBudget objB = null;
				EasDeviceBudgetKey key = budget.Owner as EasDeviceBudgetKey;
				if (this.activeBudgets.TryGetValue(key, out objB) && object.ReferenceEquals(budget, objB))
				{
					this.activeBudgets.Remove(key);
					this.UpdateIfNecessary(true);
					flag = true;
				}
				if (flag && this.activeBudgets.Count == 0)
				{
					EasDeviceBudgetAllocator easDeviceBudgetAllocator;
					EasDeviceBudgetAllocator.allocators.TryRemove(this.UserSid, out easDeviceBudgetAllocator);
				}
			}
		}

		internal void UpdateIfNecessary(bool forceUpdate)
		{
			if (this.ShouldUpdate || forceUpdate)
			{
				bool flag = false;
				try
				{
					flag = Monitor.TryEnter(this.instanceLock, 0);
					if (flag && (this.ShouldUpdate || forceUpdate))
					{
						this.Update();
					}
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(this.instanceLock);
					}
				}
			}
		}

		private void Update()
		{
			EasDeviceBudget easDeviceBudget = this.primary;
			bool flag = this.activeBudgets.Count == 1;
			foreach (KeyValuePair<EasDeviceBudgetKey, EasDeviceBudget> keyValuePair in this.activeBudgets)
			{
				EasDeviceBudget value = keyValuePair.Value;
				if (flag)
				{
					easDeviceBudget = value;
					break;
				}
				if (value != easDeviceBudget)
				{
					if (easDeviceBudget == null)
					{
						easDeviceBudget = value;
					}
					else if (value.InteractiveCallCount > easDeviceBudget.InteractiveCallCount || (value.InteractiveCallCount == easDeviceBudget.InteractiveCallCount && value.CallCount > easDeviceBudget.CallCount))
					{
						easDeviceBudget = value;
					}
				}
			}
			if (this.primary != easDeviceBudget)
			{
				this.primary = easDeviceBudget;
			}
			int num = this.activeBudgets.Count + 1;
			float num2 = 1f / (float)num;
			foreach (KeyValuePair<EasDeviceBudgetKey, EasDeviceBudget> keyValuePair2 in this.activeBudgets)
			{
				keyValuePair2.Value.UpdatePercentage((this.primary == keyValuePair2.Value) ? (num2 * 2f) : num2);
			}
			this.lastUpdate = TimeProvider.UtcNow;
		}

		private bool ShouldUpdate
		{
			get
			{
				return TimeProvider.UtcNow - this.lastUpdate >= EasDeviceBudgetAllocator.UpdateInterval;
			}
		}

		private static ConcurrentDictionary<SecurityIdentifier, EasDeviceBudgetAllocator> allocators = new ConcurrentDictionary<SecurityIdentifier, EasDeviceBudgetAllocator>();

		internal static readonly TimeSpan UpdateInterval = TimeSpan.FromMinutes(1.0);

		private object instanceLock = new object();

		private DateTime lastUpdate;

		private EasDeviceBudget primary;

		private Dictionary<EasDeviceBudgetKey, EasDeviceBudget> activeBudgets;
	}
}
