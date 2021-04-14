using System;
using System.Net;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal interface IMailboxDatabaseConnectionInfo
	{
		Guid MailboxDatabaseGuid { get; }

		long SmtpSessionId { get; }

		IPAddress RemoteIPAddress { get; }
	}
}
