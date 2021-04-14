using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public enum RequestDirection
	{
		[LocDescription(MrsStrings.IDs.MoveRequestDirectionPull)]
		Pull = 1,
		[LocDescription(MrsStrings.IDs.MoveRequestDirectionPush)]
		Push
	}
}
