using System;
using System.Security;
using System.Security.Principal;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	internal class ProcessAuthenticationEventArgs : ReceiveEventArgs
	{
		public ProcessAuthenticationEventArgs(SmtpSession smtpSession, byte[] userName, SecureString password) : base(smtpSession)
		{
			this.UserName = userName;
			this.Password = password;
		}

		public byte[] UserName { get; private set; }

		public SecureString Password { get; private set; }

		public WindowsIdentity Identity { get; set; }

		public object AuthResult { get; set; }

		public string AuthErrorDetails { get; set; }
	}
}
