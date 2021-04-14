using System;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	internal enum Pop3ResponseType
	{
		ok,
		err,
		unknown,
		sendMore
	}
}
