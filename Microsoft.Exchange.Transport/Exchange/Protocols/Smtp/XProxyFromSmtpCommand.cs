using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class XProxyFromSmtpCommand : SmtpCommand
	{
		public XProxyFromSmtpCommand(ISmtpSession session, ITransportConfiguration transportConfiguration, ITransportAppConfig appConfig) : base(session, "XPROXYFROM", null, LatencyComponent.None)
		{
			this.transportConfiguration = transportConfiguration;
			this.appConfig = appConfig;
		}

		internal override void InboundParseCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XProxyFromInboundParseCommand);
			if (!base.VerifyEhloReceived())
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.WrongSequence);
				return;
			}
			XProxyFromParseOutput xproxyFromParseOutput;
			ParseResult parseResult = XProxyFromSmtpCommandParser.Parse(CommandContext.FromSmtpCommand(this), SmtpInSessionState.FromSmtpInSession(smtpInSession), this.transportConfiguration.ProcessTransportRole, this.appConfig.SmtpReceiveConfiguration.MaxProxyHopCount, ExTraceGlobals.SmtpReceiveTracer, out xproxyFromParseOutput);
			base.SmtpResponse = parseResult.SmtpResponse;
			base.ParsingStatus = parseResult.ParsingStatus;
			if (!parseResult.IsFailed)
			{
				smtpInSession.UpdateSessionWithProxyFromInformation(xproxyFromParseOutput.ProxyParseCommonOutput.ClientIp, xproxyFromParseOutput.ProxyParseCommonOutput.ClientPort, xproxyFromParseOutput.ProxyParseCommonOutput.ClientHelloDomain, xproxyFromParseOutput.SequenceNumber, xproxyFromParseOutput.PermissionsInt, xproxyFromParseOutput.AuthSource);
				return;
			}
			if (parseResult.Equals(ParseResult.CommandNotImplementedProtocolError))
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XProxyFromNotEnabled);
				return;
			}
			if (parseResult.Equals(ParseResult.NotAuthorized))
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XProxyFromNotAuthorized);
				return;
			}
			if (parseResult.Equals(ParseResult.BadCommandSequence))
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.WrongSequence);
			}
		}

		internal override void InboundProcessCommand()
		{
			if (base.ParsingStatus != ParsingStatus.Complete)
			{
				return;
			}
			base.SmtpResponse = SmtpResponse.XProxyFromAccepted;
		}

		internal override void OutboundCreateCommand()
		{
		}

		internal override void OutboundFormatCommand()
		{
			InboundProxySmtpOutSession inboundProxySmtpOutSession = (InboundProxySmtpOutSession)base.SmtpSession;
			StringBuilder stringBuilder = new StringBuilder("XPROXYFROM");
			stringBuilder.AppendFormat(" {0}={1}", XProxyParserUtils.SessionIdKeyword, inboundProxySmtpOutSession.ProxyLayer.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo));
			stringBuilder.AppendFormat(" {0}={1}", XProxyParserUtils.ClientIPKeyword, inboundProxySmtpOutSession.ProxyLayer.ClientEndPoint.Address);
			stringBuilder.AppendFormat(" {0}={1}", XProxyParserUtils.ClientPortKeyword, Convert.ToString(inboundProxySmtpOutSession.ProxyLayer.ClientEndPoint.Port));
			if (!string.IsNullOrEmpty(inboundProxySmtpOutSession.ProxyLayer.ClientHelloDomain))
			{
				stringBuilder.AppendFormat(" {0}={1}", XProxyParserUtils.ClientHelloDomainKeyword, inboundProxySmtpOutSession.ProxyLayer.ClientHelloDomain);
			}
			stringBuilder.AppendFormat(" {0}={1}", XProxyParserUtils.SequenceNumberKeyword, inboundProxySmtpOutSession.ProxyLayer.XProxyFromSeqNum + 1U);
			if (this.appConfig.SmtpInboundProxyConfiguration.SendNewXProxyFromArguments)
			{
				stringBuilder.AppendFormat(" {0}={1}", XProxyParserUtils.PermissionsKeyword, (uint)inboundProxySmtpOutSession.ProxyLayer.Permissions);
				stringBuilder.AppendFormat(" {0}={1}", XProxyParserUtils.AuthenticationSourceKeyword, inboundProxySmtpOutSession.ProxyLayer.AuthenticationSource);
			}
			base.ProtocolCommandString = stringBuilder.ToString();
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Formatted command : {0}", base.ProtocolCommandString);
		}

		internal override void OutboundProcessResponse()
		{
			InboundProxySmtpOutSession inboundProxySmtpOutSession = (InboundProxySmtpOutSession)base.SmtpSession;
			string statusCode = base.SmtpResponse.StatusCode;
			if (statusCode[0] != '2')
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "XPROXYFROM failed with: {0}", base.SmtpResponse);
				inboundProxySmtpOutSession.FailoverConnection(base.SmtpResponse);
				inboundProxySmtpOutSession.NextState = SmtpOutSession.SessionState.Quit;
				return;
			}
			inboundProxySmtpOutSession.PrepareNextStateForEstablishedSession();
		}

		private readonly ITransportConfiguration transportConfiguration;

		private readonly ITransportAppConfig appConfig;
	}
}
