using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Providers;

namespace Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DisconnectDateCheck : SoftDeletedMailboxRemovalCheck
	{
		public DisconnectDateCheck(SoftDeletedRemovalData data, IDirectoryProvider directory, DateTime removalCutoffDate) : base(data, directory)
		{
			this.removalCutoffDate = removalCutoffDate;
		}

		protected override SoftDeleteMailboxRemovalCheckRemoval CheckRemoval()
		{
			if (base.Data.DisconnectDate == null)
			{
				return SoftDeleteMailboxRemovalCheckRemoval.DisallowRemoval("Cannot remove soft deleted mailbox {0} because it does not have a disconnect date.", new object[]
				{
					base.Data.MailboxIdentity
				});
			}
			if (base.Data.DisconnectDate > this.removalCutoffDate)
			{
				string reasonMessage = "Cannot remove soft deleted mailbox {0} because its DisconnectDate is {1} and the minimum date for removal is {2}.";
				return SoftDeleteMailboxRemovalCheckRemoval.DisallowRemoval(reasonMessage, new object[]
				{
					base.Data.MailboxIdentity,
					base.Data.DisconnectDate,
					this.removalCutoffDate
				});
			}
			return null;
		}

		private readonly DateTime removalCutoffDate;
	}
}
