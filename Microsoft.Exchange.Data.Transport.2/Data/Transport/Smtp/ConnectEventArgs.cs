using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class ConnectEventArgs : ReceiveEventArgs
	{
		public SmtpResponse Banner
		{
			get
			{
				return base.SmtpSession.Banner;
			}
			set
			{
				if (value.Equals(SmtpResponse.Empty))
				{
					throw new ArgumentException("value of banner cannot be SmtpResponse.Emtpy", "value");
				}
				base.SmtpSession.Banner = value;
			}
		}

		public bool DisableStartTls
		{
			get
			{
				return base.SmtpSession.DisableStartTls;
			}
			set
			{
				base.SmtpSession.DisableStartTls = value;
			}
		}

		internal ConnectEventArgs(SmtpSession smtpSession) : base(smtpSession)
		{
		}
	}
}
