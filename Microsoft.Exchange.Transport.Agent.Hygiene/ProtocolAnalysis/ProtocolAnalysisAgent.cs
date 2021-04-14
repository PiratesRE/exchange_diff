using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProtocolAnalysis;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;
using Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis
{
	internal class ProtocolAnalysisAgent : SmtpReceiveAgent
	{
		public ProtocolAnalysisAgent(ProtocolAnalysisAgentFactory factory, SmtpServer server, SenderReputationConfig settings)
		{
			this.factory = factory;
			this.server = server;
			this.settings = settings;
			base.OnConnect += this.OnConnectHandler;
			base.OnEndOfHeaders += this.OnEndOfHeadersHandler;
			base.OnRsetCommand += this.OnRsetHandler;
			base.OnEndOfData += this.OnEndOfDataHandler;
			base.OnDisconnect += this.OnDisconnectHandler;
			base.OnReject += this.OnRejectHandler;
			this.agentSenders = new Dictionary<IPAddress, AgentSenderData>(10);
		}

		public void OnConnectHandler(ConnectEventSource source, ConnectEventArgs connectArgs)
		{
			if (!ProtocolAnalysisAgentFactory.FirstConnect)
			{
				return;
			}
			lock (this.syncObject)
			{
				if (ProtocolAnalysisAgentFactory.FirstConnect && ProtocolAnalysisAgent.AgentFactory.BgAgentFactory.PaBgAgent != null)
				{
					ProtocolAnalysisAgentFactory.FirstConnect = false;
					if (ProtocolAnalysisAgentFactory.SrlCalculationDisabled)
					{
						ProtocolAnalysisAgentFactory.LogSrlCalculationDisabled();
					}
					ProtocolAnalysisAgent.AgentFactory.BgAgentFactory.PaBgAgent.Startup();
				}
			}
		}

		public void OnEndOfHeadersHandler(ReceiveMessageEventSource source, EndOfHeadersEventArgs mailArgs)
		{
			ExTraceGlobals.OnMailFromTracer.TraceDebug((long)this.GetHashCode(), "OnMailFromHandler");
			if (!this.IsPolicyEnabled(mailArgs.SmtpSession))
			{
				return;
			}
			if (this.HasAntispamBypassPermission(mailArgs.SmtpSession, ExTraceGlobals.OnMailFromTracer))
			{
				return;
			}
			if (source == null)
			{
				throw new ArgumentNullException("source", "The ReceiveCommandEventSource object cannot be null");
			}
			if (mailArgs == null)
			{
				throw new ArgumentNullException("mailArgs", "The Mail command arguments cannot be null");
			}
			SmtpSession smtpSession = mailArgs.SmtpSession;
			if (smtpSession == null)
			{
				throw new ArgumentNullException("mailArgs.SmtpSession", "The session object cannot be null");
			}
			AgentSenderData agentSenderData;
			if (this.agentSenders.TryGetValue(IPAddress.Any, out agentSenderData))
			{
				ExTraceGlobals.OnMailFromTracer.TraceError((long)this.GetHashCode(), "Remove existing IPAddress.Any record inside OnMailFromHandler");
				this.agentSenders.Remove(IPAddress.Any);
			}
			if (this.msgProcessing)
			{
				ExTraceGlobals.OnMailFromTracer.TraceError((long)this.GetHashCode(), "msgProcessing is true.");
				this.agentSenders.Remove(this.currentSender);
			}
			this.currentSender = this.RetrieveSenderIPAddress(smtpSession);
			if (!this.agentSenders.TryGetValue(this.currentSender, out agentSenderData) || agentSenderData == null)
			{
				agentSenderData = new AgentSenderData(DateTime.UtcNow);
			}
			agentSenderData.OnMailFrom();
			MailItem mailItem = mailArgs.MailItem;
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailArgs.MailItem", "The MailItem cannot be null");
			}
			foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
			{
				agentSenderData.OnValidRecipient(envelopeRecipient.Address.ToString());
			}
			this.lastRecipient = new RoutingAddress(string.Empty);
			this.msgProcessing = true;
			this.agentSenders[this.currentSender] = agentSenderData;
		}

		public void OnRcptToHandler(ReceiveCommandEventSource source, RcptCommandEventArgs rcptArgs)
		{
			ExTraceGlobals.OnRcptToTracer.TraceDebug((long)this.GetHashCode(), "OnRcptToHandler");
			if (!this.IsPolicyEnabled(rcptArgs.SmtpSession))
			{
				return;
			}
			if (this.HasAntispamBypassPermission(rcptArgs.SmtpSession, ExTraceGlobals.OnRcptToTracer))
			{
				return;
			}
			if (source == null)
			{
				throw new ArgumentNullException("source", "The ReceiveCommandEventSource object cannot be null");
			}
			if (rcptArgs == null)
			{
				throw new ArgumentNullException("rcptArgs", "The RCPT command arguments cannot be null");
			}
			if (rcptArgs.SmtpSession == null)
			{
				throw new ArgumentNullException("rcptArgs.SmtpSession", "The session object cannot be null");
			}
			this.OnPrivateRcptToHandler(rcptArgs, false);
		}

		public void OnRsetHandler(ReceiveCommandEventSource source, RsetCommandEventArgs rsetArgs)
		{
			ExTraceGlobals.OnRcptToTracer.TraceDebug((long)this.GetHashCode(), "OnRsetHandler");
			if (!this.IsPolicyEnabled(rsetArgs.SmtpSession))
			{
				return;
			}
			if (this.HasAntispamBypassPermission(rsetArgs.SmtpSession, ExTraceGlobals.OnEODTracer))
			{
				return;
			}
			if (source == null)
			{
				throw new ArgumentNullException("source", "The ReceiveCommandEventSource object cannot be null");
			}
			if (rsetArgs == null)
			{
				throw new ArgumentNullException("rsetArgs", "The RsetCommandEventArgs object cannot be null");
			}
			SmtpSession smtpSession = rsetArgs.SmtpSession;
			if (smtpSession == null)
			{
				throw new ArgumentNullException("rsetArgs.SmtpSession", "The session object cannot be null");
			}
			if (!this.msgProcessing)
			{
				ExTraceGlobals.OnEODTracer.TraceDebug((long)this.GetHashCode(), "Receive RSET while not processing msg.");
				return;
			}
			ExTraceGlobals.OnEODTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "In reset handler> currentSender: {0}", this.currentSender);
			AgentSenderData agentSenderData;
			if (!this.agentSenders.TryGetValue(this.currentSender, out agentSenderData) || agentSenderData == null)
			{
				ExTraceGlobals.OnEODTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "sender record not found for current sender {0}.", this.currentSender);
				return;
			}
			agentSenderData.OnEndOfData(0, 0L, CallerIdStatus.Null);
			this.agentSenders[this.currentSender] = agentSenderData;
			this.OnEndOfDataComplete();
			if (this.currentSender.Equals(IPAddress.Any))
			{
				this.agentSenders.Remove(this.currentSender);
				IPAddress ipaddress = this.RetrieveSenderIPAddress(smtpSession);
				if (StsUtil.IsValidSenderIP(ipaddress))
				{
					this.agentSenders[ipaddress] = agentSenderData;
					this.currentSender = ipaddress;
				}
			}
		}

		public void OnEndOfDataHandler(ReceiveMessageEventSource source, EndOfDataEventArgs eodArgs)
		{
			ExTraceGlobals.OnEODTracer.TraceDebug((long)this.GetHashCode(), "OnEndOfDataHandler");
			if (!this.IsPolicyEnabled(eodArgs.SmtpSession))
			{
				return;
			}
			if (this.HasAntispamBypassPermission(eodArgs.SmtpSession, ExTraceGlobals.OnEODTracer))
			{
				return;
			}
			if (source == null)
			{
				throw new ArgumentNullException("source", "The ReceiveMessageEventSource object cannot be null");
			}
			if (eodArgs == null)
			{
				throw new ArgumentNullException("eodArgs", "The EndOfData command arguments cannot be null");
			}
			MailItem mailItem = eodArgs.MailItem;
			if (mailItem == null)
			{
				throw new ArgumentNullException("eodArgs.MailItem", "The MailItem cannot be null");
			}
			SmtpSession smtpSession = eodArgs.SmtpSession;
			if (smtpSession == null)
			{
				throw new ArgumentNullException("eodArgs.SmtpSession", "The session object cannot be null");
			}
			this.OnPrivateEndOfDataHandler(smtpSession, mailItem);
			this.OnEndOfDataComplete();
		}

		public void OnDisconnectHandler(DisconnectEventSource source, DisconnectEventArgs sessionArgs)
		{
			ExTraceGlobals.DisconnectTracer.TraceError<int>((long)this.GetHashCode(), "Session stopped: {0} sender", this.agentSenders.Count);
			if (!this.IsPolicyEnabled(sessionArgs.SmtpSession))
			{
				return;
			}
			if (this.HasAntispamBypassPermission(sessionArgs.SmtpSession, ExTraceGlobals.DisconnectTracer))
			{
				return;
			}
			if (source == null)
			{
				throw new ArgumentNullException("source", "The DisconnectEventSource object cannot be null");
			}
			if (sessionArgs == null)
			{
				throw new ArgumentNullException("sessionArgs", "The Disconnect command arguments cannot be null");
			}
			SmtpSession smtpSession = sessionArgs.SmtpSession;
			if (smtpSession == null)
			{
				throw new ArgumentNullException("sessionArgs.SmtpSession", "The session object cannot be null");
			}
			if (this.msgProcessing)
			{
				ExTraceGlobals.DisconnectTracer.TraceError((long)this.GetHashCode(), "Session stopped without EndOfData event");
				if (this.currentSender.Equals(IPAddress.Any))
				{
					ExTraceGlobals.DisconnectTracer.TraceError((long)this.GetHashCode(), "current sender was an invalid IP entry");
					return;
				}
				AgentSenderData agentSenderData;
				this.agentSenders.TryGetValue(this.currentSender, out agentSenderData);
				if (this.agentSenders.TryGetValue(this.currentSender, out agentSenderData) && agentSenderData != null)
				{
					agentSenderData.OnEndOfData(0, 0L, CallerIdStatus.Null);
					this.agentSenders[this.currentSender] = agentSenderData;
				}
				this.OnEndOfDataComplete();
			}
			AgentSenderData agentSenderData2;
			object obj;
			if (smtpSession.IsExternalConnection && this.agentSenders.TryGetValue(this.currentSender, out agentSenderData2) && agentSenderData2 != null && smtpSession.Properties.TryGetValue("Microsoft.Exchange.ReverseDns", out obj))
			{
				agentSenderData2.ReverseDns = (string)obj;
			}
			IDictionaryEnumerator dictionaryEnumerator = this.agentSenders.GetEnumerator();
			while (dictionaryEnumerator.MoveNext())
			{
				AgentSenderData agentSenderData3 = (AgentSenderData)dictionaryEnumerator.Value;
				if (agentSenderData3 != null)
				{
					agentSenderData3.OnEndOfSession(smtpSession.HelloDomain);
				}
			}
			this.factory.OnDisconnect(this.agentSenders);
			this.agentSenders = null;
		}

		public void OnRejectHandler(RejectEventSource source, RejectEventArgs rejectArgs)
		{
			ExTraceGlobals.RejectTracer.TraceDebug<Type>((long)this.GetHashCode(), "Reject event {0}", rejectArgs.GetType());
			if (source == null)
			{
				throw new ArgumentNullException("source", "The RejectEventSource object cannot be null");
			}
			if (rejectArgs == null)
			{
				throw new ArgumentNullException("rejectArgs", "The OnReject command arguments cannot be null");
			}
			SmtpSession smtpSession = rejectArgs.SmtpSession;
			if (smtpSession == null)
			{
				throw new ArgumentNullException("rejectArgs.SmtpSession", "The session object cannot be null");
			}
			if (!this.msgProcessing)
			{
				ExTraceGlobals.RejectTracer.TraceDebug((long)this.GetHashCode(), "Receive Reject while not processing msg.");
				return;
			}
			if (!this.IsPolicyEnabled(smtpSession))
			{
				return;
			}
			if (this.HasAntispamBypassPermission(smtpSession, ExTraceGlobals.RejectTracer))
			{
				return;
			}
			EventArgs originalArguments = rejectArgs.OriginalArguments;
			if (originalArguments != null)
			{
				ExTraceGlobals.RejectTracer.TraceDebug<IPAddress, Type>((long)this.GetHashCode(), "Reject event received, sender: {0}, type: {1}", this.currentSender, originalArguments.GetType());
				RcptCommandEventArgs rcptCommandEventArgs = originalArguments as RcptCommandEventArgs;
				if (rcptCommandEventArgs != null)
				{
					ExTraceGlobals.RejectTracer.Information<IPAddress>((long)this.GetHashCode(), "Reject RcptCommand, sender: {0}", this.currentSender);
					this.OnPrivateRcptToHandler(rcptCommandEventArgs, true);
					return;
				}
				EndOfDataEventArgs endOfDataEventArgs = originalArguments as EndOfDataEventArgs;
				if (endOfDataEventArgs != null)
				{
					ExTraceGlobals.RejectTracer.Information<IPAddress>((long)this.GetHashCode(), "Reject EndOfDataEvent, sender: {0}", this.currentSender);
					MailItem mailItem = endOfDataEventArgs.MailItem;
					if (mailItem == null)
					{
						throw new ArgumentNullException("rejectArgs.OriginalArguments.MailItem", "The MailItem cannot be null");
					}
					bool flag = ProtocolAnalysisAgent.RetrieveProperty<bool>(smtpSession.Properties, "Microsoft.Exchange.IsOnDenyList", false);
					if (flag)
					{
						ExTraceGlobals.RejectTracer.Information((long)this.GetHashCode(), "The message's sender is already on deny list");
						this.agentSenders.Remove(this.currentSender);
						return;
					}
					this.OnPrivateEndOfDataHandler(smtpSession, mailItem);
					this.OnEndOfDataComplete();
					return;
				}
				else
				{
					ExTraceGlobals.RejectTracer.Information<IPAddress>((long)this.GetHashCode(), "Reject unknown event, sender: {0}", this.currentSender);
				}
			}
		}

		private static T RetrieveProperty<T>(IDictionary<string, object> properties, string propertyName, T defValue)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties", "The properties object cannot be null");
			}
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentNullException("propertyName", "The property name object cannot be null or empty");
			}
			object obj;
			if (properties.TryGetValue(propertyName, out obj))
			{
				return (T)((object)obj);
			}
			return defValue;
		}

		private bool IsPolicyEnabled(SmtpSession session)
		{
			bool flag = CommonUtils.IsEnabled(this.settings, session);
			if (!flag)
			{
				ExTraceGlobals.FactoryTracer.TraceDebug((long)this.GetHashCode(), "Not processing this event because the policy is disabled.");
			}
			return flag;
		}

		private bool HasAntispamBypassPermission(SmtpSession session, Trace tracer)
		{
			return CommonUtils.HasAntispamBypassPermission(session, tracer, this);
		}

		private IPAddress RetrieveSenderIPAddress(SmtpSession session)
		{
			IPAddress ipaddress = IPAddress.Any;
			if (session.IsExternalConnection)
			{
				ipaddress = session.RemoteEndPoint.Address;
			}
			else if (session.LastExternalIPAddress != null)
			{
				ipaddress = session.LastExternalIPAddress;
				ExTraceGlobals.FactoryTracer.Information<IPAddress>((long)this.GetHashCode(), "The session is not on Edge, OriginatingIP:{0}.", ipaddress);
			}
			else
			{
				ExTraceGlobals.FactoryTracer.TraceDebug((long)this.GetHashCode(), "The session is not on Edge, and OriginatingIP is null.");
			}
			return ipaddress;
		}

		private void OnPrivateEndOfDataHandler(SmtpSession session, MailItem mailItem)
		{
			if (!this.msgProcessing)
			{
				ExTraceGlobals.OnEODTracer.TraceDebug((long)this.GetHashCode(), "Receive EndOfData without receiving MailFrom.");
				return;
			}
			IPAddress ipaddress = this.RetrieveSenderIPAddress(session);
			if (!StsUtil.IsValidSenderIP(ipaddress))
			{
				ExTraceGlobals.OnEODTracer.TraceError((long)this.GetHashCode(), "Can not retrieve sender's IP address at EndOfDataEventHandler");
				this.agentSenders.Remove(this.currentSender);
				return;
			}
			AgentSenderData agentSenderData;
			if (!ipaddress.Equals(this.currentSender))
			{
				if (session.IsExternalConnection)
				{
					throw new InvalidOperationException("IsExternalConnection flag doesn't match.");
				}
				if (!this.currentSender.Equals(IPAddress.Any))
				{
					throw new InvalidOperationException("Sender IP Address doesn't match IPAddress.Any");
				}
				agentSenderData = this.agentSenders[this.currentSender];
				this.agentSenders.Remove(this.currentSender);
				this.currentSender = ipaddress;
			}
			else
			{
				agentSenderData = this.agentSenders[this.currentSender];
			}
			bool flag = ProtocolAnalysisAgent.RetrieveProperty<bool>(session.Properties, "Microsoft.Exchange.IsOnAllowList", false);
			bool flag2 = ProtocolAnalysisAgent.RetrieveProperty<bool>(session.Properties, "Microsoft.Exchange.IsOnSafeList", false);
			if (flag || flag2)
			{
				ExTraceGlobals.OnEODTracer.TraceDebug<IPAddress>((long)this.GetHashCode(), "The sender {0} is on allow or safe lists. Ignore it.", this.currentSender);
				this.agentSenders.Remove(this.currentSender);
				return;
			}
			if (agentSenderData == null)
			{
				ExTraceGlobals.OnEODTracer.TraceError((long)this.GetHashCode(), "Can't find existing sender record inside EndOfDataEventHandler");
				throw new InvalidOperationException("OnEndOfData> senderData is null.");
			}
			int scl = ProtocolAnalysisAgent.RetrieveProperty<int>(mailItem.Properties, "Microsoft.Exchange.UnmodifiedSCL", 0);
			long mimeStreamLength = mailItem.MimeStreamLength;
			CallerIdStatus status = this.RetrieveCallerId(mailItem.Properties, "Microsoft.Exchange.CallerIdStatus", CallerIdStatus.Invalid);
			agentSenderData.OnEndOfData(scl, mimeStreamLength, status);
			this.agentSenders[this.currentSender] = agentSenderData;
		}

		private void OnEndOfDataComplete()
		{
			this.msgProcessing = false;
			this.lastRecipient = new RoutingAddress(string.Empty);
		}

		private void OnPrivateRcptToHandler(RcptCommandEventArgs rcptArgs, bool fReject)
		{
			if (fReject && this.lastRecipient == rcptArgs.RecipientAddress)
			{
				return;
			}
			this.lastRecipient = rcptArgs.RecipientAddress;
			string domainPart = rcptArgs.RecipientAddress.DomainPart;
			if (this.acceptedDomains == null)
			{
				this.acceptedDomains = this.server.AcceptedDomains;
			}
			bool flag = false;
			Microsoft.Exchange.Data.Transport.AcceptedDomain acceptedDomain = this.acceptedDomains.Find(domainPart);
			if (acceptedDomain != null && acceptedDomain.IsInCorporation)
			{
				flag = true;
			}
			RecipientStatus recipientStatus = RecipientStatus.Valid;
			if (flag)
			{
				recipientStatus = (fReject ? RecipientStatus.Invalid : RecipientStatus.Valid);
			}
			AgentSenderData agentSenderData = null;
			if (!this.agentSenders.TryGetValue(this.currentSender, out agentSenderData) || agentSenderData == null)
			{
				ExTraceGlobals.OnRcptToTracer.TraceDebug<string>((long)this.GetHashCode(), "OnPrivateRcptToHandler: no sender with ip %1 found in the sender data collection", this.currentSender.ToString());
				return;
			}
			switch (recipientStatus)
			{
			case RecipientStatus.Valid:
				agentSenderData.OnValidRecipient((string)rcptArgs.RecipientAddress);
				break;
			case RecipientStatus.Invalid:
				agentSenderData.OnInvalidRecipient((string)rcptArgs.RecipientAddress);
				break;
			case RecipientStatus.Unknown:
				agentSenderData.OnUnknownRecipient((string)rcptArgs.RecipientAddress);
				break;
			}
			this.agentSenders[this.currentSender] = agentSenderData;
		}

		private CallerIdStatus RetrieveCallerId(IDictionary<string, object> properties, string propertyName, CallerIdStatus defValue)
		{
			string text;
			try
			{
				text = ProtocolAnalysisAgent.RetrieveProperty<string>(properties, propertyName, string.Empty);
			}
			catch (InvalidCastException ex)
			{
				ExTraceGlobals.OnEODTracer.TraceError<string>((long)this.GetHashCode(), "Failed to cast callerId info to string, error: {0}", ex.Message);
				return defValue;
			}
			CallerIdStatus result;
			if (string.IsNullOrEmpty(text))
			{
				result = defValue;
			}
			else if (string.Equals(text, "Valid", StringComparison.Ordinal))
			{
				result = CallerIdStatus.Valid;
			}
			else if (string.Equals(text, "Invalid", StringComparison.Ordinal))
			{
				result = CallerIdStatus.Invalid;
			}
			else if (string.Equals(text, "Indeterminate", StringComparison.Ordinal))
			{
				result = CallerIdStatus.Indeterminate;
			}
			else if (string.Equals(text, "EpdError", StringComparison.Ordinal))
			{
				result = CallerIdStatus.EpdError;
			}
			else if (string.Equals(text, "Error", StringComparison.Ordinal))
			{
				result = CallerIdStatus.Error;
			}
			else
			{
				ExTraceGlobals.OnEODTracer.TraceError<string>((long)this.GetHashCode(), "Unknown callerId info to string: {0}", text);
				result = defValue;
			}
			return result;
		}

		private const string UnmodifiedSclRating = "Microsoft.Exchange.UnmodifiedSCL";

		private const string ReverseDNS = "Microsoft.Exchange.ReverseDns";

		private const int DefaultSclRating = 0;

		private const string CallerIdStatusString = "Microsoft.Exchange.CallerIdStatus";

		internal static ExEventLog EventLogger = new ExEventLog(ExTraceGlobals.FactoryTracer.Category, "MSExchange Antispam");

		internal static ProtocolAnalysisAgentFactory AgentFactory;

		private ProtocolAnalysisAgentFactory factory;

		private Dictionary<IPAddress, AgentSenderData> agentSenders;

		private RoutingAddress lastRecipient;

		private IPAddress currentSender = IPAddress.Any;

		private AcceptedDomainCollection acceptedDomains;

		private bool msgProcessing;

		private SmtpServer server;

		private object syncObject = new object();

		private SenderReputationConfig settings;
	}
}
