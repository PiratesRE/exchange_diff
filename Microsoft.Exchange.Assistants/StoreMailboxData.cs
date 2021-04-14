using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Assistants
{
	internal class StoreMailboxData : MailboxData
	{
		public StoreMailboxData(Guid guid, Guid databaseGuid, string displayName, OrganizationId organizationId) : this(guid, databaseGuid, displayName, organizationId, null)
		{
		}

		public StoreMailboxData(Guid guid, Guid databaseGuid, string displayName, OrganizationId organizationId, TenantPartitionHint tenantPartitionHint) : base(guid, databaseGuid, displayName)
		{
			this.organizationId = organizationId;
			this.TenantPartitionHint = tenantPartitionHint;
		}

		public Guid Guid
		{
			get
			{
				return base.MailboxGuid;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		public bool IsPublicFolderMailbox { get; internal set; }

		public TenantPartitionHint TenantPartitionHint { get; set; }

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			StoreMailboxData storeMailboxData = other as StoreMailboxData;
			return storeMailboxData != null && this.Equals(storeMailboxData);
		}

		public bool Equals(StoreMailboxData other)
		{
			return other != null && !(base.MailboxGuid != other.MailboxGuid) && base.Equals(other);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.Guid.GetHashCode();
		}

		private readonly OrganizationId organizationId;
	}
}
