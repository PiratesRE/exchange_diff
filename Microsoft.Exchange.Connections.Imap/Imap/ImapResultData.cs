using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Connections.Imap
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public sealed class ImapResultData
	{
		internal ImapResultData()
		{
			this.MessageUids = new List<string>(10);
			this.MessageSizes = new List<long>(10);
			this.MessageIds = new List<string>(10);
			this.MessageFlags = new List<ImapMailFlags>(10);
			this.MessageInternalDates = new List<ExDateTime?>(10);
			this.Mailboxes = new List<ImapMailbox>(10);
			this.MessageSeqNumsHashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			this.MessageSeqNums = new List<int>(10);
			this.Capabilities = new ImapServerCapabilities();
		}

		internal int? LowestSequenceNumber { get; set; }

		internal List<string> MessageUids { get; private set; }

		internal List<ExDateTime?> MessageInternalDates { get; private set; }

		internal List<long> MessageSizes { get; private set; }

		internal List<string> MessageIds { get; private set; }

		internal List<ImapMailFlags> MessageFlags { get; private set; }

		internal List<ImapMailbox> Mailboxes { get; private set; }

		internal HashSet<string> MessageSeqNumsHashSet { get; private set; }

		internal List<int> MessageSeqNums { get; private set; }

		internal ImapServerCapabilities Capabilities { get; set; }

		internal Stream MessageStream { get; set; }

		internal ImapStatus Status { get; set; }

		internal Exception FailureException { get; set; }

		internal bool UidAlreadySeen { get; set; }

		internal bool IsParseSuccessful
		{
			get
			{
				return null == this.FailureException;
			}
		}

		internal void Clear()
		{
			this.MessageUids.Clear();
			this.MessageSizes.Clear();
			this.MessageIds.Clear();
			this.MessageFlags.Clear();
			this.MessageInternalDates.Clear();
			this.Mailboxes.Clear();
			this.MessageSeqNums.Clear();
			this.MessageSeqNumsHashSet.Clear();
			this.MessageStream = null;
			this.FailureException = null;
			this.Status = ImapStatus.Unknown;
			this.LowestSequenceNumber = null;
		}

		private const int DefaultCollectionSize = 10;
	}
}
