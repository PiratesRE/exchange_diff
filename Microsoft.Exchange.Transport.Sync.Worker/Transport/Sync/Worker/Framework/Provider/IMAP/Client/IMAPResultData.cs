using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class IMAPResultData
	{
		internal IMAPResultData()
		{
			this.capabilities = new List<string>(10);
			this.messageUids = new List<string>(10);
			this.messageSizes = new List<long>(10);
			this.messageIds = new List<string>(10);
			this.messageFlags = new List<IMAPMailFlags>(10);
			this.messageInternalDates = new List<ExDateTime?>(10);
			this.mailboxes = new List<IMAPMailbox>(10);
			this.messageSeqNumsHashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			this.messageSeqNums = new List<int>(10);
		}

		internal int? LowestSequenceNumber
		{
			get
			{
				return this.lowestSequenceNumber;
			}
			set
			{
				this.lowestSequenceNumber = value;
			}
		}

		internal IList<string> MessageUids
		{
			get
			{
				return this.messageUids;
			}
		}

		internal IList<ExDateTime?> MessageInternalDates
		{
			get
			{
				return this.messageInternalDates;
			}
		}

		internal IList<long> MessageSizes
		{
			get
			{
				return this.messageSizes;
			}
		}

		internal IList<string> MessageIds
		{
			get
			{
				return this.messageIds;
			}
		}

		internal IList<IMAPMailFlags> MessageFlags
		{
			get
			{
				return this.messageFlags;
			}
		}

		internal IList<IMAPMailbox> Mailboxes
		{
			get
			{
				return this.mailboxes;
			}
		}

		internal HashSet<string> MessageSeqNumsHashSet
		{
			get
			{
				return this.messageSeqNumsHashSet;
			}
		}

		internal List<int> MessageSeqNums
		{
			get
			{
				return this.messageSeqNums;
			}
		}

		internal IList<string> Capabilities
		{
			get
			{
				return this.capabilities;
			}
		}

		internal Stream MessageStream
		{
			get
			{
				return this.messageStream;
			}
			set
			{
				this.messageStream = value;
			}
		}

		internal IMAPStatus Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		internal Exception FailureException
		{
			get
			{
				return this.exception;
			}
			set
			{
				this.exception = value;
			}
		}

		internal bool IsParseSuccessful
		{
			get
			{
				return null == this.exception;
			}
		}

		internal bool UidAlreadySeen
		{
			get
			{
				return this.uidAlreadySeen;
			}
			set
			{
				this.uidAlreadySeen = value;
			}
		}

		internal void Clear()
		{
			this.messageUids.Clear();
			this.messageSizes.Clear();
			this.messageIds.Clear();
			this.messageFlags.Clear();
			this.messageInternalDates.Clear();
			this.mailboxes.Clear();
			this.messageSeqNums.Clear();
			this.messageSeqNumsHashSet.Clear();
			this.capabilities.Clear();
			this.messageStream = null;
			this.exception = null;
			this.status = IMAPStatus.Unknown;
			this.lowestSequenceNumber = null;
		}

		private const int DefaultCollectionSize = 10;

		private List<string> capabilities;

		private int? lowestSequenceNumber;

		private List<string> messageUids;

		private List<long> messageSizes;

		private List<string> messageIds;

		private HashSet<string> messageSeqNumsHashSet;

		private List<ExDateTime?> messageInternalDates;

		private List<int> messageSeqNums;

		private List<IMAPMailFlags> messageFlags;

		private bool uidAlreadySeen;

		private List<IMAPMailbox> mailboxes;

		private Stream messageStream;

		private IMAPStatus status;

		private Exception exception;
	}
}
