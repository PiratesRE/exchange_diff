using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ISmtpInCommandFactory<TEvent> where TEvent : struct
	{
		ISmtpInCommand<TEvent> CreateCommand(SmtpInCommand commandType);
	}
}
