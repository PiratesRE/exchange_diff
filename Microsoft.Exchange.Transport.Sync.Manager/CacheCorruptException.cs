using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class CacheCorruptException : CachePermanentException
	{
		public CacheCorruptException(Guid databaseGuid, Guid mailboxGuid, Exception innerException) : this(databaseGuid, mailboxGuid, Strings.CacheCorruptExceptionInfo(databaseGuid, mailboxGuid, innerException.Message), innerException)
		{
		}

		protected CacheCorruptException(Guid databaseGuid, Guid mailboxGuid, LocalizedString exceptionInfo, Exception innerException) : base(databaseGuid, mailboxGuid, exceptionInfo, innerException)
		{
		}
	}
}
