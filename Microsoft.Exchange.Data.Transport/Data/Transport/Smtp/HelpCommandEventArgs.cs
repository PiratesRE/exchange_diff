using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class HelpCommandEventArgs : ReceiveCommandEventArgs
	{
		internal HelpCommandEventArgs()
		{
		}

		internal HelpCommandEventArgs(SmtpSession smtpSession, string helpArg = null) : base(smtpSession)
		{
			this.HelpArgument = helpArg;
		}

		public string HelpArgument
		{
			get
			{
				return this.helpArgument;
			}
			set
			{
				this.helpArgument = value;
			}
		}

		private string helpArgument;
	}
}
