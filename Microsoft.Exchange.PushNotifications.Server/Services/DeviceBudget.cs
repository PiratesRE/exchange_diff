using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Server.Services
{
	internal class DeviceBudget : Budget
	{
		internal DeviceBudget(BudgetKey owner, IThrottlingPolicy policy) : this(owner, policy, TokenBucketFactory.Default)
		{
		}

		internal DeviceBudget(BudgetKey owner, IThrottlingPolicy policy, ITokenBucketFactory tokenBucketFactory) : base(owner, policy)
		{
			ArgumentValidator.ThrowIfNull("tokenBucketFactory", tokenBucketFactory);
			this.tokenBucketFactory = tokenBucketFactory;
			this.UpdateCachedPolicyValues(true);
		}

		protected override bool InternalCanExpire
		{
			get
			{
				bool result;
				lock (this.syncRoot)
				{
					result = (this.sentNotificationsBucket.IsFull && this.invalidNotificationsBucket.IsFull);
				}
				return result;
			}
		}

		public override string ToString()
		{
			string result;
			lock (this.syncRoot)
			{
				result = string.Format("{{owner:{0}; policy:{1}; sentNotifications:{2}; invalidNotifications:{3}}}", new object[]
				{
					base.Owner.ToString(),
					base.ThrottlingPolicy.FullPolicy.GetIdentityString(),
					this.sentNotificationsBucket.ToString(),
					this.invalidNotificationsBucket.ToString()
				});
			}
			return result;
		}

		internal static IDeviceBudget Acquire(DeviceBudgetKey budgetKey)
		{
			return new DeviceBudgetWrapper(DeviceBudget.Get(budgetKey));
		}

		internal static DeviceBudget Get(DeviceBudgetKey budgetKey)
		{
			ArgumentValidator.ThrowIfNull("budgetKey", budgetKey);
			return DeviceBudget.DeviceBudgetCache.Singleton.Get(budgetKey);
		}

		internal bool TryApproveSendNotification(out OverBudgetException obe)
		{
			return this.TryTakeToken(this.sentNotificationsBucket, out obe);
		}

		internal bool TryApproveInvalidNotification(out OverBudgetException obe)
		{
			return this.TryTakeToken(this.invalidNotificationsBucket, out obe);
		}

		protected override void UpdateCachedPolicyValues(bool resetBudgetValues)
		{
			if (this.tokenBucketFactory == null)
			{
				return;
			}
			lock (this.syncRoot)
			{
				IThrottlingPolicy fullPolicy = base.ThrottlingPolicy.FullPolicy;
				this.sentNotificationsBucket = this.tokenBucketFactory.Create(resetBudgetValues ? null : this.sentNotificationsBucket, fullPolicy.PushNotificationMaxBurstPerDevice, fullPolicy.PushNotificationRechargeRatePerDevice, fullPolicy.PushNotificationSamplingPeriodPerDevice);
				if (this.invalidNotificationsBucket == null || resetBudgetValues)
				{
					this.invalidNotificationsBucket = this.tokenBucketFactory.Create(1U, 1U, 86400000U);
				}
			}
			base.UpdateCachedPolicyValues(resetBudgetValues);
		}

		protected override bool InternalTryCheckOverBudget(ICollection<CostType> costTypes, out OverBudgetException exception)
		{
			bool result;
			lock (this.syncRoot)
			{
				exception = null;
				if (this.invalidNotificationsBucket.IsEmpty)
				{
					exception = base.CreateOverBudgetException("PushNotificationInvalidNotificationMaxBurst", 1.ToString(), 86400000);
				}
				if (this.sentNotificationsBucket.IsEmpty)
				{
					exception = base.CreateOverBudgetException("PushNotificationMaxBurstPerDevice", base.ThrottlingPolicy.FullPolicy.PushNotificationMaxBurstPerDevice.Value.ToString(), (int)base.ThrottlingPolicy.FullPolicy.PushNotificationSamplingPeriodPerDevice.Value);
				}
				result = (exception != null);
			}
			return result;
		}

		private bool TryTakeToken(ITokenBucket tokenBucket, out OverBudgetException obe)
		{
			bool result;
			lock (this.syncRoot)
			{
				bool flag2 = !base.TryCheckOverBudget(out obe);
				if (flag2)
				{
					tokenBucket.TryTakeToken();
				}
				result = flag2;
			}
			return result;
		}

		private const int InvalidNotificationMaxBurst = 1;

		private const int InvalidNotificationRechargeInterval = 86400000;

		private ITokenBucketFactory tokenBucketFactory;

		private ITokenBucket sentNotificationsBucket;

		private ITokenBucket invalidNotificationsBucket;

		private object syncRoot = new object();

		private class DeviceBudgetCache : BudgetCache<DeviceBudget>
		{
			protected override DeviceBudget CreateBudget(BudgetKey key, IThrottlingPolicy policy)
			{
				return new DeviceBudget(key, policy, TokenBucketFactory.Default);
			}

			public static readonly DeviceBudget.DeviceBudgetCache Singleton = new DeviceBudget.DeviceBudgetCache();
		}
	}
}
