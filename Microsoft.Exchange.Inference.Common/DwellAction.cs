using System;

namespace Microsoft.Exchange.Inference.Common
{
	public enum DwellAction
	{
		MarkedAsClutter,
		MarkedAsNotClutter,
		DweltOn,
		MarkedAsRead,
		MarkedAsUnread,
		DweltOnInClutter,
		MarkedAsReadInClutter,
		RepliedTo,
		Forwarded,
		Flagged,
		Deleted,
		MoveFromInbox,
		MoveToInbox,
		MoveFromClutter,
		MoveToClutter,
		DeleteFromInbox,
		DeleteFromClutter
	}
}
