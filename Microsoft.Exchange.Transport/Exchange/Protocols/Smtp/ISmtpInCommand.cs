using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ISmtpInCommand<TEvent> where TEvent : struct
	{
		Task<ParseAndProcessResult<TEvent>> ParseAndProcessAsync(CommandContext commandContext, CancellationToken cancellationToken);

		void LogSmtpResponse(SmtpResponse smtpResponse);
	}
}
