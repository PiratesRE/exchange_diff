using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal interface IActionsSource
	{
		IEnumerable<ReplayAction> ReadActions(IActionWatermark watermark);
	}
}
