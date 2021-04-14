using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;

namespace Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MoveHistoryCheck : SoftDeletedMailboxRemovalCheck
	{
		public MoveHistoryCheck(SoftDeletedRemovalData data, LoadBalanceAnchorContext context) : base(data, context.Directory)
		{
			this.context = context;
		}

		protected override SoftDeleteMailboxRemovalCheckRemoval CheckRemoval()
		{
			SoftDeletedMoveHistory softDeletedMoveHistory = this.context.RetrieveSoftDeletedMailboxMoveHistory(base.Data.MailboxIdentity.Guid, base.Data.TargetDatabase.Guid, base.Data.SourceDatabase.Guid);
			if (softDeletedMoveHistory == null)
			{
				string reasonMessage = "Cannot remove soft deleted mailbox {0} because its Movehistory for source database '{1}' and target database '{2}' could not be found";
				return SoftDeleteMailboxRemovalCheckRemoval.DisallowRemoval(reasonMessage, new object[]
				{
					base.Data.MailboxIdentity.Guid,
					base.Data.SourceDatabase.Name,
					base.Data.TargetDatabase.Name
				});
			}
			if (softDeletedMoveHistory.BadItemsEncountered > 0 || softDeletedMoveHistory.LargeItemsEncountered > 0 || softDeletedMoveHistory.MissingItemsEncountered > 0)
			{
				string reasonMessage2 = "Cannot remove soft deleted mailbox {0} because its MoveHistory from '{1}' to '{2}' has BadItemCount: {3} LargeItemCount: {4} MissingItemCount: {5}.";
				return SoftDeleteMailboxRemovalCheckRemoval.DisallowRemoval(reasonMessage2, new object[]
				{
					base.Data.MailboxIdentity.Guid,
					base.Data.SourceDatabase.Name,
					base.Data.TargetDatabase.Name,
					softDeletedMoveHistory.BadItemsEncountered,
					softDeletedMoveHistory.LargeItemsEncountered,
					softDeletedMoveHistory.MissingItemsEncountered
				});
			}
			return null;
		}

		private readonly LoadBalanceAnchorContext context;
	}
}
