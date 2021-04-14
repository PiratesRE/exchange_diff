using System;

namespace Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors
{
	internal class MoveInfo
	{
		public MoveInfo(MoveStatus status, Guid moveRequestGuid)
		{
			this.Status = status;
			this.MoveRequestGuid = moveRequestGuid;
		}

		public MoveStatus Status { get; private set; }

		public Guid MoveRequestGuid { get; private set; }
	}
}
