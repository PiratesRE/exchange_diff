using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class ShadowSmtpOutSession : InboundProxySmtpOutSession
	{
		public ShadowSmtpOutSession(ulong sessionId, SmtpOutConnection smtpOutConnection, NextHopConnection nextHopConnection, IPEndPoint target, ProtocolLog protocolLog, ProtocolLoggingLevel loggingLevel, IMailRouter mailRouter, CertificateCache certificateCache, CertificateValidator certificateValidator, ShadowRedundancyManager shadowRedundancyManager, TransportAppConfig transportAppConfig, ITransportConfiguration transportConfiguration, IInboundProxyLayer proxyLayer) : base(sessionId, smtpOutConnection, nextHopConnection, target, protocolLog, loggingLevel, mailRouter, certificateCache, certificateValidator, shadowRedundancyManager, transportAppConfig, transportConfiguration, proxyLayer)
		{
		}

		public override bool SendShadow
		{
			get
			{
				return false;
			}
		}

		public override bool SendXShadowRequest
		{
			get
			{
				return true;
			}
		}

		public override bool SendXQDiscard
		{
			get
			{
				return false;
			}
		}

		public override bool SupportExch50
		{
			get
			{
				return false;
			}
		}

		public override bool ShadowCurrentMailItem
		{
			get
			{
				return true;
			}
		}

		protected override bool MessageContextBlobTransferSupported
		{
			get
			{
				return false;
			}
		}

		protected override SmtpCommand CreateSmtpCommand(string cmd)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "ShadowSmtpOutSession.CreateSmtpCommand: {0}", cmd);
			SmtpCommand smtpCommand = null;
			if (cmd != null)
			{
				if (<PrivateImplementationDetails>{BF04BF36-E6FD-46F0-A1BE-B963EEF0FE07}.$$method0x60027f3-1 == null)
				{
					<PrivateImplementationDetails>{BF04BF36-E6FD-46F0-A1BE-B963EEF0FE07}.$$method0x60027f3-1 = new Dictionary<string, int>(12)
					{
						{
							"ConnectResponse",
							0
						},
						{
							"EHLO",
							1
						},
						{
							"X-EXPS",
							2
						},
						{
							"STARTTLS",
							3
						},
						{
							"X-ANONYMOUSTLS",
							4
						},
						{
							"XSHADOWREQUEST",
							5
						},
						{
							"MAIL",
							6
						},
						{
							"RCPT",
							7
						},
						{
							"DATA",
							8
						},
						{
							"BDAT",
							9
						},
						{
							"RSET",
							10
						},
						{
							"QUIT",
							11
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{BF04BF36-E6FD-46F0-A1BE-B963EEF0FE07}.$$method0x60027f3-1.TryGetValue(cmd, out num))
				{
					switch (num)
					{
					case 0:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.ShadowCreateCmdConnectResponse);
						break;
					case 1:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.ShadowCreateCmdEhlo);
						smtpCommand = new EHLOShadowSmtpCommand(this, this.transportConfiguration);
						break;
					case 2:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.ShadowCreateCmdAuth);
						smtpCommand = new AuthSmtpCommand(this, true, this.transportConfiguration);
						break;
					case 3:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.ShadowCreateCmdStarttls);
						smtpCommand = new StarttlsSmtpCommand(this, false);
						break;
					case 4:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.ShadowCreateCmdStarttls);
						smtpCommand = new StarttlsSmtpCommand(this, true);
						break;
					case 5:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.ShadowCreateCmdXShadowRequest);
						smtpCommand = new XShadowRequestSmtpCommand(this, this.shadowRedundancyManager);
						break;
					case 6:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.ShadowCreateCmdMail);
						smtpCommand = new MailSmtpCommand(this, this.transportAppConfig);
						break;
					case 7:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.ShadowCreateCmdRcpt);
						smtpCommand = new RcptInboundProxySmtpCommand(this, this.recipientCorrelator, this.transportAppConfig);
						break;
					case 8:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.ShadowCreateCmdData);
						smtpCommand = new DataInboundProxySmtpCommand(this, this.transportAppConfig);
						break;
					case 9:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.ShadowCreateCmdBdat);
						smtpCommand = new BdatInboundProxySmtpCommand(this, this.transportAppConfig);
						break;
					case 10:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.ShadowCreateCmdRset);
						smtpCommand = new RsetShadowSmtpCommand(this);
						break;
					case 11:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.ShadowCreateCmdQuit);
						smtpCommand = new QuitSmtpCommand(this);
						break;
					default:
						goto IL_22A;
					}
					if (smtpCommand != null)
					{
						smtpCommand.ParsingStatus = ParsingStatus.Complete;
						smtpCommand.OutboundCreateCommand();
					}
					return smtpCommand;
				}
			}
			IL_22A:
			throw new ArgumentException("Unknown command encountered in ShadowSmtpOut: " + cmd, "cmd");
		}

		protected override IInboundProxyLayer GetProxyLayer(NextHopConnection newConnection)
		{
			if (!(newConnection is ShadowPeerNextHopConnection))
			{
				throw new InvalidOperationException("GetProxyLayer called with incorrect NextHopConnection type");
			}
			return ((ShadowPeerNextHopConnection)newConnection).ProxyLayer;
		}
	}
}
