using System;

namespace Microsoft.Exchange.Assistants
{
	internal interface IMailboxFilter
	{
		MailboxType MailboxType { get; }
	}
}
