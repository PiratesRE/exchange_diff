using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.OAuth
{
	internal enum PassiveMonitorType
	{
		OAuthRequestFailure,
		OAuthAcsTimeout,
		OAuthExpiredToken
	}
}
