using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TeamMailboxSyncId : IEquatable<TeamMailboxSyncId>, IComparable<TeamMailboxSyncId>, IComparable
	{
		public override int GetHashCode()
		{
			return this.MailboxGuid.GetHashCode();
		}

		public bool Equals(TeamMailboxSyncId other)
		{
			return other != null && this.MailboxGuid == other.MailboxGuid;
		}

		public int CompareTo(object other)
		{
			TeamMailboxSyncId teamMailboxSyncId = other as TeamMailboxSyncId;
			if (teamMailboxSyncId != null)
			{
				return this.MailboxGuid.CompareTo(teamMailboxSyncId.MailboxGuid);
			}
			return 1;
		}

		public int CompareTo(TeamMailboxSyncId other)
		{
			return this.MailboxGuid.CompareTo(other.MailboxGuid);
		}

		public Guid MailboxGuid { get; private set; }

		public OrganizationId OrgId { get; private set; }

		public string DomainController { get; private set; }

		public TeamMailboxSyncId(Guid mailboxGuid, OrganizationId orgId, string domainController)
		{
			this.MailboxGuid = mailboxGuid;
			this.OrgId = orgId;
			this.DomainController = domainController;
		}
	}
}
