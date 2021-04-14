using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class EndOfAuthenticationEventArgs : ReceiveEventArgs
	{
		internal EndOfAuthenticationEventArgs(SmtpSession smtpSession, string authenticationMechanism, string remoteIdentityName) : base(smtpSession)
		{
			this.AuthenticationMechanism = authenticationMechanism;
			this.RemoteIdentityName = remoteIdentityName;
		}

		public string AuthenticationMechanism { get; internal set; }

		public string RemoteIdentityName { get; internal set; }
	}
}
