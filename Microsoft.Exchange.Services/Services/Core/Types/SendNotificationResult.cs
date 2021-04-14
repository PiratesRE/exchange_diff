using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("SendNotificationResultType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class SendNotificationResult
	{
		public SendNotificationResult()
		{
			this.subscriptionStatusField = SubscriptionStatus.Invalid;
		}

		[XmlElement("SubscriptionStatus", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public SubscriptionStatus SubscriptionStatus
		{
			get
			{
				return this.subscriptionStatusField;
			}
			set
			{
				this.subscriptionStatusField = value;
			}
		}

		private SubscriptionStatus subscriptionStatusField;
	}
}
