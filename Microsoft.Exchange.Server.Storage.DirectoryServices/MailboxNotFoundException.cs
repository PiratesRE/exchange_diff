using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.DirectoryServices
{
	public class MailboxNotFoundException : StoreException
	{
		public MailboxNotFoundException(LID lid, Guid mailboxGuid) : base(lid, ErrorCodeValue.NotFound, string.Format("Mailbox not found: {0}", mailboxGuid))
		{
		}

		public MailboxNotFoundException(LID lid, string mailboxId) : base(lid, ErrorCodeValue.NotFound, string.Format("Mailbox not found: {0}", mailboxId))
		{
		}

		public MailboxNotFoundException(LID lid, Guid mailboxGuid, Exception innerException) : base(lid, ErrorCodeValue.NotFound, string.Format("Mailbox not found: {0}", mailboxGuid), innerException)
		{
		}

		public MailboxNotFoundException(LID lid, string mailboxId, Exception innerException) : base(lid, ErrorCodeValue.NotFound, string.Format("Mailbox not found: {0}", mailboxId), innerException)
		{
		}

		private const string MailboxNotFoundTemplate = "Mailbox not found: {0}";
	}
}
