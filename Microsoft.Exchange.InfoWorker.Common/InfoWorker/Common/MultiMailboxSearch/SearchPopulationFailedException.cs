using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SearchPopulationFailedException : SearchMailboxTaskException
	{
		public SearchPopulationFailedException(MailboxInfo mailbox) : base(mailbox.IsPrimary ? Strings.PrimarySearchPopulationFailed(mailbox.DisplayName, mailbox.MailboxGuid.ToString()) : Strings.ArchiveSearchPopulationFailed(mailbox.DisplayName, mailbox.MailboxGuid.ToString()))
		{
		}

		protected SearchPopulationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
