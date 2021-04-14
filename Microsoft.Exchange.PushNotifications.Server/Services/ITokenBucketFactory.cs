using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PushNotifications.Server.Services
{
	internal interface ITokenBucketFactory
	{
		ITokenBucket Create(Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> rechargeInterval);

		ITokenBucket Create(ITokenBucket template, Unlimited<uint> maxBurst, Unlimited<uint> rechargeRate, Unlimited<uint> rechargeInterval);
	}
}
