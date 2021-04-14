using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class MailboxMergeResource : MailboxResource
	{
		public MailboxMergeResource(Guid mailboxGuid) : base(mailboxGuid)
		{
		}
	}
}
