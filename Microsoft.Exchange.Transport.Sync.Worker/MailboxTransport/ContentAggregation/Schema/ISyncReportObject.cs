using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISyncReportObject
	{
		string FolderName { get; }

		string Sender { get; }

		string Subject { get; }

		string MessageClass { get; }

		int? MessageSize { get; }

		ExDateTime? DateSent { get; }

		ExDateTime? DateReceived { get; }
	}
}
