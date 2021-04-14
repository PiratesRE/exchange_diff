using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IEmailNotificationHandler
	{
		void AddNotificationRecipient(IMailboxLocator recipient);
	}
}
