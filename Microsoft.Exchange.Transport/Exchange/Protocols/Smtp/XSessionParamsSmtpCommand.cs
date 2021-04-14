using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class XSessionParamsSmtpCommand : SmtpCommand
	{
		public XSessionParamsSmtpCommand(ISmtpSession session) : base(session, "XSESSIONPARAMS", "OnXSessionParamsCommand", LatencyComponent.None)
		{
		}

		internal override void InboundParseCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XSessionParamsInboundParseCommand);
			if (!base.VerifyEhloReceived() || !base.VerifyNoOngoingMailTransaction())
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.WrongSequence);
				return;
			}
			XSessionParams xsessionParams;
			ParseResult parseResult = XSessionParamsSmtpCommandParser.Parse(CommandContext.FromSmtpCommand(this), SmtpInSessionState.FromSmtpInSession(smtpInSession), out xsessionParams);
			if (parseResult.IsFailed)
			{
				if (parseResult.SmtpResponse == SmtpResponse.NotAuthorized)
				{
					smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XSessionParamsNotAuthorized);
				}
				else if (parseResult.SmtpResponse == SmtpResponse.CommandNotImplemented)
				{
					smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XSessionParamsNotEnabled);
				}
			}
			else
			{
				this.CommandEventArgs = new XSessionParamsCommandEventArgs(smtpInSession.SessionSource, xsessionParams.MdbGuid, xsessionParams.Type);
			}
			base.SmtpResponse = parseResult.SmtpResponse;
			base.ParsingStatus = parseResult.ParsingStatus;
		}

		internal override void InboundProcessCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XSessionParamsInboundProcessCommand);
			if (base.ParsingStatus == ParsingStatus.Complete)
			{
				base.SmtpResponse = SmtpResponse.XSessionParamsOk;
			}
		}

		internal override void OutboundCreateCommand()
		{
		}

		internal override void OutboundFormatCommand()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			StringBuilder stringBuilder = new StringBuilder("XSESSIONPARAMS");
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " {0}={1}", new object[]
			{
				"MDBGUID",
				smtpOutSession.NextHopConnection.Key.NextHopConnector.ToString("N")
			});
			base.ProtocolCommandString = stringBuilder.ToString();
		}

		internal override void OutboundProcessResponse()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			if (base.SmtpResponse.SmtpResponseType == SmtpResponseType.Success)
			{
				smtpOutSession.PrepareNextStateForEstablishedSession();
				return;
			}
			smtpOutSession.FailoverConnection(base.SmtpResponse);
			smtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
		}
	}
}
