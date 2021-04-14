using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class CacheTransientException : TransientException
	{
		public CacheTransientException(Guid databaseGuid, Guid mailboxGuid, LocalizedString transientConditionInfo) : base(Strings.CacheTransientExceptionInfo(databaseGuid, mailboxGuid, transientConditionInfo))
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			this.databaseGuid = databaseGuid;
			this.mailboxGuid = mailboxGuid;
			this.storeRequestedBackOff = false;
		}

		public CacheTransientException(Guid databaseGuid, Guid mailboxGuid, Exception innerException) : base(Strings.CacheTransientExceptionInfo(databaseGuid, mailboxGuid, innerException.Message), innerException)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			SyncUtilities.ThrowIfArgumentNull("innerException", innerException);
			this.databaseGuid = databaseGuid;
			this.mailboxGuid = mailboxGuid;
			Exception[] array = new Exception[]
			{
				innerException,
				innerException.InnerException
			};
			foreach (Exception ex in array)
			{
				if (ex is MapiExceptionMaxObjsExceeded || ex is MapiExceptionRpcServerTooBusy)
				{
					this.storeRequestedBackOff = true;
					return;
				}
			}
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

		public bool StoreRequestedBackOff
		{
			get
			{
				return this.storeRequestedBackOff;
			}
		}

		private Guid databaseGuid;

		private Guid mailboxGuid;

		private bool storeRequestedBackOff;
	}
}
