using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.SecureMail;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal sealed class AuthParseOutput
	{
		public AuthParseOutput(SmtpAuthenticationMechanism authenticationMechanism, MultilevelAuthMechanism multilevelAuthMechanism, CommandContext initialBlob, string exchangeAuthHashAlgorithm = null)
		{
			this.AuthenticationMechanism = authenticationMechanism;
			this.MultilevelAuthMechanism = multilevelAuthMechanism;
			this.InitialBlob = initialBlob;
			this.ExchangeAuthHashAlgorithm = (string.IsNullOrEmpty(exchangeAuthHashAlgorithm) ? string.Empty : exchangeAuthHashAlgorithm);
		}

		public SmtpAuthenticationMechanism AuthenticationMechanism { get; private set; }

		public MultilevelAuthMechanism MultilevelAuthMechanism { get; private set; }

		public CommandContext InitialBlob { get; private set; }

		public string ExchangeAuthHashAlgorithm { get; private set; }
	}
}
