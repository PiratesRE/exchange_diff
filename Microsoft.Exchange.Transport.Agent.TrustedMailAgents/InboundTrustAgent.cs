using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;

namespace Microsoft.Exchange.Transport.Agent.TrustedMail
{
	internal class InboundTrustAgent : SmtpReceiveAgent
	{
		public InboundTrustAgent(SmtpServer server, InboundTrustAgentFactory parent, bool enabled)
		{
			if (!enabled)
			{
				return;
			}
			this.smtpServer = server;
			this.parent = parent;
			base.OnMailCommand += this.MailCommandHandler;
			base.OnEndOfHeaders += this.EndOfHeadersHandler;
			base.OnEndOfData += this.EndOfDataHandler;
		}

		public InboundTrustAgent()
		{
		}

		protected virtual bool IsFrontEndTransport
		{
			get
			{
				return this.parent.IsFrontEndTransport;
			}
		}

		protected virtual string ComputerName
		{
			get
			{
				return this.parent.ComputerName;
			}
		}

		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.InboundTrustAgentTracer;
			}
		}

		public void MailCommandHandler(ReceiveCommandEventSource eventSource, MailCommandEventArgs eventArgs)
		{
			if (eventSource == null)
			{
				throw new ArgumentNullException("eventSource");
			}
			if (eventArgs == null)
			{
				throw new ArgumentNullException("eventArgs");
			}
			if (TrustedMailUtils.IsMultiTenancyEnabled)
			{
				return;
			}
			SmtpDomain originatingDomain = this.GetOriginatingDomain(eventArgs);
			bool flag = false;
			if (originatingDomain == null)
			{
				InboundTrustAgent.Tracer.TraceDebug((long)this.GetHashCode(), "No originating domain present on mail");
				return;
			}
			if (TrustedMailUtils.IsOriginatingOrgDomainInboundTrustEnabled(this.GetHashCode(), InboundTrustAgent.Tracer, new TrustedMailUtils.GetRemoteDomainEntryDelegate(this.GetRemoteDomainEntry), new TrustedMailUtils.GetAcceptedDomainEntryDelegate(this.GetAcceptedDomainEntry), originatingDomain, null, out flag))
			{
				InboundTrustAgent.Tracer.TraceDebug((long)this.GetHashCode(), "Originating domain is trusted, so setting anti-spam and accept any recipient permissions");
				this.StampTrustedMessagePermissions(eventArgs);
				eventArgs.MailItemProperties["Microsoft.Exchange.Transport.InboundTrustEnabled"] = true;
				return;
			}
			eventArgs.MailItemProperties["Microsoft.Exchange.Transport.InboundTrustEnabled"] = false;
		}

		public void EndOfHeadersHandler(ReceiveMessageEventSource eventSource, EndOfHeadersEventArgs eventArgs)
		{
			if (eventSource == null)
			{
				throw new ArgumentNullException("eventSource");
			}
			if (eventArgs == null)
			{
				throw new ArgumentNullException("eventArgs");
			}
			if (InboundTrustAgent.AlreadyProcessedMessage(eventArgs.Headers))
			{
				InboundTrustAgent.Tracer.TraceDebug((long)this.GetHashCode(), "Message has already been processed for cross premises headers");
				return;
			}
			if (TrustedMailUtils.HeadersPreservedOutbound(eventArgs.Headers))
			{
				InboundTrustAgent.Tracer.TraceDebug((long)this.GetHashCode(), "Cross premises headers have been preserved for outbound; ignoring the message");
				return;
			}
			SmtpDomain originatingDomain = this.GetOriginatingDomain(eventArgs);
			bool flag = false;
			if (originatingDomain == null)
			{
				InboundTrustAgent.Tracer.TraceDebug((long)this.GetHashCode(), "No originating domain present on mail");
			}
			else if (TrustedMailUtils.InboundTrustEnabledOnMail(this.GetHashCode(), InboundTrustAgent.Tracer, eventArgs.MailItem.Properties, new TrustedMailUtils.GetRemoteDomainEntryDelegate(this.GetRemoteDomainEntry), new TrustedMailUtils.GetAcceptedDomainEntryDelegate(this.GetAcceptedDomainEntry), originatingDomain, eventArgs.MailItem, out flag))
			{
				bool flag2 = TrustedMailUtils.CrossPremisesHeadersPreserved;
				object obj;
				if (eventArgs.MailItem.Properties.TryGetValue("PreserveCrossPremisesHeaders", out obj))
				{
					flag2 = (bool)obj;
				}
				if ((flag2 || flag) && HeaderFirewall.PromoteIncomingCrossPremisesHeaders(InboundTrustAgent.Tracer, eventArgs.Headers))
				{
					InboundTrustAgent.Tracer.TraceDebug((long)this.GetHashCode(), "Cross premises headers found and promoted to organization headers on message");
					TrustedMailUtils.StampHeader(eventArgs.Headers, "X-CrossPremisesHeadersPromoted", this.ComputerName);
					TrustedMailUtils.StampHeader(eventArgs.Headers, "X-MS-Exchange-Organization-Cross-Premises-Headers-Promoted", this.ComputerName);
					this.LogModifyHeaders(eventArgs, InboundTrustAgent.headersPromotedLogEntry);
					if (MultilevelAuth.IsAuthenticated(eventArgs.Headers))
					{
						if (!string.IsNullOrEmpty(eventArgs.MailItem.OriginalAuthenticator) && !string.Equals(eventArgs.MailItem.OriginalAuthenticator, (string)RoutingAddress.NullReversePath, StringComparison.OrdinalIgnoreCase))
						{
							throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unexpected original authenticator value: {0}", new object[]
							{
								eventArgs.MailItem.OriginalAuthenticator
							}));
						}
						eventArgs.MailItem.OriginalAuthenticator = string.Empty;
					}
				}
			}
			if (HeaderFirewall.FilterCrossPremisesHeaders(eventArgs.Headers))
			{
				InboundTrustAgent.Tracer.TraceDebug((long)this.GetHashCode(), "Cross premises headers found and filtered from message");
				TrustedMailUtils.StampHeader(eventArgs.Headers, "X-CrossPremisesHeadersFiltered", this.ComputerName);
				this.LogModifyHeaders(eventArgs, InboundTrustAgent.headersFilteredLogEntry);
			}
			TrustedMailUtils.StampHeader(eventArgs.Headers, "X-MS-Exchange-Organization-Cross-Premises-Headers-Processed", this.ComputerName);
		}

		public void EndOfDataHandler(ReceiveMessageEventSource eventSource, EndOfDataEventArgs eventArgs)
		{
			if (eventSource == null)
			{
				throw new ArgumentNullException("eventSource");
			}
			if (eventArgs == null)
			{
				throw new ArgumentNullException("eventArgs");
			}
			if (!this.IsFrontEndTransport && TrustedMailUtils.HandleCrossPremisesProbeEnabled && CrossPremisesMonitoringHelper.TryHandleCrossPremisesProbe(eventArgs.MailItem, this.smtpServer))
			{
				InboundTrustAgent.Tracer.TraceDebug((long)this.GetHashCode(), "Cross premises monitoring probe found and handled.");
			}
		}

		protected virtual void LogModifyHeaders(EndOfHeadersEventArgs eventArgs, LogEntry logEntry)
		{
			AgentLog.Instance.LogModifyHeaders(base.Name, base.EventTopic, eventArgs, eventArgs.SmtpSession, eventArgs.MailItem, logEntry);
		}

		protected virtual SmtpDomain GetOriginatingDomain(MailCommandEventArgs mailCommandEventArgs)
		{
			if (!string.IsNullOrEmpty(mailCommandEventArgs.Oorg))
			{
				return new SmtpDomain(mailCommandEventArgs.Oorg);
			}
			return null;
		}

		protected virtual SmtpDomain GetOriginatingDomain(EndOfHeadersEventArgs endOfHeadersEventArgs)
		{
			string originatorOrganization = endOfHeadersEventArgs.MailItem.OriginatorOrganization;
			SmtpDomain result = null;
			if (!string.IsNullOrEmpty(originatorOrganization))
			{
				result = new SmtpDomain(originatorOrganization);
			}
			else if (TrustedMailUtils.StampOriginatorOrgForMsitConnector && string.Equals("From_MSITCorp", endOfHeadersEventArgs.SmtpSession.ReceiveConnectorName, StringComparison.OrdinalIgnoreCase))
			{
				result = InboundTrustAgent.MsitOriginatorOrganization;
				endOfHeadersEventArgs.MailItem.OriginatorOrganization = InboundTrustAgent.MsitOriginatorOrganization.Domain;
			}
			return result;
		}

		protected virtual void StampTrustedMessagePermissions(ReceiveEventArgs eventArgs)
		{
			Permission permission = Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit;
			if (TrustedMailUtils.AcceptAnyRecipientOnPremises)
			{
				permission |= Permission.SMTPAcceptAnyRecipient;
			}
			eventArgs.SmtpSession.GrantMailItemPermissions(permission);
		}

		protected virtual RemoteDomainEntry GetRemoteDomainEntry(SmtpDomain domain, MailItem mailItem)
		{
			return TrustedMailUtils.GetRemoteDomainEntry(domain, mailItem);
		}

		protected virtual AcceptedDomainEntry GetAcceptedDomainEntry(SmtpDomain domain, MailItem mailItem)
		{
			return TrustedMailUtils.GetAcceptedDomainEntry(domain, mailItem);
		}

		private static bool AlreadyProcessedMessage(HeaderList headers)
		{
			Header header = headers.FindFirst("X-MS-Exchange-Organization-Cross-Premises-Headers-Processed");
			return null != header;
		}

		private const string MsitConnectorName = "From_MSITCorp";

		private const string PromotedDiagnosticHeader = "X-CrossPremisesHeadersPromoted";

		private const string FilteredDiagnosticHeader = "X-CrossPremisesHeadersFiltered";

		private const string CrossPremisesHeadersProcessedHeader = "X-MS-Exchange-Organization-Cross-Premises-Headers-Processed";

		private static readonly SmtpDomain MsitOriginatorOrganization = SmtpDomain.Parse("microsoft.com");

		private static LogEntry headersPromotedLogEntry = new LogEntry(string.Empty, string.Empty, "Cross premises headers promoted");

		private static LogEntry headersFilteredLogEntry = new LogEntry(string.Empty, string.Empty, "Cross premises headers filtered");

		private readonly InboundTrustAgentFactory parent;

		private readonly SmtpServer smtpServer;
	}
}
