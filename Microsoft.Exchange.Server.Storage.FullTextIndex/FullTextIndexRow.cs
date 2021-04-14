using System;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.FullTextIndex
{
	public class FullTextIndexRow
	{
		private FullTextIndexRow(long fastDocumentId)
		{
			this.fastDocumentId = fastDocumentId;
			this.documentId = IndexId.GetDocumentId(fastDocumentId);
			this.mailboxNumber = IndexId.GetMailboxNumber(fastDocumentId);
		}

		private FullTextIndexRow(long fastDocumentId, int conversationDocumentId) : this(fastDocumentId)
		{
			this.conversationDocumentId = new int?(conversationDocumentId);
		}

		private FullTextIndexRow(Guid mailboxGuid, long fastDocumentId) : this(fastDocumentId)
		{
			this.mailboxGuid = mailboxGuid;
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		[Queryable]
		public int MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		[Queryable]
		public int DocumentId
		{
			get
			{
				return this.documentId;
			}
		}

		[Queryable]
		public long FastDocumentId
		{
			get
			{
				return this.fastDocumentId;
			}
		}

		public int? ConversationDocumentId
		{
			get
			{
				return this.conversationDocumentId;
			}
		}

		public static FullTextIndexRow Parse(long fastDocumentId)
		{
			return new FullTextIndexRow(fastDocumentId);
		}

		public static FullTextIndexRow Parse(long fastDocumentId, int conversationId)
		{
			return new FullTextIndexRow(fastDocumentId, conversationId);
		}

		public static FullTextIndexRow Parse(Guid mailboxGuid, long fastDocumentId)
		{
			return new FullTextIndexRow(mailboxGuid, fastDocumentId);
		}

		private readonly Guid mailboxGuid;

		private readonly int mailboxNumber;

		private readonly int documentId;

		private readonly int? conversationDocumentId;

		private readonly long fastDocumentId;
	}
}
