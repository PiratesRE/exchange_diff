using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SearchFolderQueryFailedException : SearchMailboxTaskException
	{
		public SearchFolderQueryFailedException(MailboxInfo mailbox) : base(mailbox.IsPrimary ? Strings.PrimarySearchPopulationFailed(mailbox.DisplayName, mailbox.MailboxGuid.ToString()) : Strings.ArchiveSearchPopulationFailed(mailbox.DisplayName, mailbox.MailboxGuid.ToString()))
		{
		}

		protected SearchFolderQueryFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
