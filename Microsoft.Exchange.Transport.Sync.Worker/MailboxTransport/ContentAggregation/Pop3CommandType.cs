using System;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	internal enum Pop3CommandType
	{
		Quit,
		Stat,
		List,
		Retr,
		Dele,
		Noop,
		Rset,
		Top,
		Uidl,
		User,
		Pass,
		Auth,
		Blob,
		Capa,
		Stls
	}
}
