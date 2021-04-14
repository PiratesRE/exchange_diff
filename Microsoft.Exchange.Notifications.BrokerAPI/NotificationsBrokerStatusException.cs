using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Notifications.Broker
{
	[Serializable]
	internal class NotificationsBrokerStatusException : NotificationsBrokerPermanentException
	{
		public NotificationsBrokerStatusException(LocalizedString localizedString) : base(localizedString)
		{
		}

		public NotificationsBrokerStatusException(BrokerStatus brokerStatus) : base(NotificationsBrokerStatusException.LocalizedStringFromBrokerStatus(brokerStatus))
		{
		}

		private static LocalizedString LocalizedStringFromBrokerStatus(BrokerStatus brokerStatus)
		{
			switch (brokerStatus)
			{
			default:
				return ClientAPIStrings.BrokerStatus_UnknownError;
			case BrokerStatus.Cancelled:
				return ClientAPIStrings.BrokerStatus_Cancelled;
			}
		}
	}
}
