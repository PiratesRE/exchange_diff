using System;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class CompletedTasks
	{
		public static readonly Task<SmtpResponse> SmtpResponseEmpty = Task.FromResult<SmtpResponse>(SmtpResponse.Empty);

		public static readonly Task<SmtpResponse> SmtpResponseUnrecognizedCommand = Task.FromResult<SmtpResponse>(SmtpResponse.UnrecognizedCommand);

		public static readonly Task<SmtpResponse> SmtpResponseBadCommandSequence = Task.FromResult<SmtpResponse>(SmtpResponse.BadCommandSequence);
	}
}
