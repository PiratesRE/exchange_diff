using System;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISyncEmail : ISyncObject, IDisposeTrackable, IDisposable
	{
		ISyncSourceSession SourceSession { get; }

		bool? IsRead { get; }

		string From { get; }

		string Subject { get; }

		ExDateTime? ReceivedTime { get; }

		string MessageClass { get; }

		Importance? Importance { get; }

		string ConversationTopic { get; }

		string ConversationIndex { get; }

		Sensitivity? Sensitivity { get; }

		int? Size { get; }

		bool? HasAttachments { get; }

		bool? IsDraft { get; }

		string InternetMessageId { get; }

		Stream MimeStream { get; }

		SyncMessageResponseType? SyncMessageResponseType { get; }
	}
}
