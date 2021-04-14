using System;
using System.Collections.Generic;
using Microsoft.Exchange.PushNotifications.Utils;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ProxyErrorTracker : ErrorTracker<ProxyErrorType>
	{
		public ProxyErrorTracker(int maxErrorWeight, int backOffTimeInSeconds, int baseDelayInMilliseconds) : base(ProxyErrorTracker.ProxyErrorWeightTable, maxErrorWeight * 20, backOffTimeInSeconds, baseDelayInMilliseconds)
		{
		}

		private const int UnknownErrorWeight = 30;

		private const int TransientErrorWeight = 20;

		private const int PermanentErrorWeight = 60;

		private static readonly Dictionary<ProxyErrorType, int> ProxyErrorWeightTable = new Dictionary<ProxyErrorType, int>
		{
			{
				ProxyErrorType.Unknown,
				30
			},
			{
				ProxyErrorType.Transient,
				20
			},
			{
				ProxyErrorType.Permanent,
				60
			}
		};
	}
}
