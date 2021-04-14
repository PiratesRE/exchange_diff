using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NonExistentMailboxGuidException : StoragePermanentException
	{
		internal NonExistentMailboxGuidException(Guid mailboxGuid) : base(LocalizedString.Empty)
		{
			this.MailboxGuid = mailboxGuid;
		}

		private NonExistentMailboxGuidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public Guid MailboxGuid { get; private set; }
	}
}
