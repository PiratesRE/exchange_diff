using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SearchMailboxTaskTimeoutException : SearchMailboxTaskException
	{
		public SearchMailboxTaskTimeoutException(MailboxInfo mailbox) : base(mailbox.IsPrimary ? Strings.SearchTaskTimeoutPrimary(mailbox.DisplayName, mailbox.MailboxGuid.ToString()) : Strings.SearchTaskTimeoutArchive(mailbox.DisplayName, mailbox.MailboxGuid.ToString()))
		{
		}

		protected SearchMailboxTaskTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
