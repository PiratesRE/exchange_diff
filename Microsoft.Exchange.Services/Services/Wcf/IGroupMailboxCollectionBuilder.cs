using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.GroupMailbox;

namespace Microsoft.Exchange.Services.Wcf
{
	internal interface IGroupMailboxCollectionBuilder
	{
		List<GroupMailbox> BuildGroupMailboxes(string[] externalIds);
	}
}
