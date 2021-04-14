using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SyncStoreUnhealthyException : NonPromotableTransientException
	{
		public SyncStoreUnhealthyException(Guid databaseGuid, int backoff) : base(Strings.SyncStoreUnhealthyExceptionInfo(databaseGuid, backoff))
		{
			this.backoff = backoff;
		}

		internal int Backoff
		{
			get
			{
				return this.backoff;
			}
		}

		private int backoff;
	}
}
