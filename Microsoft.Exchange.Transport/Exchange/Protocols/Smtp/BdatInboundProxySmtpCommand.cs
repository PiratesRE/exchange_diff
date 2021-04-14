using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class BdatInboundProxySmtpCommand : BaseDataInboundProxySmtpCommand
	{
		public BdatInboundProxySmtpCommand(ISmtpSession session, ITransportAppConfig transportAppConfig) : base(session, transportAppConfig, "BDAT")
		{
		}

		protected override long AccumulatedMessageSize
		{
			get
			{
				return this.smtpInBdatParser.TotalBytesWritten;
			}
		}

		protected override bool IsProxying
		{
			get
			{
				return this.bdatProxyLayer != null;
			}
		}

		protected override long EohPosition
		{
			get
			{
				return this.smtpInBdatParser.EohPos;
			}
		}

		public bool IsBdat0Last
		{
			get
			{
				return this.isLastChunk && this.chunkSize == 0L;
			}
		}

		internal override void InboundParseCommand()
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "BDATInboundProxy.InboundParseCommand");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyBdatInboundParseCommand);
			bool flag;
			bool flag2;
			if (!BdatSmtpCommand.RunBdatSequenceChecks(this, smtpInSession, out flag, out flag2))
			{
				return;
			}
			if (flag)
			{
				this.RestoreBdatState();
			}
			ParseResult parseResult = BdatSmtpCommandParser.Parse(CommandContext.FromSmtpCommand(this), SmtpInSessionState.FromSmtpInSession(smtpInSession), this.totalChunkSize, out this.chunkSize);
			if (parseResult.IsFailed)
			{
				if (parseResult.SmtpResponse == SmtpResponse.MessageTooLarge)
				{
					BdatSmtpCommand.SetMessageTooLargeResponse(smtpInSession, this, false);
				}
				else
				{
					base.SmtpResponse = parseResult.SmtpResponse;
					base.ParsingStatus = parseResult.ParsingStatus;
				}
				this.StartDiscardingMessage();
				smtpInSession.Disconnect(DisconnectReason.DroppedSession);
				return;
			}
			this.isLastChunk = BdatSmtpCommandParser.IsLastChunk(parseResult.ParsingStatus);
			if ((this.seenEoh || !base.OnlyCheckMessageSizeAfterEoh) && BdatSmtpCommandParser.IsMessageSizeExceeded(SmtpInSessionUtils.HasSMTPBypassMessageSizeLimitPermission(smtpInSession.Permissions), this.totalChunkSize, this.chunkSize, this.originalMessageSize, this.messageSizeLimit, smtpInSession.LogSession, ExTraceGlobals.SmtpReceiveTracer))
			{
				BdatSmtpCommand.SetMessageTooLargeResponse(smtpInSession, this, true);
				this.StartDiscardingMessage();
				return;
			}
			if (this.discardingMessage)
			{
				base.SmtpResponse = SmtpResponse.BadCommandSequence;
				base.IsResponseReady = false;
			}
			base.ParsingStatus = ParsingStatus.MoreDataRequired;
		}

		internal override void InboundProcessCommand()
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "BDATInboundProxy.InboundProcessCommand");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyDataInboundProcessCommand);
			base.IsResponseReady = false;
			if (this.smtpInBdatParser == null)
			{
				this.smtpInBdatParser = new SmtpInBdatProxyParser(smtpInSession.MimeDocument);
			}
			this.smtpInBdatParser.ResetForNewBdatCommand(this.chunkSize, this.discardingMessage, this.isLastChunk, new SmtpInDataProxyParser.EndOfHeadersCallback(base.ParserEndOfHeadersCallback), new ExceptionFilter(base.ParserException));
			if (!this.isLastChunk)
			{
				base.LowAuthenticationLevelTarpitOverride = TarpitAction.DoTarpit;
			}
			smtpInSession.SetRawModeAfterCommandCompleted(new RawDataHandler(this.RawDataReceived));
			if (this.IsProxying)
			{
				this.bdatProxyLayer.CreateNewCommand(this.chunkSize, this.chunkSize, this.isLastChunk);
			}
		}

		internal override void OutboundCreateCommand()
		{
		}

		internal override void OutboundFormatCommand()
		{
			InboundProxySmtpOutSession inboundProxySmtpOutSession = (InboundProxySmtpOutSession)base.SmtpSession;
			this.chunkSize = inboundProxySmtpOutSession.ProxyLayer.OutboundChunkSize;
			this.totalChunkSize = inboundProxySmtpOutSession.ProxyLayer.BytesRead + inboundProxySmtpOutSession.ProxyLayer.OutboundChunkSize;
			if (inboundProxySmtpOutSession.ProxyLayer.IsLastChunk)
			{
				base.ProtocolCommandString = string.Format(CultureInfo.InvariantCulture, "BDAT {0} LAST", new object[]
				{
					this.chunkSize
				});
				return;
			}
			base.ProtocolCommandString = string.Format(CultureInfo.InvariantCulture, "BDAT {0}", new object[]
			{
				this.chunkSize
			});
		}

		internal override void OutboundProcessResponse()
		{
			InboundProxySmtpOutSession inboundProxySmtpOutSession = (InboundProxySmtpOutSession)base.SmtpSession;
			string statusCode = base.SmtpResponse.StatusCode;
			bool issueBetweenMsgRset = true;
			bool flag = true;
			if (statusCode[0] == '5')
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<SmtpResponse>((long)this.GetHashCode(), "Message body response was: ", base.SmtpResponse);
				inboundProxySmtpOutSession.AckMessage(AckStatus.Fail, base.SmtpResponse);
			}
			else if (statusCode[0] != '2')
			{
				ExTraceGlobals.SmtpSendTracer.TraceError<string, IPEndPoint, SmtpResponse>((long)this.GetHashCode(), "Failed to deliver message from {0} to {1}, Status: {2}", inboundProxySmtpOutSession.RoutedMailItem.From.ToString(), inboundProxySmtpOutSession.RemoteEndPoint, base.SmtpResponse);
				inboundProxySmtpOutSession.AckMessage(AckStatus.Retry, base.SmtpResponse);
			}
			else if (inboundProxySmtpOutSession.ProxyLayer.IsLastChunk)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<string, IPEndPoint>((long)this.GetHashCode(), "Delivered message from {0} to {1}", inboundProxySmtpOutSession.RoutedMailItem.From.ToString(), inboundProxySmtpOutSession.RemoteEndPoint);
				issueBetweenMsgRset = false;
				inboundProxySmtpOutSession.AckMessage(AckStatus.Success, base.SmtpResponse, this.totalChunkSize);
			}
			else
			{
				inboundProxySmtpOutSession.ProxyLayer.AckCommandSuccessful();
				flag = false;
			}
			if (flag)
			{
				inboundProxySmtpOutSession.PrepareForNextMessage(issueBetweenMsgRset);
				return;
			}
			inboundProxySmtpOutSession.NextState = SmtpOutSession.SessionState.Bdat;
			inboundProxySmtpOutSession.WaitingForNextProxiedBdat = true;
		}

		protected override void StartDiscardingMessage()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyBdatDiscardingMessage);
			this.discardingMessage = true;
			if (this.smtpInBdatParser != null)
			{
				this.smtpInBdatParser.StartDiscardingMessage();
			}
			if (this.bdatProxyLayer != null)
			{
				this.bdatProxyLayer.NotifySmtpInStopProxy();
				this.bdatProxyLayer = null;
			}
			this.StoreCurrentDataState();
		}

		private static IAsyncResult RaiseProxyInboundMessageEvent(ISmtpInSession session, AsyncCallback callback)
		{
			session.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyBdatRaiseOnProxyInboundMessageEvent);
			session.AgentLatencyTracker.BeginTrackLatency(LatencyComponent.SmtpReceiveOnProxyInboundMessage, session.TransportMailItem.LatencyTracker);
			ProxyInboundMessageEventSource proxyInboundMessageEventSource = ProxyInboundMessageEventSourceImpl.Create(session.SessionSource);
			bool clientIsPreE15InternalServer = SmtpInSessionUtils.IsAuthenticated(session.RemoteIdentity) && session.AuthMethod == MultilevelAuthMechanism.MUTUALGSSAPI;
			bool localFrontendIsColocatedWithHub = Components.Configuration.LocalServer.TransportServer.IsHubTransportServer && Components.Configuration.LocalServer.TransportServer.IsFrontendTransportServer;
			return session.AgentSession.BeginRaiseEvent("OnProxyInboundMessage", proxyInboundMessageEventSource, new ProxyInboundMessageEventArgs(session.SessionSource, session.TransportMailItemWrapper, clientIsPreE15InternalServer, localFrontendIsColocatedWithHub, session.SmtpInServer.Name), callback, proxyInboundMessageEventSource);
		}

		private AsyncReturnType RawDataReceived(byte[] data, int offset, int numBytes)
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<int>((long)this.GetHashCode(), "RawDataReceived received {0} bytes", numBytes);
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			bool isDiscardingData = this.smtpInBdatParser.IsDiscardingData;
			int num;
			this.seenEod = this.smtpInBdatParser.Write(data, offset, numBytes, out num);
			if (numBytes != num)
			{
				smtpInSession.PutBackReceivedBytes(numBytes - num);
			}
			this.bytesReceived += (long)num;
			if (!isDiscardingData && this.smtpInBdatParser.IsDiscardingData)
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
			if (this.eohEventArgs != null)
			{
				DataBdatHelpers.RaiseEOHEvent(null, smtpInSession, new AsyncCallback(this.ContinueEndOfHeaders), this.eohEventArgs);
			}
			else if (this.IsProxying)
			{
				this.bdatProxyLayer.BeginWriteData(data, offset, num, this.seenEod, base.WriteCompleteCallback);
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
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "ContinueEndOfHeaders");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyBdatContinueEndOfHeaders);
			smtpInSession.AgentLatencyTracker.EndTrackLatency();
			base.ProcessAgentResponse(ar, this.eohEventArgs);
			this.eohEventArgs = null;
			if (!this.discardingMessage)
			{
				BdatInboundProxySmtpCommand.RaiseProxyInboundMessageEvent((ISmtpInSession)base.SmtpSession, new AsyncCallback(this.ContinueProxyInboundMessage));
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
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "FinishEodSequence");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.InboundProxyBdatFinishEodSequence);
			if (!this.discardingMessage)
			{
				this.totalChunkSize += this.chunkSize;
				if (base.SmtpResponse.Equals(SmtpResponse.Empty))
				{
					if (this.seenEoh)
					{
						base.SmtpResponse = SmtpResponse.DataTransactionFailed;
					}
					else if (!this.isLastChunk)
					{
						base.SmtpResponse = SmtpResponse.OctetsReceived(this.chunkSize);
					}
				}
			}
			else if (base.SmtpResponse.Equals(SmtpResponse.Empty))
			{
				base.SmtpResponse = SmtpResponse.GenericProxyFailure;
			}
			base.IsResponseReady = true;
			if (base.SmtpResponse.SmtpResponseType != SmtpResponseType.Success)
			{
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveMessageRejected, null, new object[]
				{
					smtpInSession.Connector.Name,
					base.SmtpResponse
				});
			}
			else if (this.isLastChunk)
			{
				if (this.bdatProxyLayer != null)
				{
					smtpInSession.UpdateSmtpReceivePerfCountersForMessageReceived(smtpInSession.TransportMailItem.Recipients.Count, this.bdatProxyLayer.BytesWritten);
					smtpInSession.UpdateInboundProxyDestinationPerfCountersForMessageReceived(smtpInSession.TransportMailItem.Recipients.Count, this.bdatProxyLayer.BytesWritten);
				}
				smtpInSession.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.SuccessfulSubmission);
			}
			base.ParsingStatus = ParsingStatus.Complete;
			if (base.ShouldDisconnect && !smtpInSession.SessionSource.ShouldDisconnect)
			{
				smtpInSession.Disconnect(DisconnectReason.DroppedSession);
			}
			if (this.isLastChunk)
			{
				smtpInSession.ReleaseMailItem();
				smtpInSession.BdatState = null;
			}
			else if (!this.discardingMessage)
			{
				this.StoreCurrentDataState();
			}
			this.bdatProxyLayer = null;
			smtpInSession.RawDataReceivedCompleted();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (this.bdatProxyLayer != null)
					{
						this.bdatProxyLayer.NotifySmtpInStopProxy();
						this.bdatProxyLayer = null;
					}
					if (this.smtpInBdatParser != null && this.isLastChunk)
					{
						this.smtpInBdatParser.Dispose();
						this.smtpInBdatParser = null;
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private void StoreCurrentDataState()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (smtpInSession.BdatState == null)
			{
				smtpInSession.BdatState = new SmtpInBdatState();
			}
			smtpInSession.BdatState.TotalChunkSize = this.totalChunkSize;
			smtpInSession.BdatState.DiscardingMessage = this.discardingMessage;
			smtpInSession.BdatState.SeenEoh = this.seenEoh;
			smtpInSession.BdatState.OriginalMessageSize = this.originalMessageSize;
			smtpInSession.BdatState.MessageSizeLimit = this.messageSizeLimit;
			smtpInSession.BdatState.ProxyParser = this.smtpInBdatParser;
			smtpInSession.BdatState.ProxyLayer = this.bdatProxyLayer;
		}

		private void RestoreBdatState()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			this.totalChunkSize = smtpInSession.BdatState.TotalChunkSize;
			this.discardingMessage = smtpInSession.BdatState.DiscardingMessage;
			this.seenEoh = smtpInSession.BdatState.SeenEoh;
			this.originalMessageSize = smtpInSession.BdatState.OriginalMessageSize;
			this.messageSizeLimit = smtpInSession.BdatState.MessageSizeLimit;
			this.smtpInBdatParser = smtpInSession.BdatState.ProxyParser;
			this.bdatProxyLayer = smtpInSession.BdatState.ProxyLayer;
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
				this.bdatProxyLayer = new InboundBdatProxyLayer(smtpInSession.SessionId, smtpInSession.RemoteEndPoint, smtpInSession.HelloDomain, smtpInSession.AdvertisedEhloOptions, smtpInSession.XProxyFromSeqNum, smtpInSession.TransportMailItem, internalDestination, enumerable, this.TransportAppConfig.SmtpInboundProxyConfiguration.AccumulatedMessageSizeThreshold.ToBytes(), smtpInSession.LogSession, smtpInSession.SmtpInServer.SmtpOutConnectionHandler, this.TransportAppConfig.SmtpInboundProxyConfiguration.PreserveTargetResponse, smtpInSession.ProxiedClientPermissions, smtpInSession.AuthenticationSourceForAgents, smtpInSession.SmtpInServer.InboundProxyDestinationTracker);
				byte[] accumulatedBytesForProxying = this.smtpInBdatParser.GetAccumulatedBytesForProxying();
				this.bdatProxyLayer.CreateNewCommand(this.chunkSize, (long)accumulatedBytesForProxying.Length + this.chunkSize - this.bytesReceived, this.isLastChunk);
				this.bdatProxyLayer.SetupProxySession();
				this.bdatProxyLayer.BeginWriteData(accumulatedBytesForProxying, 0, accumulatedBytesForProxying.Length, this.seenEod, base.WriteCompleteCallback, false);
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

		private SmtpInBdatProxyParser smtpInBdatParser;

		private InboundBdatProxyLayer bdatProxyLayer;

		private long chunkSize;

		private long bytesReceived;

		private long totalChunkSize;

		private bool isLastChunk;
	}
}
