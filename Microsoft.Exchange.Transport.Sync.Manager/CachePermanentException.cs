using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class CachePermanentException : LocalizedException
	{
		public CachePermanentException(Guid databaseGuid, Guid mailboxGuid, Exception innerException) : base(Strings.CachePermanentExceptionInfo(databaseGuid, mailboxGuid, innerException.Message), innerException)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			this.databaseGuid = databaseGuid;
			this.mailboxGuid = mailboxGuid;
		}

		protected CachePermanentException(Guid databaseGuid, Guid mailboxGuid, LocalizedString exceptionInfo, Exception innerException) : base(exceptionInfo, innerException)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			this.databaseGuid = databaseGuid;
			this.mailboxGuid = mailboxGuid;
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		private Guid databaseGuid;

		private Guid mailboxGuid;
	}
}
