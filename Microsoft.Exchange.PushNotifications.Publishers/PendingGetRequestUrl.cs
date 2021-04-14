using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	public class PendingGetRequestUrl
	{
		public PendingGetRequestUrl(string baseUrl, int timeout, string subscriptionId, string unseenNotificationId)
		{
			this.Timeout = timeout;
			this.SubscriptionId = subscriptionId;
			this.UnseenNotificationId = unseenNotificationId;
			this.BaseUrl = baseUrl;
		}

		public string BaseUrl { get; private set; }

		public int Timeout { get; private set; }

		public string SubscriptionId { get; private set; }

		public string UnseenNotificationId { get; private set; }

		public string Serialize()
		{
			Uri uri = new Uri(new Uri(this.BaseUrl), new Uri(string.Format("{0}/{1}?{2}", "PushNotifications", "mowapendingget.ashx", this.SerializedParameters()), UriKind.Relative));
			return uri.AbsoluteUri;
		}

		public override string ToString()
		{
			return this.Serialize();
		}

		private string SerializedParameters()
		{
			return string.Format("T={0}&S={1}&US={2}", this.Timeout, this.SubscriptionId, this.UnseenNotificationId);
		}

		public const string VirtualDirectory = "PushNotifications";

		public const string Page = "mowapendingget.ashx";
	}
}
