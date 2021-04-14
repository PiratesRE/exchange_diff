using System;
using System.Collections.Generic;
using Microsoft.Exchange.MailboxTransport.Delivery;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal interface IDeliveryThrottlingLogWorker
	{
		IDeliveryThrottlingLog DeliveryThrottlingLog { get; }

		void TrackMDBServerThrottle(bool isThrottle, double mdbServerThreadThreshold);

		void TrackMDBThrottle(bool isThrottle, string mdbName, double mdbResourceThreshold, List<KeyValuePair<string, double>> healthMonitorList, ThrottlingResource throttleResource);

		void TrackRecipientThrottle(bool isThrottle, string recipient, Guid orgID, string mdbName, double recipientThreadThreshold);

		void TrackConcurrentMessageSizeThrottle(bool isThrottle, ulong concurrentMessageSizeThreshold, int numOfRecipients);
	}
}
