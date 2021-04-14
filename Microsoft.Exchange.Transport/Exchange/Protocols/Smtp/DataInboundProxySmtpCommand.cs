using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class DataInboundProxySmtpCommand : BaseDataInboundProxySmtpCommand
	{
		public DataInboundProxySmtpCommand(ISmtpSession session, ITransportAppConfig transportAppConfig) : base(session, transportAppConfig, "DATA")
		{
		}

		protected override long AccumulatedMessageSize
		{
			get
			{
				return this.smtpInDataParser.TotalBytesWritten;
			}
		}

		protected override bool IsProxying
		{
			get
			{
				return this.dataProxyLayer != null;
			}
		}

		protected override long EohPosition
		{
			get
			{
				return this.smtpInDataParser.EohPos;
			}
		}

		internal override void InboundParseCommand()
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "DATAInboundProxy.InboundParseCommand");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyDataInboundParseCommand);
			DataSmtpCommand.RunInboundParseChecks(smtpInSession, this);
		}

		internal override void InboundProcessCommand()
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "DATAInboundProxy.InboundProcessCommand");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyDataInboundProcessCommand);
			if (base.ParsingStatus != ParsingStatus.MoreDataRequired)
			{
				return;
			}
			this.smtpInDataParser = new SmtpInDataProxyParser(new SmtpInDataProxyParser.EndOfHeadersCallback(base.ParserEndOfHeadersCallback), smtpInSession.MimeDocument);
			this.smtpInDataParser.ExceptionFilter = new ExceptionFilter(base.ParserException);
			base.SmtpResponse = SmtpResponse.StartData;
			smtpInSession.SetRawModeAfterCommandCompleted(new RawDataHandler(this.RawDataReceived));
		}

		public static void SetOorg(HeaderList headerList, ISmtpInSession session)
		{
			string oorg = DataBdatHelpers.GetOorg(session.TransportMailItem, session.Capabilities, session.LogSession, headerList);
			session.TransportMailItem.Oorg = oorg;
			Util.SetOorgHeaders(headerList, oorg);
		}

		protected override void StartDiscardingMessage()
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "DATAInboundProxy.StartDiscardingMessage");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyDataDiscardingMessage);
			this.discardingMessage = true;
			this.smtpInDataParser.StartDiscardingMessage();
			if (this.dataProxyLayer != null)
			{
				this.dataProxyLayer.NotifySmtpInStopProxy();
				this.dataProxyLayer = null;
			}
		}

		private static IAsyncResult RaiseProxyInboundMessageEvent(ISmtpInSession session, AsyncCallback callback)
		{
			session.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyDataRaiseOnProxyInboundMessageEvent);
			session.AgentLatencyTracker.BeginTrackLatency(LatencyComponent.SmtpReceiveOnProxyInboundMessage, session.TransportMailItem.LatencyTracker);
			ProxyInboundMessageEventSource proxyInboundMessageEventSource = ProxyInboundMessageEventSourceImpl.Create(session.SessionSource);
			bool clientIsPreE15InternalServer = SmtpInSessionUtils.IsAuthenticated(session.RemoteIdentity) && session.AuthMethod == MultilevelAuthMechanism.MUTUALGSSAPI;
			bool localFrontendIsColocatedWithHub = Components.Configuration.LocalServer.TransportServer.IsHubTransportServer && Components.Configuration.LocalServer.TransportServer.IsFrontendTransportServer;
			return session.AgentSession.BeginRaiseEvent("OnProxyInboundMessage", proxyInboundMessageEventSource, new ProxyInboundMessageEventArgs(session.SessionSource, session.TransportMailItemWrapper, clientIsPreE15InternalServer, localFrontendIsColocatedWithHub, session.SmtpInServer.Name), callback, proxyInboundMessageEventSource);
		}

		private AsyncReturnType RawDataReceived(byte[] data, int offset, int numBytes)
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<int>((long)this.GetHashCode(), "DATAInboundProxy.RawDataReceived received {0} bytes", numBytes);
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			bool isDiscardingData = this.smtpInDataParser.IsDiscardingData;
			int num;
			this.seenEod = this.smtpInDataParser.Write(data, offset, numBytes, out num);
			if (numBytes != num)
			{
				smtpInSession.PutBackReceivedBytes(numBytes - num);
			}
			if (!isDiscardingData && this.smtpInDataParser.IsDiscardingData)
			{
				this.StartDiscardingMessage();
			}
			if (!this.discardingMessage && !this.seenEoh && !SmtpInSessionUtils.HasSMTPBypassMessageSizeLimitPermission(smtpInSession.Permissions) && smtpInSession.Connector.MaxHeaderSize.ToBytes() < (ulong)this.AccumulatedMessageSize)
			{
				base.SmtpResponse = SmtpResponse.HeadersTooLarge;
				smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "AccumulatedMessageSize: {0} > MaxHeaderSize: {1}", new object[]
				{
					this.AccumulatedMessageSize,
					smtpInSession.Connector.MaxHeaderSize
				});
				base.IsResponseReady = false;
				this.StartDiscardingMessage();
			}
			if (!this.discardingMessage && (this.seenEoh || !base.OnlyCheckMessageSizeAfterEoh) && DataBdatHelpers.MessageSizeExceeded(this.AccumulatedMessageSize, this.originalMessageSize, this.messageSizeLimit, smtpInSession.Permissions))
			{
				base.SmtpResponse = SmtpResponse.MessageTooLarge;
				smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "AccumulatedMessageSize: {0} > MessageSizeLimit: {1}", new object[]
				{
					(ulong)this.AccumulatedMessageSize,
					this.messageSizeLimit
				});
				base.IsResponseReady = false;
				this.StartDiscardingMessage();
				if (smtpInSession.SmtpReceivePerformanceCounters != null)
				{
					smtpInSession.SmtpReceivePerformanceCounters.MessagesRefusedForSize.Increment();
				}
			}
			if (this.eohEventArgs != null)
			{
				DataBdatHelpers.RaiseEOHEvent(null, smtpInSession, new AsyncCallback(this.ContinueEndOfHeaders), this.eohEventArgs);
			}
			else if (this.IsProxying)
			{
				this.dataProxyLayer.BeginWriteData(data, offset, num, this.seenEod, base.WriteCompleteCallback);
			}
			else if (this.seenEod)
			{
				base.RaiseOnRejectIfNecessary();
			}
			else
			{
				smtpInSession.RawDataReceivedCompleted();
			}
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<int, AsyncReturnType>((long)this.GetHashCode(), "RawDataReceived consumed {0} bytes, return returnType={1}", num, AsyncReturnType.Async);
			return AsyncReturnType.Async;
		}

		private void ContinueEndOfHeaders(IAsyncResult ar)
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "DATAInboundProxy.ContinueEndOfHeaders");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyDataContinueEndOfHeaders);
			smtpInSession.AgentLatencyTracker.EndTrackLatency();
			base.ProcessAgentResponse(ar, this.eohEventArgs);
			this.eohEventArgs = null;
			if (!this.discardingMessage)
			{
				DataInboundProxySmtpCommand.RaiseProxyInboundMessageEvent((ISmtpInSession)base.SmtpSession, new AsyncCallback(this.ContinueProxyInboundMessage));
				return;
			}
			if (this.seenEod)
			{
				base.RaiseOnRejectIfNecessary();
				return;
			}
			smtpInSession.RawDataReceivedCompleted();
		}

		protected override void FinishEodSequence()
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "DATAInboundProxy.FinishEodSequence");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyDataFinishEodSequence);
			if (base.SmtpResponse.Equals(SmtpResponse.Empty))
			{
				base.SmtpResponse = SmtpResponse.GenericProxyFailure;
			}
			base.IsResponseReady = true;
			if (base.SmtpResponse.StatusCode[0] != '2')
			{
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveMessageRejected, null, new object[]
				{
					smtpInSession.Connector.Name,
					base.SmtpResponse
				});
			}
			else if (!this.discardingMessage)
			{
				smtpInSession.UpdateSmtpReceivePerfCountersForMessageReceived(smtpInSession.TransportMailItem.Recipients.Count, this.dataProxyLayer.BytesWritten);
				smtpInSession.UpdateInboundProxyDestinationPerfCountersForMessageReceived(smtpInSession.TransportMailItem.Recipients.Count, this.dataProxyLayer.BytesWritten);
				smtpInSession.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.SuccessfulSubmission);
			}
			base.ParsingStatus = ParsingStatus.Complete;
			if (base.ShouldDisconnect && !smtpInSession.SessionSource.ShouldDisconnect)
			{
				smtpInSession.Disconnect(DisconnectReason.DroppedSession);
			}
			smtpInSession.ReleaseMailItem();
			smtpInSession.RawDataReceivedCompleted();
		}

		private void ContinueProxyInboundMessage(IAsyncResult ar)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.AgentLatencyTracker.EndTrackLatency();
			ProxyInboundMessageEventSourceImpl proxyInboundMessageEventSourceImpl = (ProxyInboundMessageEventSourceImpl)ar.AsyncState;
			IEnumerable<INextHopServer> enumerable = null;
			bool internalDestination = false;
			if (this.TransportAppConfig.SmtpInboundProxyConfiguration.ProxyDestinations != null && this.TransportAppConfig.SmtpInboundProxyConfiguration.ProxyDestinations.Count != 0)
			{
				smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Proxy destination(s) obtained from app config");
				List<RoutingHost> list = new List<RoutingHost>(this.TransportAppConfig.SmtpInboundProxyConfiguration.ProxyDestinations.Count);
				foreach (IPAddress ipaddress in this.TransportAppConfig.SmtpInboundProxyConfiguration.ProxyDestinations)
				{
					list.Add(new RoutingHost(ipaddress.ToString()));
				}
				enumerable = list;
				internalDestination = this.TransportAppConfig.SmtpInboundProxyConfiguration.IsInternalDestination;
			}
			else if (proxyInboundMessageEventSourceImpl.DestinationServers != null)
			{
				smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Proxy destination(s) obtained from OnProxyInboundMessage event");
				enumerable = proxyInboundMessageEventSourceImpl.DestinationServers;
				internalDestination = proxyInboundMessageEventSourceImpl.InternalDestination;
			}
			if (enumerable != null)
			{
				BaseDataInboundProxySmtpCommand.FinalizeLatencyTracking(smtpInSession);
				this.dataProxyLayer = new InboundDataProxyLayer(smtpInSession.SessionId, smtpInSession.RemoteEndPoint, smtpInSession.HelloDomain, smtpInSession.AdvertisedEhloOptions, smtpInSession.XProxyFromSeqNum, smtpInSession.TransportMailItem, internalDestination, enumerable, this.TransportAppConfig.SmtpInboundProxyConfiguration.AccumulatedMessageSizeThreshold.ToBytes(), smtpInSession.LogSession, smtpInSession.SmtpInServer.SmtpOutConnectionHandler, this.TransportAppConfig.SmtpInboundProxyConfiguration.PreserveTargetResponse, smtpInSession.ProxiedClientPermissions, smtpInSession.AuthenticationSourceForAgents, smtpInSession.SmtpInServer.InboundProxyDestinationTracker);
				this.dataProxyLayer.SetupProxySession();
				byte[] accumulatedBytesForProxying = this.smtpInDataParser.GetAccumulatedBytesForProxying();
				this.dataProxyLayer.BeginWriteData(accumulatedBytesForProxying, 0, accumulatedBytesForProxying.Length, this.seenEod, base.WriteCompleteCallback, false);
				return;
			}
			smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "No destinations could be obtained to proxy to");
			SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveNoDestinationToProxyTo, smtpInSession.Connector.Name, new object[]
			{
				smtpInSession.ToString()
			});
			ExTraceGlobals.SmtpReceiveTracer.TraceError<string>((long)this.GetHashCode(), "No destinations could be obtained to proxy to. Details {0}", smtpInSession.ToString());
			if (this.TransportAppConfig.SmtpInboundProxyConfiguration.PreserveTargetResponse)
			{
				base.SmtpResponse = SmtpResponse.NoProxyDestinationsResponse;
			}
			else
			{
				base.SmtpResponse = SmtpResponse.EncodedProxyFailureResponseNoDestinations;
			}
			this.StartDiscardingMessage();
			if (this.seenEod)
			{
				base.RaiseOnRejectIfNecessary();
				return;
			}
			smtpInSession.RawDataReceivedCompleted();
		}

		internal override void OutboundCreateCommand()
		{
		}

		internal override void OutboundFormatCommand()
		{
			if (this.sentDataVerb)
			{
				return;
			}
			base.ProtocolCommandString = "DATA";
		}

		internal override void OutboundProcessResponse()
		{
			InboundProxySmtpOutSession inboundProxySmtpOutSession = (InboundProxySmtpOutSession)base.SmtpSession;
			string statusCode = base.SmtpResponse.StatusCode;
			if (inboundProxySmtpOutSession.NextHopConnection == null || inboundProxySmtpOutSession.RoutedMailItem == null)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<SmtpResponse>((long)this.GetHashCode(), "Connection already marked for Failover or the message has already been acked for a non-success status.  Not processing response for the DATA command: {0}", base.SmtpResponse);
				return;
			}
			if (statusCode[0] == '5')
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<string, SmtpResponse>((long)this.GetHashCode(), "DATA command for message from {0} failed with: {1}", inboundProxySmtpOutSession.RoutedMailItem.From.ToString(), base.SmtpResponse);
				inboundProxySmtpOutSession.AckMessage(AckStatus.Fail, base.SmtpResponse);
				inboundProxySmtpOutSession.PrepareForNextMessage(true);
				return;
			}
			if (this.sentDataVerb)
			{
				bool issueBetweenMsgRset = false;
				if (statusCode[0] != '2')
				{
					ExTraceGlobals.SmtpSendTracer.TraceError<string, IPEndPoint, SmtpResponse>((long)this.GetHashCode(), "Failed to deliver message from {0} to {1}, Status: {2}", inboundProxySmtpOutSession.RoutedMailItem.From.ToString(), inboundProxySmtpOutSession.RemoteEndPoint, base.SmtpResponse);
					inboundProxySmtpOutSession.AckMessage(AckStatus.Retry, base.SmtpResponse);
					issueBetweenMsgRset = true;
				}
				else
				{
					ExTraceGlobals.SmtpSendTracer.TraceDebug<string, IPEndPoint>((long)this.GetHashCode(), "Delivered message from {0} to {1}", inboundProxySmtpOutSession.RoutedMailItem.From.ToString(), inboundProxySmtpOutSession.RemoteEndPoint);
					inboundProxySmtpOutSession.AckMessage(AckStatus.Success, base.SmtpResponse, inboundProxySmtpOutSession.ProxyLayer.BytesRead);
				}
				inboundProxySmtpOutSession.PrepareForNextMessage(issueBetweenMsgRset);
				return;
			}
			if (!string.Equals(statusCode, "354", StringComparison.Ordinal))
			{
				inboundProxySmtpOutSession.AckMessage(AckStatus.Retry, base.SmtpResponse);
				inboundProxySmtpOutSession.PrepareForNextMessage(true);
				return;
			}
			this.sentDataVerb = true;
			base.ParsingStatus = ParsingStatus.MoreDataRequired;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (this.dataProxyLayer != null)
					{
						this.dataProxyLayer.NotifySmtpInStopProxy();
						this.dataProxyLayer = null;
					}
					if (this.smtpInDataParser != null)
					{
						this.smtpInDataParser.Dispose();
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private SmtpInDataProxyParser smtpInDataParser;

		private InboundDataProxyLayer dataProxyLayer;

		private bool sentDataVerb;
	}
}
