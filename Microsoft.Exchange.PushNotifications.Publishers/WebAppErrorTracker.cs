using System;
using System.Collections.Generic;
using Microsoft.Exchange.PushNotifications.Utils;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class WebAppErrorTracker : ErrorTracker<WebAppErrorType>
	{
		public WebAppErrorTracker(int backOffTimeInSeconds) : base(WebAppErrorTracker.ErrorWeightTable, 10, backOffTimeInSeconds, 0)
		{
		}

		private const int MaxErrorWeight = 10;

		private static readonly Dictionary<WebAppErrorType, int> ErrorWeightTable = new Dictionary<WebAppErrorType, int>
		{
			{
				WebAppErrorType.Unknown,
				1
			}
		};
	}
}
