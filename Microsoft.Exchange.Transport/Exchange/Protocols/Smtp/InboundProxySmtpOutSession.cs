using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class InboundProxySmtpOutSession : SmtpOutSession
	{
		public InboundProxySmtpOutSession(ulong sessionId, SmtpOutConnection smtpOutConnection, NextHopConnection nextHopConnection, IPEndPoint target, ProtocolLog protocolLog, ProtocolLoggingLevel loggingLevel, IMailRouter mailRouter, CertificateCache certificateCache, CertificateValidator certificateValidator, ShadowRedundancyManager shadowRedundancyManager, TransportAppConfig transportAppConfig, ITransportConfiguration transportConfiguration, IInboundProxyLayer proxyLayer) : base(sessionId, smtpOutConnection, nextHopConnection, target, protocolLog, loggingLevel, mailRouter, certificateCache, certificateValidator, shadowRedundancyManager, transportAppConfig, transportConfiguration, false)
		{
			this.proxyLayer = proxyLayer;
			this.ReadFromProxyLayerCompleteCallback = new InboundProxyLayer.ReadCompletionCallback(this.ReadFromProxyLayerComplete);
			this.WaitForNextBdatCompleteCallback = new InboundBdatProxyLayer.CommandReceivedCallback(this.WaitForBdatCommandComplete);
		}

		public IInboundProxyLayer ProxyLayer
		{
			get
			{
				return this.proxyLayer;
			}
		}

		public bool WaitingForNextProxiedBdat
		{
			get
			{
				return this.waitingForNextProxiedBdat;
			}
			set
			{
				this.waitingForNextProxiedBdat = value;
			}
		}

		public override bool SendShadow
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

		protected override bool SendFewerMessagesToSlowerServerEnabled
		{
			get
			{
				return false;
			}
		}

		protected override bool FailoverPermittedForRemoteShutdown
		{
			get
			{
				return !this.proxyingMessageBody;
			}
		}

		protected override void IncrementConnectionCounters()
		{
			base.IncrementConnectionCounters();
			if (!this.transportAppConfig.SmtpInboundProxyConfiguration.TrackInboundProxyDestinationsInRcpt)
			{
				this.proxyLayer.InboundProxyDestinationTracker.IncrementProxyCount(this.proxyLayer.NextHopFqdn);
			}
		}

		protected override void DecrementConnectionCounters()
		{
			base.DecrementConnectionCounters();
			if (!this.transportAppConfig.SmtpInboundProxyConfiguration.TrackInboundProxyDestinationsInRcpt)
			{
				this.proxyLayer.InboundProxyDestinationTracker.DecrementProxyCount(this.proxyLayer.NextHopFqdn);
			}
		}

		public override void ShutdownConnection()
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "InboundProxySmtpOutSession.ShutdownConnection");
			base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyShutdownConnection);
			this.dontCacheThisConnection = true;
			base.ShutdownConnection();
			this.proxyLayer.Shutdown();
		}

		public override void ResetSession(SmtpOutConnection smtpOutConnection, NextHopConnection nextHopConnection)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "InboundProxySmtpOutSession.ResetSession");
			this.proxyLayer = this.GetProxyLayer(nextHopConnection);
			this.waitingForNextProxiedBdat = false;
			base.ResetSession(smtpOutConnection, nextHopConnection);
			this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Proxying inbound session with session id {0}", new object[]
			{
				this.proxyLayer.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo)
			});
		}

		public override void AckMessage(AckStatus ackStatus, SmtpResponse smtpResponse, long messageSize, SessionSetupFailureReason failureReason, bool updateSmtpSendFailureCounters)
		{
			this.proxyingMessageBody = false;
			bool replaceFailureResponse = failureReason != SessionSetupFailureReason.ProtocolError;
			this.ProxyLayer.AckMessage(ackStatus, smtpResponse, replaceFailureResponse, "InboundProxySmtpOutSession.AckMessage with session id" + base.SessionId, failureReason);
			base.AckMessage(ackStatus, smtpResponse, messageSize, failureReason, updateSmtpSendFailureCounters);
		}

		public override void PrepareForNextMessageOnCachedSession()
		{
			base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyPrepareForNextMessageOnCachedSession);
			if (base.AdvertisedEhloOptions.XProxyFrom)
			{
				base.NextState = SmtpOutSession.SessionState.Rset;
				base.SendNextCommands();
				return;
			}
			base.PrepareForNextMessageOnCachedSession();
		}

		protected override void HandleError(object error)
		{
			bool failoverConnection = !this.proxyingMessageBody;
			base.HandleError(error, false, failoverConnection);
		}

		protected override bool PreProcessMessage()
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "InboundProxySmtpOutSession.PreProcessMessage");
			if (base.NextHopDeliveryType == DeliveryType.Heartbeat)
			{
				throw new InvalidOperationException("Inbound proxy should never send a hearbeat");
			}
			bool supportLongAddresses = base.SupportLongAddresses;
			bool supportOrar = base.SupportOrar;
			bool supportRDst = base.SupportRDst;
			bool supportSmtpUtf = base.SupportSmtpUtf8;
			if (!base.CheckLongSenderSupport(supportLongAddresses))
			{
				return false;
			}
			if (!base.CheckSmtpUtf8SenderSupport(supportSmtpUtf))
			{
				return false;
			}
			foreach (MailRecipient recipient in base.NextHopConnection.ReadyRecipients)
			{
				if (!base.PreProcessRecipient(recipient, supportLongAddresses, supportOrar, supportRDst, supportSmtpUtf))
				{
					ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "Message from '{0}' was Ack'ed because a recipient failed in PreProcess()", base.RoutedMailItem.From.ToString());
					base.AckMessage(AckStatus.Fail, SmtpResponse.NoRecipientSucceeded);
					this.ProxyLayer.AckMessage(AckStatus.Fail, SmtpResponse.NoRecipientSucceeded, true, "InboundProxySmtpOutSession.PreProcessMessage for session id " + base.SessionId, SessionSetupFailureReason.ProtocolError);
					return false;
				}
			}
			return true;
		}

		protected override SmtpCommand CreateSmtpCommand(string cmd)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "InboundProxySmtpOutSession.CreateSmtpCommand: {0}", cmd);
			SmtpCommand smtpCommand = null;
			if (cmd != null)
			{
				if (<PrivateImplementationDetails>{BF04BF36-E6FD-46F0-A1BE-B963EEF0FE07}.$$method0x60027e2-1 == null)
				{
					<PrivateImplementationDetails>{BF04BF36-E6FD-46F0-A1BE-B963EEF0FE07}.$$method0x60027e2-1 = new Dictionary<string, int>(12)
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
							"MAIL",
							5
						},
						{
							"RCPT",
							6
						},
						{
							"DATA",
							7
						},
						{
							"BDAT",
							8
						},
						{
							"RSET",
							9
						},
						{
							"XPROXYFROM",
							10
						},
						{
							"QUIT",
							11
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{BF04BF36-E6FD-46F0-A1BE-B963EEF0FE07}.$$method0x60027e2-1.TryGetValue(cmd, out num))
				{
					switch (num)
					{
					case 0:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyCreateCmdConnectResponse);
						break;
					case 1:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyCreateCmdEhlo);
						smtpCommand = new EHLOInboundProxySmtpCommand(this);
						break;
					case 2:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyCreateCmdAuth);
						smtpCommand = new AuthSmtpCommand(this, true, this.transportConfiguration);
						break;
					case 3:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyCreateCmdStarttls);
						smtpCommand = new StarttlsSmtpCommand(this, false);
						break;
					case 4:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyCreateCmdStarttls);
						smtpCommand = new StarttlsSmtpCommand(this, true);
						break;
					case 5:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyCreateCmdMail);
						smtpCommand = new MailSmtpCommand(this, this.transportAppConfig);
						break;
					case 6:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyCreateCmdRcpt);
						smtpCommand = new RcptInboundProxySmtpCommand(this, this.recipientCorrelator, this.transportAppConfig);
						break;
					case 7:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyCreateCmdData);
						smtpCommand = new DataInboundProxySmtpCommand(this, this.transportAppConfig);
						break;
					case 8:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyCreateCmdBdat);
						smtpCommand = new BdatInboundProxySmtpCommand(this, this.transportAppConfig);
						break;
					case 9:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyCreateCmdRset);
						smtpCommand = new RsetInboundProxySmtpCommand(this);
						break;
					case 10:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyCreateCmdXProxyFrom);
						smtpCommand = new XProxyFromSmtpCommand(this, this.transportConfiguration, this.transportAppConfig);
						break;
					case 11:
						base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyCreateCmdQuit);
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
			throw new ArgumentException("Unknown command encountered in SmtpOut: " + cmd, "cmd");
		}

		protected override bool InvokeCommandHandler(SmtpCommand command)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "InboundProxySmtpOutSession.InvokeCommandHandler");
			base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyInvokeCommandHandler);
			command.OutboundFormatCommand();
			if (command.ProtocolCommandString != null)
			{
				command.ProtocolCommand = ByteString.StringToBytesAndAppendCRLF(command.ProtocolCommandString, true);
				if (string.IsNullOrEmpty(command.RedactedProtocolCommandString))
				{
					this.logSession.LogSend(command.ProtocolCommand);
				}
				else
				{
					this.logSession.LogSend(ByteString.StringToBytes(command.RedactedProtocolCommandString, true));
				}
				ExTraceGlobals.SmtpSendTracer.TraceDebug<string>((long)this.GetHashCode(), "Enqueuing Command: {0} on the connection", command.ProtocolCommandString);
				base.EnqueueResponseHandler(command);
				BdatInboundProxySmtpCommand bdatInboundProxySmtpCommand = command as BdatInboundProxySmtpCommand;
				if (bdatInboundProxySmtpCommand != null)
				{
					if (this.sendBuffer.Length != 0)
					{
						throw new InvalidOperationException("BDAT cannot be pipelined");
					}
					this.connection.BeginWrite(command.ProtocolCommand, 0, command.ProtocolCommand.Length, InboundProxySmtpOutSession.WriteBdatCompleteSendBuffersCallback, this);
					return true;
				}
				else
				{
					this.sendBuffer.Append(command.ProtocolCommand);
				}
			}
			else if (command.ProtocolCommand != null)
			{
				base.EnqueueResponseHandler(command);
				this.logSession.LogSend(SmtpOutSession.BinaryData);
				this.sendBuffer.Append(command.ProtocolCommand);
			}
			else
			{
				DataInboundProxySmtpCommand dataInboundProxySmtpCommand = command as DataInboundProxySmtpCommand;
				if (dataInboundProxySmtpCommand != null)
				{
					if (this.sendBuffer.Length != 0)
					{
						throw new InvalidOperationException("DATA cannot send stream unless send buffer is empty");
					}
					base.EnqueueResponseHandler(command);
					this.proxyingMessageBody = true;
					this.SendDataBuffers();
					return true;
				}
				else
				{
					command.Dispose();
				}
			}
			return false;
		}

		protected override void ConnectResponseEvent(SmtpResponse smtpResponse)
		{
			this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Proxying inbound session with session id {0}", new object[]
			{
				this.proxyLayer.SessionId.ToString("X16", NumberFormatInfo.InvariantInfo)
			});
			base.ConnectResponseEvent(smtpResponse);
		}

		protected override void FinalizeNextStateAndSendCommands()
		{
			if (this.WaitingForNextProxiedBdat)
			{
				this.ProxyLayer.WaitForNewCommand(this.WaitForNextBdatCompleteCallback);
				return;
			}
			base.FinalizeNextStateAndSendCommands();
		}

		protected virtual IInboundProxyLayer GetProxyLayer(NextHopConnection newConnection)
		{
			if (!(newConnection is InboundProxyNextHopConnection))
			{
				throw new InvalidOperationException("GetProxyLayer called with incorrect NextHopConnection type");
			}
			return ((InboundProxyNextHopConnection)newConnection).ProxyLayer;
		}

		private static void WriteBdatCompleteSendBuffers(IAsyncResult asyncResult)
		{
			InboundProxySmtpOutSession inboundProxySmtpOutSession = (InboundProxySmtpOutSession)asyncResult.AsyncState;
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)inboundProxySmtpOutSession.GetHashCode(), "InboundProxySmtpOutSession.WriteBdatCompleteSendBuffers");
			inboundProxySmtpOutSession.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyWriteBdatCompleteSendBuffers);
			object obj;
			inboundProxySmtpOutSession.connection.EndWrite(asyncResult, out obj);
			if (obj != null)
			{
				inboundProxySmtpOutSession.HandleError(obj);
				return;
			}
			inboundProxySmtpOutSession.proxyingMessageBody = true;
			if (inboundProxySmtpOutSession.ProxyLayer.OutboundChunkSize == 0L)
			{
				inboundProxySmtpOutSession.sendBuffer.Reset();
				inboundProxySmtpOutSession.StartReadLine();
				return;
			}
			inboundProxySmtpOutSession.ProxyLayer.BeginReadData(inboundProxySmtpOutSession.ReadFromProxyLayerCompleteCallback);
		}

		private static void WriteProxiedBytesToTargetComplete(IAsyncResult asyncResult)
		{
			KeyValuePair<InboundProxySmtpOutSession, BufferCacheEntry> keyValuePair = (KeyValuePair<InboundProxySmtpOutSession, BufferCacheEntry>)asyncResult.AsyncState;
			InboundProxySmtpOutSession key = keyValuePair.Key;
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)key.GetHashCode(), "InboundProxySmtpOutSession.WriteProxiedBytesToTargetComplete");
			key.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyWriteProxiedBytesToTargetComplete);
			object obj;
			key.connection.EndWrite(asyncResult, out obj);
			key.ProxyLayer.ReturnBuffer(keyValuePair.Value);
			if (obj != null)
			{
				key.HandleError(obj);
				return;
			}
			if (key.eodSeen)
			{
				key.sendBuffer.Reset();
				key.StartReadLine();
				return;
			}
			key.ProxyLayer.BeginReadData(key.ReadFromProxyLayerCompleteCallback);
		}

		private void SendDataBuffers()
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug((long)this.GetHashCode(), "InboundProxySmtpOutSession.SendDataBuffers");
			base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxySendDataBuffers);
			this.ProxyLayer.BeginReadData(this.ReadFromProxyLayerCompleteCallback);
		}

		private void ReadFromProxyLayerComplete(BufferCacheEntry buffer, bool lastBuffer)
		{
			ExTraceGlobals.SmtpSendTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "InboundProxySmtpOutSession.ReadFromProxyLayerComplete. buffer = {0}. lastBuffer = {1}", (buffer == null) ? "null" : buffer.Buffer.Length.ToString(CultureInfo.InvariantCulture), lastBuffer);
			base.DropBreadcrumb(SmtpOutSession.SmtpOutSessionBreadcrumbs.InboundProxyReadFromProxyLayerComplete);
			if (buffer != null)
			{
				this.eodSeen = lastBuffer;
				this.connection.BeginWrite(buffer.Buffer, 0, buffer.Buffer.Length, InboundProxySmtpOutSession.WriteProxiedBytesToTargetCompleteCallback, new KeyValuePair<InboundProxySmtpOutSession, BufferCacheEntry>(this, buffer));
				return;
			}
			ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "The proxy layer returned null bytes. Acking the message from {0}.", base.RoutedMailItem.From.ToString());
			this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Proxy layer started discarding data. Acking message as failed.");
			base.AckMessage(AckStatus.Fail, SmtpResponse.ProxyDiscardingMessage);
			base.Disconnect();
		}

		private void WaitForBdatCommandComplete(bool commandReceived)
		{
			if (commandReceived)
			{
				base.NextState = SmtpOutSession.SessionState.Bdat;
				this.WaitingForNextProxiedBdat = false;
			}
			else
			{
				this.logSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Proxy layer started discarding data. Acking message as failed.");
				base.AckMessage(AckStatus.Fail, SmtpResponse.ProxyDiscardingMessage);
				base.NextState = SmtpOutSession.SessionState.Quit;
			}
			base.FinalizeNextStateAndSendCommands();
		}

		private static readonly AsyncCallback WriteProxiedBytesToTargetCompleteCallback = new AsyncCallback(InboundProxySmtpOutSession.WriteProxiedBytesToTargetComplete);

		private static readonly AsyncCallback WriteBdatCompleteSendBuffersCallback = new AsyncCallback(InboundProxySmtpOutSession.WriteBdatCompleteSendBuffers);

		private readonly InboundProxyLayer.ReadCompletionCallback ReadFromProxyLayerCompleteCallback;

		private readonly InboundBdatProxyLayer.CommandReceivedCallback WaitForNextBdatCompleteCallback;

		private IInboundProxyLayer proxyLayer;

		private bool eodSeen;

		private bool waitingForNextProxiedBdat;

		private bool proxyingMessageBody;
	}
}
