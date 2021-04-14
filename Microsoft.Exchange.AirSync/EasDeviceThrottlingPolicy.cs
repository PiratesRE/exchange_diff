using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	internal class EasDeviceThrottlingPolicy : SingleComponentThrottlingPolicy
	{
		public float Percentage { get; private set; }

		public string DeviceId { get; private set; }

		public string DeviceType { get; private set; }

		public EasDeviceThrottlingPolicy(IThrottlingPolicy innerPolicy, string deviceId, string deviceType, float percentage) : base(BudgetType.Eas, innerPolicy)
		{
			ArgumentValidator.ThrowIfNull("innerPolicy", innerPolicy);
			ArgumentValidator.ThrowIfNullOrEmpty("deviceId", deviceId);
			ArgumentValidator.ThrowIfNullOrEmpty("deviceType", deviceType);
			if (percentage <= 0f || percentage > 1f)
			{
				throw new ArgumentOutOfRangeException("percentage", percentage, "Percentage must be > 0 and <= 1");
			}
			this.DeviceId = deviceId;
			this.DeviceType = deviceType;
			this.Percentage = percentage;
			this.cachedIdentity = string.Format("{0}-{1}-{2}-{3}-{4}", new object[]
			{
				innerPolicy.GetShortIdentityString(),
				this.DeviceId,
				this.DeviceType,
				this.Percentage,
				TimeProvider.UtcNow
			});
			this.cutoffBalance = this.GetFactoredValue(innerPolicy.EasCutoffBalance);
			this.maxBurst = this.GetFactoredValue(innerPolicy.EasMaxBurst);
			this.rechargeRate = innerPolicy.EasRechargeRate;
			this.maxConcurrency = innerPolicy.EasMaxConcurrency;
		}

		public override Unlimited<uint> CutoffBalance
		{
			get
			{
				return this.cutoffBalance;
			}
		}

		public override Unlimited<uint> MaxBurst
		{
			get
			{
				return this.maxBurst;
			}
		}

		public override Unlimited<uint> MaxConcurrency
		{
			get
			{
				return this.maxConcurrency;
			}
		}

		public override Unlimited<uint> RechargeRate
		{
			get
			{
				return this.rechargeRate;
			}
		}

		private Unlimited<uint> GetFactoredValue(Unlimited<uint> fullValue)
		{
			if (fullValue.IsUnlimited)
			{
				return fullValue;
			}
			return (uint)Math.Ceiling((double)(fullValue.Value * this.Percentage));
		}

		private const string IdentityFormat = "{0}-{1}-{2}-{3}-{4}";

		private readonly string cachedIdentity;

		private readonly Unlimited<uint> cutoffBalance;

		private readonly Unlimited<uint> rechargeRate;

		private readonly Unlimited<uint> maxBurst;

		private readonly Unlimited<uint> maxConcurrency;
	}
}
