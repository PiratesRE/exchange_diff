using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.PushNotifications.Utils;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class GcmErrorTracker : ErrorTracker<GcmErrorType>
	{
		public GcmErrorTracker(int backOffTimeInSeconds) : base(GcmErrorTracker.ErrorWeightTable, 100, backOffTimeInSeconds, 0)
		{
		}

		public override ExDateTime BackOffEndTime
		{
			get
			{
				if (!(this.RetryAfter > base.BackOffEndTime))
				{
					return base.BackOffEndTime;
				}
				return this.RetryAfter;
			}
		}

		private ExDateTime RetryAfter { get; set; }

		public virtual void SetRetryAfter(ExDateTime retryAfter)
		{
			this.RetryAfter = retryAfter;
		}

		private const int MaxErrorWeight = 100;

		private static readonly Dictionary<GcmErrorType, int> ErrorWeightTable = new Dictionary<GcmErrorType, int>
		{
			{
				GcmErrorType.Unknown,
				1
			},
			{
				GcmErrorType.Transport,
				40
			}
		};
	}
}
