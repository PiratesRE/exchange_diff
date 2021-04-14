using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface IMessageHandler
	{
		MessageHandlerResult Process(CommandContext commandContext, out SmtpResponse smtpResponse);
	}
}
