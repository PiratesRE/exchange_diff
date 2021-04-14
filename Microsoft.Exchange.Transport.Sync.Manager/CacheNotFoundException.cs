using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class CacheNotFoundException : CachePermanentException
	{
		public CacheNotFoundException(Guid databaseGuid, Guid mailboxGuid) : base(databaseGuid, mailboxGuid, Strings.CacheNotFoundExceptionInfo(databaseGuid, mailboxGuid), null)
		{
		}
	}
}
