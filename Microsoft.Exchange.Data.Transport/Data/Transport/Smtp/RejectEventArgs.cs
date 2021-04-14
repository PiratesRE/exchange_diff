using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class RejectEventArgs : ReceiveEventArgs
	{
		internal byte[] RawCommand
		{
			set
			{
				this.command = value;
			}
		}

		public string Command
		{
			get
			{
				if (this.commandString == null)
				{
					this.commandString = CTSGlobals.AsciiEncoding.GetString(this.command);
				}
				return this.commandString;
			}
		}

		public EventArgs OriginalArguments
		{
			get
			{
				return this.originalArguments;
			}
			internal set
			{
				this.originalArguments = value;
			}
		}

		public ParsingStatus ParsingStatus
		{
			get
			{
				return this.parsingStatus;
			}
			internal set
			{
				this.parsingStatus = value;
			}
		}

		public SmtpResponse SmtpResponse
		{
			get
			{
				return this.smtpResponse;
			}
			internal set
			{
				this.smtpResponse = value;
			}
		}

		internal RejectEventArgs(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		private byte[] command;

		private EventArgs originalArguments;

		private ParsingStatus parsingStatus;

		private SmtpResponse smtpResponse;

		private string commandString;
	}
}
