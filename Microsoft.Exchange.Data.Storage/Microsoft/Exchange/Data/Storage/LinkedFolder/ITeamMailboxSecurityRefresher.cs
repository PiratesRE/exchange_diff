using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ITeamMailboxSecurityRefresher
	{
		void Refresh(ADUser mailbox, IRecipientSession session);
	}
}
