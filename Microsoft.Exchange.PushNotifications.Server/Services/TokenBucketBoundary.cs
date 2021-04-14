using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications.Server.Services
{
	internal class TokenBucketBoundary : ITokenBucket
	{
		private TokenBucketBoundary(bool isUnlimited)
		{
			this.isUnlimited = isUnlimited;
		}

		public uint CurrentBalance
		{
			get
			{
				if (!this.isUnlimited)
				{
					return 0U;
				}
				return uint.MaxValue;
			}
		}

		public ExDateTime NextRecharge
		{
			get
			{
				return ExDateTime.MaxValue;
			}
		}

		public bool IsFull
		{
			get
			{
				return this.isUnlimited;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return !this.isUnlimited;
			}
		}

		public bool TryTakeToken()
		{
			return this.isUnlimited;
		}

		internal static readonly TokenBucketBoundary Unlimited = new TokenBucketBoundary(true);

		internal static readonly TokenBucketBoundary Empty = new TokenBucketBoundary(false);

		private readonly bool isUnlimited;
	}
}
