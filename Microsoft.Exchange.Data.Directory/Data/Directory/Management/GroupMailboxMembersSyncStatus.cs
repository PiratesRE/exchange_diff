using System;
using System.Linq;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class GroupMailboxMembersSyncStatus
	{
		internal GroupMailboxMembersSyncStatus(ADObjectId[] membersInAD, ADObjectId[] membersInMailbox)
		{
			this.MembersInADOnly = membersInAD.Except(membersInMailbox).ToArray<ADObjectId>();
			this.MembersInMailboxOnly = membersInMailbox.Except(membersInAD).ToArray<ADObjectId>();
			this.IsSynced = (this.MembersInADOnly.Length == 0 && this.MembersInMailboxOnly.Length == 0);
		}

		public ADObjectId[] MembersInADOnly { get; set; }

		public ADObjectId[] MembersInMailboxOnly { get; set; }

		public bool IsSynced { get; private set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"IsSynced: ",
				this.IsSynced.ToString(),
				". MembersInADOnly: ",
				this.MembersInADOnly.Length,
				", MembersInMailboxOnly: ",
				this.MembersInMailboxOnly.Length
			});
		}
	}
}
