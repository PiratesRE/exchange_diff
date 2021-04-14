using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class BadItemData
	{
		internal BadItemData(Guid requestGuid, BadMessageRec badMessage)
		{
			this.RequestGuid = requestGuid;
			this.badMessage = badMessage;
			this.CallStackHash = ((badMessage.RawFailure != null) ? CommonUtils.ComputeCallStackHash(badMessage.RawFailure, 5) : string.Empty);
		}

		public Guid RequestGuid { get; private set; }

		public BadItemKind Kind
		{
			get
			{
				return this.badMessage.Kind;
			}
		}

		public byte[] EntryId
		{
			get
			{
				return this.badMessage.EntryId;
			}
		}

		public byte[] FolderId
		{
			get
			{
				return this.badMessage.FolderId;
			}
		}

		public string FolderName
		{
			get
			{
				return this.badMessage.FolderName;
			}
		}

		public WellKnownFolderType WKFType
		{
			get
			{
				return this.badMessage.WellKnownFolderType;
			}
		}

		public string MessageClass
		{
			get
			{
				return this.badMessage.MessageClass;
			}
		}

		public int? MessageSize
		{
			get
			{
				return this.badMessage.MessageSize;
			}
		}

		public DateTime? DateSent
		{
			get
			{
				return this.badMessage.DateSent;
			}
		}

		public DateTime? DateReceived
		{
			get
			{
				return this.badMessage.DateReceived;
			}
		}

		public Exception FailureMessage
		{
			get
			{
				return this.badMessage.RawFailure;
			}
		}

		public string Category
		{
			get
			{
				return this.badMessage.Category;
			}
		}

		public string CallStackHash { get; private set; }

		private BadMessageRec badMessage;
	}
}
