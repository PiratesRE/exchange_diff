using System;

namespace Microsoft.Exchange.Rpc.MultiMailboxSearch
{
	[Serializable]
	internal sealed class MultiMailboxSearchResult : MultiMailboxSearchResultItem
	{
		internal MultiMailboxSearchResult(int version, Guid mailboxGuid, int documentId, long referenceId) : base(version)
		{
			this.mailboxGuid = mailboxGuid;
			this.documentId = documentId;
			this.referenceId = referenceId;
		}

		internal MultiMailboxSearchResult(Guid mailboxGuid, int documentId, long referenceId) : base(MultiMailboxSearchBase.CurrentVersion)
		{
			this.mailboxGuid = mailboxGuid;
			this.documentId = documentId;
			this.referenceId = referenceId;
		}

		internal Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		internal int DocumentId
		{
			get
			{
				return this.documentId;
			}
		}

		internal long ReferenceId
		{
			get
			{
				return this.referenceId;
			}
		}

		private readonly Guid mailboxGuid;

		private readonly int documentId;

		private readonly long referenceId;
	}
}
