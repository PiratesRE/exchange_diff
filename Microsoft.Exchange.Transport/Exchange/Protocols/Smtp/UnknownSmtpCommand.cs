using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class UnknownSmtpCommand : SmtpCommand
	{
		public UnknownSmtpCommand(ISmtpSession session, string commandEventString, bool notSupported) : base(session, commandEventString, null, LatencyComponent.None)
		{
			this.notSupported = notSupported;
		}

		internal override void InboundParseCommand()
		{
			if (!this.notSupported)
			{
				base.SmtpResponse = SmtpResponse.UnrecognizedCommand;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				return;
			}
			if (string.Equals(base.ProtocolCommandKeyword, "vrfy", StringComparison.OrdinalIgnoreCase))
			{
				base.SmtpResponse = SmtpResponse.UnableToVrfyUser;
				base.ParsingStatus = ParsingStatus.Complete;
				return;
			}
			base.SmtpResponse = SmtpResponse.CommandNotImplemented;
			base.ParsingStatus = ParsingStatus.Complete;
		}

		internal override void InboundProcessCommand()
		{
			base.LowAuthenticationLevelTarpitOverride = TarpitAction.DoTarpit;
		}

		internal override void OutboundCreateCommand()
		{
		}

		internal override void OutboundFormatCommand()
		{
		}

		internal override void OutboundProcessResponse()
		{
		}

		private bool notSupported;
	}
}
