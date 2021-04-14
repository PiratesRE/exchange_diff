using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class MailboxLockName : MailboxLockNameBase
	{
		public MailboxLockName(Guid databaseGuid, int mailboxPartitionNumber) : base(databaseGuid, mailboxPartitionNumber)
		{
			this.databaseGuid = databaseGuid;
		}

		public override Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public override ILockName GetLockNameToCache()
		{
			return this;
		}

		public override string GetFriendlyNameForLogging()
		{
			return string.Empty;
		}

		private readonly Guid databaseGuid;
	}
}
