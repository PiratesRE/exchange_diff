using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct MailboxCreation
	{
		private MailboxCreation(bool allowCreation, Guid? unifiedMailboxGuid, bool allowPartitionCreation)
		{
			this.allowCreation = allowCreation;
			this.unifiedMailboxGuid = unifiedMailboxGuid;
			this.allowPartitionCreation = allowPartitionCreation;
		}

		public static MailboxCreation DontAllow
		{
			get
			{
				return new MailboxCreation(false, null, false);
			}
		}

		public bool IsAllowed
		{
			get
			{
				return this.allowCreation;
			}
		}

		public Guid? UnifiedMailboxGuid
		{
			get
			{
				return this.unifiedMailboxGuid;
			}
		}

		public bool AllowPartitionCreation
		{
			get
			{
				return this.allowPartitionCreation;
			}
		}

		public static MailboxCreation Allow(Guid? unifiedMailboxGuid)
		{
			return MailboxCreation.Allow(unifiedMailboxGuid, true);
		}

		public static MailboxCreation Allow(Guid? unifiedMailboxGuid, bool allowPartitionCreation)
		{
			return new MailboxCreation(true, unifiedMailboxGuid, allowPartitionCreation);
		}

		private bool allowCreation;

		private Guid? unifiedMailboxGuid;

		private bool allowPartitionCreation;
	}
}
