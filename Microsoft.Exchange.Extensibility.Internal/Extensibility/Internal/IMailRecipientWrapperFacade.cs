using System;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal interface IMailRecipientWrapperFacade
	{
		IMailRecipientFacade MailRecipient { get; }
	}
}
