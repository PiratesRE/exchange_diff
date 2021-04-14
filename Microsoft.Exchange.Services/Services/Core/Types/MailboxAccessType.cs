using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal enum MailboxAccessType
	{
		Normal,
		ServerToServer,
		ExchangeImpersonation,
		ApplicationAction
	}
}
