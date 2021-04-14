using System;
using System.Net;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class MailboxDatabaseConnectionInfo : IMailboxDatabaseConnectionInfo
	{
		public MailboxDatabaseConnectionInfo(Guid mdbGuid, long smtpSessionId, IPAddress remoteIpAddress)
		{
			this.MailboxDatabaseGuid = mdbGuid;
			this.SmtpSessionId = smtpSessionId;
			this.RemoteIPAddress = remoteIpAddress;
		}

		public Guid MailboxDatabaseGuid { get; private set; }

		public IPAddress RemoteIPAddress { get; private set; }

		public long SmtpSessionId { get; private set; }
	}
}
