using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal abstract class BaseDataInboundProxySmtpCommand : SmtpCommand
	{
		public BaseDataInboundProxySmtpCommand(ISmtpSession session, ITransportAppConfig transportAppConfig, string protocolCommandKeyword) : base(session, protocolCommandKeyword, "OnDataCommand", LatencyComponent.SmtpReceiveOnDataCommand)
		{
			DataCommandEventArgs dataCommandEventArgs = new DataCommandEventArgs();
			this.CommandEventArgs = dataCommandEventArgs;
			ISmtpInSession smtpInSession = session as ISmtpInSession;
			if (smtpInSession != null)
			{
				dataCommandEventArgs.MailItem = smtpInSession.TransportMailItemWrapper;
				this.messageSizeLimit = (long)smtpInSession.Connector.MaxMessageSize.ToBytes();
				if (smtpInSession.SmtpReceivePerformanceCounters != null)
				{
					smtpInSession.SmtpReceivePerformanceCounters.InboundMessageConnectionsCurrent.Increment();
					smtpInSession.SmtpReceivePerformanceCounters.InboundMessageConnectionsTotal.Increment();
				}
			}
			this.TransportAppConfig = transportAppConfig;
			this.writeCompleteCallback = new InboundProxyLayer.CompletionCallback(this.WriteProxyDataComplete);
		}

		protected abstract long AccumulatedMessageSize { get; }

		protected abstract bool IsProxying { get; }

		protected abstract long EohPosition { get; }

		protected bool ShouldDisconnect
		{
			get
			{
				return this.shouldDisconnect;
			}
		}

		protected InboundProxyLayer.CompletionCallback WriteCompleteCallback
		{
			get
			{
				return this.writeCompleteCallback;
			}
		}

		protected abstract void StartDiscardingMessage();

		public static void FinalizeLatencyTracking(ISmtpInSession session)
		{
			LatencyComponent previousHopLatencyComponent = BaseDataInboundProxySmtpCommand.GetPreviousHopLatencyComponent(session);
			LatencyHeaderManager.HandleLatencyHeaders(null, session.TransportMailItem.RootPart.Headers, session.TransportMailItem.DateReceived, previousHopLatencyComponent);
			LatencyTracker.EndTrackLatency(LatencyComponent.SmtpReceive, session.TransportMailItem.LatencyTracker);
			LatencyHeaderManager.FinalizeLatencyHeadersOnFrontend(session.TransportMailItem, session.SmtpInServer.ServerConfiguration.Fqdn);
			LatencyTracker.BeginTrackLatency(LatencyComponent.Delivery, session.TransportMailItem.LatencyTracker);
		}

		public void ParserEndOfHeadersCallback(HeaderList headerList)
		{
			if (headerList == null)
			{
				throw new ArgumentNullException("headerList");
			}
			if (this.seenEoh)
			{
				throw new InvalidOperationException("EndOfHeadersCallback got called again");
			}
			if (this.EohPosition == -1L)
			{
				throw new InvalidOperationException("The parsers Eoh position cannot be -1");
			}
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyBaseDataParserEndOfHeadersCallback);
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "ParserEndOfHeadersCallback");
			this.seenEoh = true;
			if (this.discardingMessage)
			{
				return;
			}
			if (!DataBdatHelpers.CheckHeaders(headerList, smtpInSession, this.EohPosition, this) || !DataBdatHelpers.CheckMaxHopCounts(headerList, smtpInSession, this, this.TransportAppConfig.Routing.LocalLoopDetectionEnabled, this.TransportAppConfig.Routing.LocalLoopSubdomainDepth))
			{
				this.StartDiscardingMessage();
				return;
			}
			RestrictedHeaderSet blocked = SmtpInSessionUtils.RestrictedHeaderSetFromPermissions(smtpInSession.Permissions);
			HeaderFirewall.Filter(headerList, blocked);
			string text;
			DataBdatHelpers.PatchHeaders(headerList, smtpInSession, this.TransportAppConfig.SmtpDataConfiguration.AcceptAndFixSmtpAddressWithInvalidLocalPart, out text);
			DataBdatHelpers.UpdateLoopDetectionCounter(headerList, smtpInSession, this.TransportAppConfig.Routing.LocalLoopDetectionSubDomainLeftToRightOffsetForPerfCounter, this.TransportAppConfig.Routing.LoopDetectionNumberOfTransits, text);
			if (smtpInSession.TransportMailItem != null)
			{
				smtpInSession.TransportMailItem.ExposeMessageHeaders = true;
				smtpInSession.TransportMailItem.InternetMessageId = text;
				if (!this.CheckAttributionAndLoadADRecipientCache(smtpInSession))
				{
					this.StartDiscardingMessage();
					return;
				}
				if (!DataBdatHelpers.CheckMessageSubmitPermissions(smtpInSession, this))
				{
					this.StartDiscardingMessage();
					return;
				}
				if (!SmtpInSessionUtils.HasSMTPBypassMessageSizeLimitPermission(smtpInSession.Permissions))
				{
					long num;
					if (SmtpInSessionUtils.HasSMTPAcceptOrgHeadersPermission(smtpInSession.Permissions) && DataBdatHelpers.TryGetOriginalSize(headerList, out num))
					{
						this.originalMessageSize = num;
					}
					if (base.OnlyCheckMessageSizeAfterEoh && DataBdatHelpers.MessageSizeExceeded(this.AccumulatedMessageSize, this.originalMessageSize, this.messageSizeLimit, smtpInSession.Permissions))
					{
						base.SmtpResponse = SmtpResponse.MessageTooLarge;
						base.IsResponseReady = false;
						this.StartDiscardingMessage();
						if (smtpInSession.SmtpReceivePerformanceCounters != null)
						{
							smtpInSession.SmtpReceivePerformanceCounters.MessagesRefusedForSize.Increment();
						}
						return;
					}
				}
				DataInboundProxySmtpCommand.SetOorg(headerList, smtpInSession);
			}
			DataBdatHelpers.EnableEOHEvent(smtpInSession, headerList, out this.eohEventArgs);
		}

		public void WriteProxyDataComplete(SmtpResponse response)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (!response.Equals(SmtpResponse.Empty))
			{
				base.SmtpResponse = response;
				if (response.SmtpResponseType != SmtpResponseType.Success)
				{
					this.StartDiscardingMessage();
				}
			}
			if (this.seenEod)
			{
				this.RaiseOnRejectIfNecessary();
				return;
			}
			smtpInSession.RawDataReceivedCompleted();
		}

		protected void ProcessAgentResponse(IAsyncResult ar, EventArgs args)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			SmtpResponse smtpResponse = smtpInSession.AgentSession.EndRaiseEvent(ar);
			this.shouldDisconnect = (!smtpResponse.IsEmpty || smtpInSession.SessionSource.ShouldDisconnect);
			if (this.shouldDisconnect)
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxySessionDisconnectByAgent);
				if (!smtpResponse.IsEmpty)
				{
					base.SmtpResponse = smtpResponse;
				}
				this.StartDiscardingMessage();
				return;
			}
			if (args != null && !smtpInSession.SessionSource.SmtpResponse.Equals(SmtpResponse.Empty))
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyMessageRejectedByAgent);
				base.SmtpResponse = smtpInSession.SessionSource.SmtpResponse;
				this.originalEventArgs = args;
				this.StartDiscardingMessage();
				smtpInSession.SessionSource.SmtpResponse = SmtpResponse.Empty;
			}
		}

		protected void ParserException(Exception e)
		{
			if (e is ExchangeDataException)
			{
				base.SmtpResponse = SmtpResponse.InvalidContent;
				ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
				smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "A parsing error has occurred: {0}", new object[]
				{
					e.Message
				});
			}
			else if (e is IOException)
			{
				base.SmtpResponse = SmtpResponse.CTSParseError;
			}
			else if (base.SmtpResponse.Equals(SmtpResponse.Empty))
			{
				base.SmtpResponse = SmtpResponse.DataTransactionFailed;
			}
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<Exception>((long)this.GetHashCode(), "Handled parser exception: {0}", e);
		}

		protected void RaiseOnRejectIfNecessary()
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "RaiseOnRejectIfNecessary");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyBaseDataRaiseOnRejectIfNecessary);
			if (this.discardingMessage && !this.shouldDisconnect)
			{
				smtpInSession.RaiseOnRejectEvent(null, this.originalEventArgs, base.SmtpResponse, new AsyncCallback(this.ContinueOnReject));
				return;
			}
			this.FinishEodSequence();
		}

		protected void ContinueOnReject(IAsyncResult ar)
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "OnRejectCallback");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyBaseDataContinueOnReject);
			this.ProcessAgentResponse(ar, null);
			this.FinishEodSequence();
		}

		protected abstract void FinishEodSequence();

		private static LatencyComponent GetPreviousHopLatencyComponent(ISmtpInSession session)
		{
			LatencyComponent result;
			if (session.TransportMailItem.ExtendedProperties.GetValue<uint>("Microsoft.Exchange.Transport.TransportMailItem.InboundProxySequenceNumber", 0U) != 0U)
			{
				result = LatencyComponent.SmtpSend;
			}
			else if (SmtpInSessionUtils.IsAuthenticated(session.RemoteIdentity) && session.AuthMethod == MultilevelAuthMechanism.MUTUALGSSAPI && session.SmtpInServer.Configuration.LocalServer.TransportServer.IsHubTransportServer && session.SmtpInServer.Configuration.LocalServer.TransportServer.IsFrontendTransportServer)
			{
				result = LatencyComponent.DeliveryQueueInternal;
			}
			else if (session.TransportMailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Hygiene-ReleasedFromQuarantine") != null)
			{
				result = LatencyComponent.QuarantineReleaseOrReport;
			}
			else
			{
				result = LatencyComponent.ExternalServers;
			}
			return result;
		}

		private bool CheckAttributionAndLoadADRecipientCache(ISmtpInSession session)
		{
			SmtpResponse smtpResponse = DataBdatHelpers.CheckAttributionAndCreateAdRecipientCache(session.TransportMailItem, this.TransportAppConfig.SmtpReceiveConfiguration.RejectUnscopedMessages, session.InboundClientProxyState != InboundClientProxyStates.None, true);
			if (!smtpResponse.Equals(SmtpResponse.Empty))
			{
				base.SmtpResponse = smtpResponse;
			}
			return smtpResponse.Equals(SmtpResponse.Empty);
		}

		protected readonly ITransportAppConfig TransportAppConfig;

		protected long messageSizeLimit;

		protected bool seenEoh;

		protected bool seenEod;

		protected bool discardingMessage;

		protected EndOfHeadersEventArgs eohEventArgs;

		private EventArgs originalEventArgs;

		protected long originalMessageSize = long.MaxValue;

		private bool shouldDisconnect;

		private readonly InboundProxyLayer.CompletionCallback writeCompleteCallback;
	}
}
