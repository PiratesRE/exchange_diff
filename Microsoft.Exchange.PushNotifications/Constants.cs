using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications
{
	public class Constants
	{
		public const uint DefaultSubscriptionExpirationInHours = 72U;

		public const string PushNotificationNamespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf";

		public const string PushNotificationsServiceRelativeUri = "PushNotifications/service.svc";

		public const string NamedPipeUri = "net.pipe://localhost/PushNotifications/service.svc";

		public const string PushNotificationsAppPoolId = "MSExchangePushNotificationsAppPool";

		public const string OnPremPublishNotificationsUriTemplate = "PublishOnPremNotifications";

		public const string OnPremPublishNotificationsRelativeUri = "PushNotifications/service.svc/PublishOnPremNotifications";

		public const string GetAppConfigDataAddress = "AppConfig";

		public const string GetAppConfigDataUriTemplate = "GetAppConfigData";

		public const string GetAppConfigDataRelativeUri = "PushNotifications/service.svc/AppConfig/GetAppConfigData";

		public const string RegistryRootPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange PushNotifications";

		public static readonly ExDateTime EpochBaseTime = new ExDateTime(ExTimeZone.UtcTimeZone, 1970, 1, 1);
	}
}
