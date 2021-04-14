using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SearchMailboxTaskCancelledException : SearchMailboxTaskException
	{
		public SearchMailboxTaskCancelledException(MailboxInfo mailbox) : base(mailbox.IsPrimary ? Strings.SearchTaskCancelledPrimary(mailbox.DisplayName, mailbox.MailboxGuid.ToString()) : Strings.SearchTaskCancelledArchive(mailbox.DisplayName, mailbox.MailboxGuid.ToString()))
		{
		}

		protected SearchMailboxTaskCancelledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
