using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureSendRequest : AzureRequestBase
	{
		public AzureSendRequest(AzureNotification notification, AzureSasToken sasToken, string resourceUri, string azureTag = null) : base("application/json;charset=utf-8", "POST", sasToken, resourceUri, "")
		{
			ArgumentValidator.ThrowIfNull("notification", notification);
			base.Headers["ServiceBusNotification-Format"] = "template";
			base.Headers["TrackingId"] = notification.Identifier;
			this.Target = notification.RecipientId;
			base.RequestBody = notification.SerializedPaylod;
		}

		public string Target
		{
			get
			{
				return base.Headers["ServiceBusNotification-Tags"];
			}
			private set
			{
				base.Headers["ServiceBusNotification-Tags"] = value;
			}
		}

		public const string SendNotificationContentType = "application/json;charset=utf-8";

		public const string HeaderFormatName = "ServiceBusNotification-Format";

		public const string HeaderFormat = "template";

		public const string HeaderTagName = "ServiceBusNotification-Tags";

		public const string HeaderNotificationTrackingId = "TrackingId";
	}
}
