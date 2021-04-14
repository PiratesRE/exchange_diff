using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class MailboxNotFoundException : CacheCorruptException
	{
		public MailboxNotFoundException(Guid databaseGuid, Guid mailboxGuid, Exception innerException) : base(databaseGuid, mailboxGuid, Strings.MailboxNotFoundExceptionInfo(databaseGuid, mailboxGuid, innerException.Message), innerException)
		{
		}
	}
}
