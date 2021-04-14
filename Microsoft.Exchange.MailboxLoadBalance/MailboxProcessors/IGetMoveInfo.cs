using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors
{
	internal interface IGetMoveInfo
	{
		MoveInfo GetInfo(DirectoryMailbox mailbox, IAnchorRunspaceProxy runspace);
	}
}
