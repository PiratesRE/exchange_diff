using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class AuthCommandEventArgs : ReceiveCommandEventArgs
	{
		internal AuthCommandEventArgs()
		{
		}

		internal AuthCommandEventArgs(SmtpSession smtpSession, string authenticationMechanism) : base(smtpSession)
		{
			this.AuthenticationMechanism = authenticationMechanism;
		}

		public string AuthenticationMechanism { get; internal set; }
	}
}
