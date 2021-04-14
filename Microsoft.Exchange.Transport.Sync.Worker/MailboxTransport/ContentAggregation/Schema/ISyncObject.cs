using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISyncObject : IDisposeTrackable, IDisposable
	{
		SchemaType Type { get; }

		ExDateTime? LastModifiedTime { get; }
	}
}
