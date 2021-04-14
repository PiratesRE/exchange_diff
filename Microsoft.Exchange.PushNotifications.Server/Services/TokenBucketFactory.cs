using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications.Server.Services
{
	internal class TokenBucketFactory : ITokenBucketFactory
	{
		public ITokenBucket Create(Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> rechargeInterval)
		{
			return this.Create(null, maxBurst, rechargeRate, rechargeInterval);
		}

		public ITokenBucket Create(ITokenBucket template, Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> rechargeInterval)
		{
			if (maxBurst.IsUnlimited || rechargeInterval.IsUnlimited || rechargeRate.IsUnlimited)
			{
				return TokenBucketBoundary.Unlimited;
			}
			if (maxBurst.Value == 0U || rechargeRate.Value == 0U || rechargeInterval == 0U)
			{
				return TokenBucketBoundary.Empty;
			}
			uint num = maxBurst.Value;
			if (template != null)
			{
				num = Math.Min(num, template.CurrentBalance);
				new ExDateTime?(template.NextRecharge);
			}
			return new TokenBucket(maxBurst.Value, rechargeRate.Value, rechargeInterval.Value, num, null);
		}

		internal static readonly TokenBucketFactory Default = new TokenBucketFactory();
	}
}
