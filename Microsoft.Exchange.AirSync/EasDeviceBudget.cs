using System;
using System.Security.Principal;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	internal class EasDeviceBudget : StandardBudget
	{
		public static IStandardBudget Acquire(EasDeviceBudgetKey budgetKey)
		{
			EasDeviceBudget innerBudget = EasDeviceBudgetCache.Singleton.Get(budgetKey);
			return new EasDeviceBudgetWrapper(innerBudget);
		}

		public static IStandardBudget Acquire(SecurityIdentifier sid, string deviceId, string deviceType, ADSessionSettings sessionSettings)
		{
			EasDeviceBudgetKey budgetKey = new EasDeviceBudgetKey(sid, deviceId, deviceType, sessionSettings);
			return EasDeviceBudget.Acquire(budgetKey);
		}

		public void AddInteractiveCall()
		{
			this.interactivecCallsPerMinute.Add(1U);
		}

		public void AddCall()
		{
			this.clientCallsPerMinute.Add(1U);
		}

		public uint InteractiveCallCount
		{
			get
			{
				return this.interactivecCallsPerMinute.GetValue();
			}
		}

		public uint CallCount
		{
			get
			{
				return this.clientCallsPerMinute.GetValue();
			}
		}

		public float Percentage
		{
			get
			{
				return this.percentage;
			}
		}

		public EasDeviceBudgetAllocator Allocator
		{
			get
			{
				return this.allocator;
			}
		}

		internal EasDeviceBudget(BudgetKey key, IThrottlingPolicy policy) : base(key, policy)
		{
			this.allocator = EasDeviceBudgetAllocator.GetAllocator((key as EasDeviceBudgetKey).Sid);
			this.allocator.Add(this);
		}

		internal void UpdatePercentage(float percentage)
		{
			if (percentage <= 0f || percentage > 1f)
			{
				throw new ArgumentOutOfRangeException("percentage", percentage, "Percentage must be > 0 and <= 1");
			}
			if (this.percentage != percentage)
			{
				this.percentage = percentage;
				base.SetPolicy(base.ThrottlingPolicy.FullPolicy, false);
			}
		}

		protected override void AfterExpire()
		{
			base.AfterExpire();
			this.allocator.Remove(this);
		}

		internal override void AfterCacheHit()
		{
			base.AfterCacheHit();
			this.allocator.UpdateIfNecessary(false);
		}

		protected override SingleComponentThrottlingPolicy GetSingleComponentPolicy(IThrottlingPolicy policy)
		{
			EasDeviceBudgetKey easDeviceBudgetKey = base.Owner as EasDeviceBudgetKey;
			return new EasDeviceThrottlingPolicy(policy, easDeviceBudgetKey.DeviceId, easDeviceBudgetKey.DeviceType, this.Percentage);
		}

		internal override bool UpdatePolicy()
		{
			bool result = base.UpdatePolicy();
			this.allocator.UpdateIfNecessary(false);
			return result;
		}

		public override string ToString()
		{
			return string.Format("Owner:{0},Allocation:{1}%,LastUpdate:{2},MaxConn:{3},Conn:{4},MaxBurst:{5},Balance:{6},Cutoff:{7},InterCallsFiveMin:{8},CallsFiveMin:{9},LiveTime:{10},ActiveDevices:{11},Policy:{12},ActiveDevicesDetails:{13}", new object[]
			{
				base.Owner,
				this.Percentage * 100f,
				this.allocator.LastUpdate,
				base.ThrottlingPolicy.MaxConcurrency,
				base.Connections,
				base.ThrottlingPolicy.MaxBurst,
				base.CasTokenBucket.GetBalance(),
				base.ThrottlingPolicy.CutoffBalance,
				this.InteractiveCallCount,
				this.CallCount,
				TimeProvider.UtcNow - base.CreationTime,
				this.allocator.Count,
				base.ThrottlingPolicy.FullPolicy.GetShortIdentityString(),
				this.allocator.GetActiveBudgetsString()
			});
		}

		private readonly FixedTimeSum interactivecCallsPerMinute = new FixedTimeSum(10000, 30);

		private readonly FixedTimeSum clientCallsPerMinute = new FixedTimeSum(10000, 30);

		private EasDeviceBudgetAllocator allocator;

		private float percentage = 1f;
	}
}
