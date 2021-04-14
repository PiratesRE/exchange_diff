using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public abstract class ReceiveEventArgs : EventArgs
	{
		internal ReceiveEventArgs()
		{
		}

		internal ReceiveEventArgs(SmtpSession smtpSession)
		{
			this.Initialize(smtpSession);
		}

		public SmtpSession SmtpSession
		{
			get
			{
				return this.smtpSession;
			}
		}

		internal void Initialize(SmtpSession session)
		{
			this.smtpSession = session;
		}

		private SmtpSession smtpSession;
	}
}
