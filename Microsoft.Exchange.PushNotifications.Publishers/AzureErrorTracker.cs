using System;
using System.Collections.Generic;
using Microsoft.Exchange.PushNotifications.Utils;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureErrorTracker : ErrorTracker<AzureErrorType>
	{
		public AzureErrorTracker(int backOffTimeInSeconds) : base(AzureErrorTracker.AzureErrorWeightTable, 60, backOffTimeInSeconds, 0)
		{
		}

		private const int MaxErrorWeight = 60;

		private static readonly Dictionary<AzureErrorType, int> AzureErrorWeightTable = new Dictionary<AzureErrorType, int>
		{
			{
				AzureErrorType.Unknown,
				30
			},
			{
				AzureErrorType.Transient,
				20
			},
			{
				AzureErrorType.Permanent,
				60
			}
		};
	}
}
