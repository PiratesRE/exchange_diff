using System;
using System.Collections.Generic;
using Microsoft.Exchange.PushNotifications.Utils;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureHubCreationErrorTracker : ErrorTracker<AzureHubCreationErrorType>
	{
		public AzureHubCreationErrorTracker(int baseDelayInMilliseconds, int backOffTimeInSeconds) : base(AzureHubCreationErrorTracker.AzureErrorWeightTable, 60, backOffTimeInSeconds, baseDelayInMilliseconds)
		{
		}

		private const int MaxErrorWeight = 60;

		private static readonly Dictionary<AzureHubCreationErrorType, int> AzureErrorWeightTable = new Dictionary<AzureHubCreationErrorType, int>
		{
			{
				AzureHubCreationErrorType.Unknown,
				30
			},
			{
				AzureHubCreationErrorType.Unauthorized,
				20
			},
			{
				AzureHubCreationErrorType.Permanent,
				60
			}
		};
	}
}
