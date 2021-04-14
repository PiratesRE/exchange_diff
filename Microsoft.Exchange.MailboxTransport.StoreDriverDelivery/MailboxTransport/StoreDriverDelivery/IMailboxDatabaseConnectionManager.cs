using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal interface IMailboxDatabaseConnectionManager : IDisposable
	{
		bool AddConnection(long smtpSessionId, IPAddress remoteIPAddress);

		bool GetMdbHealthAndAddConnection(long smtpSessionId, IPAddress remoteIPAddress, out int mdbHealthMeasure, out List<KeyValuePair<string, double>> healthMonitorList, out int currentConnectionLimit);

		bool RemoveConnection(long smtpSessionId, IPAddress remoteIPAddress);

		IMailboxDatabaseConnectionInfo Acquire(long smtpSessionId, IPAddress remoteIPAddress, TimeSpan timeout);

		bool TryAcquire(long smtpSessionId, IPAddress remoteIPAddress, TimeSpan timeout, out IMailboxDatabaseConnectionInfo mdbConnectionInfo);

		bool Release(ref IMailboxDatabaseConnectionInfo mailboxDatabaseConnectionInfo);

		void UpdateLastActivityTime(long smtpSessionId);
	}
}
