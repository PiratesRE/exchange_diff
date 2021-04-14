using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.MailboxTransport.Delivery;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal interface IDeliveryThrottling : IDisposable
	{
		XElement DeliveryRecipientDiagnostics { get; }

		XElement DeliveryDatabaseDiagnostics { get; }

		XElement DeliveryServerDiagnostics { get; }

		IMailboxDatabaseCollectionManager MailboxDatabaseCollectionManager { get; }

		IDeliveryThrottlingLog DeliveryThrottlingLog { get; }

		IDeliveryThrottlingLogWorker DeliveryThrottlingLogWorker { get; }

		bool CheckAndTrackThrottleServer(long smtpSessionId);

		void UpdateMdbThreadCounters();

		bool CheckAndTrackThrottleMDB(Guid databaseGuid, long smtpSessionId, out List<KeyValuePair<string, double>> mdbHealthMonitorValues);

		bool CheckAndTrackDynamicThrottleMDBPendingConnections(Guid databaseGuid, IMailboxDatabaseConnectionManager mdbConnectionManager, long smtpSessionId, IPAddress sessionRemoteEndPointAddress, out List<KeyValuePair<string, double>> mdbHealthMonitorValues);

		bool CheckAndTrackDynamicThrottleMDBTimeout(Guid databaseGuid, IMailboxDatabaseConnectionInfo mdbConnectionInfo, IMailboxDatabaseConnectionManager mdbConnectionManager, long smtpSessionId, IPAddress sessionRemoteEndPointAddress, TimeSpan connectionWaitTime, List<KeyValuePair<string, double>> mdbHealthMonitorValues);

		bool CheckAndTrackThrottleRecipient(RoutingAddress recipient, long smtpSessionId, string mdbName, Guid tenantId);

		bool CheckAndTrackThrottleConcurrentMessageSizeLimit(long smtpSessionId, int numOfRecipients);

		void SetSessionMessageSize(long messageSize, long smtpSessionId);

		bool TryGetDatabaseHealth(Guid databaseGuid, out int health);

		bool TryGetDatabaseHealth(Guid databaseGuid, out int health, out List<KeyValuePair<string, double>> monitorHealthValues);

		void ResetSession(long smtpSessionId);

		void ClearSession(long smtpSessionId);

		void DecrementRecipient(long smtpSessionId, RoutingAddress recipient);

		void DecrementCurrentMessageSize(long smtpSessionId);

		GetMDBThreadLimitAndHealth GetMDBThreadLimitAndHealth { get; }
	}
}
