using System;

namespace Microsoft.Exchange.Data.Storage
{
	public enum FlagStatus
	{
		[LocDescription(ServerStrings.IDs.InboxRuleFlagStatusNotFlagged)]
		NotFlagged,
		[LocDescription(ServerStrings.IDs.InboxRuleFlagStatusComplete)]
		Complete,
		[LocDescription(ServerStrings.IDs.InboxRuleFlagStatusFlagged)]
		Flagged
	}
}
