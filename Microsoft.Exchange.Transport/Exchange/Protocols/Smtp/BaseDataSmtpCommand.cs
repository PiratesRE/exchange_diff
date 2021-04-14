using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MessageSecurity.MessageClassifications;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal abstract class BaseDataSmtpCommand : SmtpCommand
	{
		public BaseDataSmtpCommand(ISmtpSession session, string protocolCommandKeyword, string commandEventString, LatencyComponent commandEventComponent, ITransportAppConfig transportAppConfig) : base(session, protocolCommandKeyword, commandEventString, commandEventComponent)
		{
			DataCommandEventArgs dataCommandEventArgs = new DataCommandEventArgs();
			this.CommandEventArgs = dataCommandEventArgs;
			this.transportAppConfig = transportAppConfig;
			this.smtpCustomDataResponse = this.transportAppConfig.SmtpDataConfiguration.SmtpDataResponse;
			ISmtpInSession smtpInSession = session as ISmtpInSession;
			if (smtpInSession != null)
			{
				dataCommandEventArgs.MailItem = smtpInSession.TransportMailItemWrapper;
				this.messageSizeLimit = (long)smtpInSession.Connector.MaxMessageSize.ToBytes();
			}
			this.agentLoopChecker = new AgentGeneratedMessageLoopChecker(new AgentGeneratedMessageLoopCheckerTransportConfig(Components.Configuration));
		}

		internal BaseDataSmtpCommand(string protocolCommandKeyword) : base(protocolCommandKeyword)
		{
		}

		public virtual bool IsBlob
		{
			get
			{
				return false;
			}
		}

		internal Stream BodyStream
		{
			get
			{
				return this.bodyStream;
			}
		}

		protected internal bool DiscardingMessage
		{
			get
			{
				return this.discardingMessage;
			}
		}

		protected abstract SmtpInParser SmtpInParser { get; }

		protected abstract long AccumulatedMessageSize { get; }

		internal override void InboundParseCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			this.InboundParseCommandInternal();
			if (base.ParsingStatus != ParsingStatus.Error && base.ParsingStatus != ParsingStatus.ProtocolError)
			{
				if (smtpInSession.ShadowSession != null)
				{
					if (this.IsBlob)
					{
						this.shadowSession = new NullShadowSession();
					}
					else
					{
						this.shadowSession = smtpInSession.ShadowSession;
					}
				}
				else
				{
					if (smtpInSession.ShadowRedundancyManagerObject != null && !this.DiscardingMessage && !this.IsBlob)
					{
						this.shadowSession = smtpInSession.ShadowRedundancyManagerObject.GetShadowSession(smtpInSession, this is BdatSmtpCommand);
					}
					else
					{
						this.shadowSession = new NullShadowSession();
					}
					this.shadowSession.BeginOpen(smtpInSession.TransportMailItem, BaseDataSmtpCommand.shadowOpenCallback, this);
					if (!this.IsBlob)
					{
						smtpInSession.ShadowSession = this.shadowSession;
					}
				}
				this.shadowSession.PrepareForNewCommand(this);
			}
		}

		protected abstract AsyncReturnType SubmitMessageIfReady();

		protected abstract void ContinueSubmitMessageIfReady();

		protected virtual bool SetSuccessResponse()
		{
			if (!this.discardingMessage)
			{
				ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
				if (smtpInSession.IsPeerShadowSession)
				{
					base.SmtpResponse = SmtpResponse.QueuedMailForRedundancy(SmtpCommand.GetBracketedString(this.messageId));
				}
				else
				{
					base.SmtpResponse = SmtpResponse.QueuedMailForDelivery(SmtpCommand.GetBracketedString(this.messageId));
				}
				smtpInSession.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.SuccessfulSubmission);
			}
			return !this.discardingMessage;
		}

		protected abstract void StoreCurrentDataState();

		protected abstract void InboundParseCommandInternal();

		protected AsyncReturnType EodDone(bool isAsync)
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "EodDone");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (isAsync)
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.EodDoneAsync);
			}
			else
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.EodDoneSync);
			}
			if (base.SmtpResponse.SmtpResponseType != SmtpResponseType.Success)
			{
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveMessageRejected, null, new object[]
				{
					smtpInSession.Connector.Name,
					base.SmtpResponse
				});
			}
			base.ParsingStatus = ParsingStatus.Complete;
			if (this.shouldDisconnect && !smtpInSession.SessionSource.ShouldDisconnect)
			{
				smtpInSession.Disconnect(DisconnectReason.DroppedSession);
			}
			if (isAsync)
			{
				smtpInSession.RawDataReceivedCompleted();
			}
			if (!isAsync)
			{
				return AsyncReturnType.Sync;
			}
			return AsyncReturnType.Async;
		}

		protected bool FoundEndOfMessage()
		{
			return this.seenEod && this.isLastChunk;
		}

		protected AsyncReturnType SubmitMessage()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.SubmitMessage);
			SmtpResponse failureSmtpResponse;
			if (smtpInSession.ShouldRejectMailItem(smtpInSession.TransportMailItem.From, true, out failureSmtpResponse))
			{
				this.HandleSubmitFailure(failureSmtpResponse);
				return AsyncReturnType.Sync;
			}
			smtpInSession.CloseMessageWriteStream();
			if (smtpInSession.IsPeerShadowSession && smtpInSession.TransportMailItem.Priority == DeliveryPriority.Normal)
			{
				smtpInSession.TransportMailItem.PrioritizationReason = "ShadowRedundancy";
				smtpInSession.TransportMailItem.Priority = DeliveryPriority.None;
			}
			LatencyComponent component = (smtpInSession.SessionSource.IsInboundProxiedSession || smtpInSession.SessionSource.IsClientProxiedSession) ? LatencyComponent.SmtpReceiveDataExternal : LatencyComponent.SmtpReceiveDataInternal;
			LatencyTracker.EndTrackLatency(component, smtpInSession.TransportMailItem.LatencyTracker);
			LatencyTracker.BeginTrackLatency(LatencyComponent.SmtpReceiveCommit, smtpInSession.TransportMailItem.LatencyTracker);
			this.commitCoordinator = new CommitCoordinator(smtpInSession.SmtpInServer.MailItemStorage, this.shadowSession, Components.Configuration.ProcessTransportRole);
			this.commitCoordinator.BeginCommitMailItem(smtpInSession.TransportMailItem, BaseDataSmtpCommand.commitCallback, this);
			return AsyncReturnType.Async;
		}

		protected bool SetupMessageStream(bool expectBinaryContent)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			bool result = false;
			if (!this.isFirstChunk)
			{
				this.bodyStream = smtpInSession.MessageWriteStream;
				return true;
			}
			try
			{
				this.bodyStream = smtpInSession.OpenMessageWriteStream(expectBinaryContent);
				result = true;
			}
			catch (IOException arg)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceError<IOException>((long)this.GetHashCode(), "OpenMessageWriteStream failed: {0}", arg);
				smtpInSession.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
				this.bodyStream = null;
				this.StartDiscardingMessage();
				base.SmtpResponse = SmtpResponse.DataTransactionFailed;
				smtpInSession.DeleteTransportMailItem();
			}
			return result;
		}

		protected void StartDiscardingMessage()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.DiscardingMessage);
			this.discardingMessage = true;
			if (this.SmtpInParser != null)
			{
				this.SmtpInParser.IsDiscardingData = true;
			}
			if (smtpInSession.MimeDocument != null)
			{
				smtpInSession.MimeDocument.EndOfHeaders = null;
			}
			if (this.shadowSession != null)
			{
				this.shadowSession.Close(AckStatus.Fail, SmtpResponse.Empty);
			}
			this.StoreCurrentDataState();
		}

		protected void ParserEndOfHeadersCallback(MimePart part, out bool stopLoading)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.ParserEndOfHeadersCallback);
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "ParserEndOfHeadersCallback");
			if (this.IsBlob)
			{
				throw new InvalidOperationException("ParserEndOfHeadersCallback should not be called for BDAT blob.");
			}
			stopLoading = false;
			smtpInSession.MimeDocument.EndOfHeaders = null;
			if (this.seenEoh)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceError((long)this.GetHashCode(), "ParserEndOfHeadersCallback got called again");
				return;
			}
			this.seenEoh = true;
			if (this.discardingMessage)
			{
				return;
			}
			if (part == null)
			{
				ExTraceGlobals.SmtpReceiveTracer.TraceError((long)this.GetHashCode(), "MimePart is null");
				this.StartDiscardingMessage();
				return;
			}
			if (this.CheckPoisonMessage(part.Headers, smtpInSession))
			{
				this.StartDiscardingMessage();
				return;
			}
			int num;
			if (!DataBdatHelpers.CheckHeaders(part.Headers, smtpInSession, this.SmtpInParser.EohPos, this) || !DataBdatHelpers.CheckMaxHopCounts(part.Headers, smtpInSession, this, this.transportAppConfig.Routing.LocalLoopDetectionEnabled, this.transportAppConfig.Routing.LocalLoopSubdomainDepth, this.transportAppConfig.Routing.LocalLoopMessageDeferralIntervals, out num))
			{
				this.StartDiscardingMessage();
				return;
			}
			DataBdatHelpers.UpdateDagSelectorPerformanceCounters(part.Headers, this.transportAppConfig.Routing.CheckDagSelectorHeader, smtpInSession.SmtpReceivePerformanceCounters);
			RestrictedHeaderSet blocked = SmtpInSessionUtils.RestrictedHeaderSetFromPermissions(smtpInSession.Permissions);
			HeaderFirewall.Filter(smtpInSession.TransportMailItem.RootPart.Headers, blocked);
			DataBdatHelpers.PatchHeaders(part.Headers, smtpInSession, this.transportAppConfig.SmtpDataConfiguration.AcceptAndFixSmtpAddressWithInvalidLocalPart, out this.messageId);
			smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "receiving message with InternetMessageId {0}", new object[]
			{
				this.messageId
			});
			if (smtpInSession.TransportMailItem != null)
			{
				smtpInSession.TransportMailItem.ExposeMessageHeaders = true;
				smtpInSession.TransportMailItem.InternetMessageId = this.messageId;
				smtpInSession.SetupPoisonContext();
				smtpInSession.TransportMailItem.SetMimeDefaultEncoding();
				if (!this.CheckAttributionAndCreateADRecipientCache(smtpInSession))
				{
					this.StartDiscardingMessage();
					return;
				}
				if (num > 0)
				{
					smtpInSession.TransportMailItem.DeferUntil = DateTime.UtcNow.AddSeconds((double)num);
					smtpInSession.TransportMailItem.DeferReason = DeferReason.LoopDetected;
					LatencyTracker.BeginTrackLatency(LatencyComponent.Deferral, smtpInSession.TransportMailItem.LatencyTracker);
					MessageTrackingLog.TrackDefer(MessageTrackingSource.SMTP, smtpInSession.TransportMailItem, null);
				}
				if (!DataBdatHelpers.CheckMessageSubmitPermissions(smtpInSession, this))
				{
					this.StartDiscardingMessage();
					return;
				}
				if (!SmtpInSessionUtils.HasSMTPAcceptAnySenderPermission(smtpInSession.Permissions) || !SmtpInSessionUtils.HasSMTPAcceptAuthoritativeDomainSenderPermission(smtpInSession.Permissions))
				{
					if (!this.P1ChecksPass())
					{
						return;
					}
					if (!this.P2ChecksPass())
					{
						return;
					}
				}
				if (!SmtpInSessionUtils.HasSMTPBypassMessageSizeLimitPermission(smtpInSession.Permissions))
				{
					long num2;
					if (SmtpInSessionUtils.HasSMTPAcceptOrgHeadersPermission(smtpInSession.Permissions) && DataBdatHelpers.TryGetOriginalSize(part.Headers, out num2))
					{
						this.originalMessageSize = num2;
					}
					if (smtpInSession.SmtpInServer.IsBridgehead && !SmtpInSessionUtils.IsAnonymous(smtpInSession.RemoteIdentity))
					{
						try
						{
							long num3;
							if (BaseDataSmtpCommand.GetSenderSizeLimit(smtpInSession, out num3))
							{
								this.messageSizeLimit = num3;
							}
						}
						catch (ADTransientException)
						{
							ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)smtpInSession.GetHashCode(), "SMTP rejected a mail due to a transient AD error");
							smtpInSession.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToADDown);
							base.SmtpResponse = SmtpResponse.DataTransactionFailed;
							base.IsResponseReady = false;
							this.StartDiscardingMessage();
							return;
						}
					}
					if (!base.OnlyCheckMessageSizeAfterEoh || !DataBdatHelpers.MessageSizeExceeded(this.AccumulatedMessageSize, this.originalMessageSize, this.messageSizeLimit, smtpInSession.Permissions))
					{
						goto IL_360;
					}
					base.SmtpResponse = SmtpResponse.MessageTooLarge;
					base.IsResponseReady = false;
					this.StartDiscardingMessage();
					if (smtpInSession.SmtpReceivePerformanceCounters != null)
					{
						smtpInSession.SmtpReceivePerformanceCounters.MessagesRefusedForSize.Increment();
					}
					return;
				}
				IL_360:
				if (!this.CheckSubmissionQuota())
				{
					return;
				}
				this.SetOorg();
			}
			DataBdatHelpers.EnableEOHEvent(smtpInSession, smtpInSession.TransportMailItem.RootPart.Headers, out this.eohEventArgs);
		}

		protected void EnableEODEvent()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (smtpInSession.TransportMailItem.IsActive)
			{
				this.eodEventArgs = new EndOfDataEventArgs(smtpInSession.SessionSource);
				this.eodEventArgs.MailItem = smtpInSession.TransportMailItemWrapper;
			}
		}

		protected AsyncReturnType OnEod(bool isAsync)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (isAsync)
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.OnEodAsync);
			}
			else
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.OnEodSync);
			}
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "OnEod");
			if (smtpInSession.TransportMailItem != null)
			{
				smtpInSession.TransportMailItem.MimeSize += this.SmtpInParser.TotalBytesWritten;
			}
			if (this.discardingMessage || !this.isLastChunk)
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.RaisingOnRejectIfNecessary1);
				return this.RaiseOnRejectIfNecessary(isAsync);
			}
			try
			{
				if (this.bodyStream != null)
				{
					this.bodyStream.Close();
					this.bodyStream = null;
				}
			}
			catch (IOException e)
			{
				smtpInSession.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
				this.bodyStream = null;
				this.DiscardOnException(e);
			}
			catch (ExchangeDataException e2)
			{
				this.bodyStream = null;
				this.DiscardOnException(e2);
			}
			smtpInSession.TransportMailItem.ExposeMessage = true;
			if (!this.discardingMessage && smtpInSession.TransportMailItem != null && smtpInSession.TransportMailItem.RootPart != null && !this.SendOnBehalfOfChecksPass())
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.RaisingOnRejectIfNecessary2);
				return this.RaiseOnRejectIfNecessary(isAsync);
			}
			if (this.discardingMessage || !this.isLastChunk)
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.RaisingOnRejectIfNecessary3);
				return this.RaiseOnRejectIfNecessary(isAsync);
			}
			if (this is DataSmtpCommand && (smtpInSession.TransportMailItem.MimeDocument.ComplianceStatus & MimeComplianceStatus.BareLinefeedInBody) != MimeComplianceStatus.Compliant)
			{
				if (smtpInSession.SmtpReceivePerformanceCounters != null)
				{
					smtpInSession.SmtpReceivePerformanceCounters.MessagesReceivedWithBareLinefeeds.Increment();
				}
				if (smtpInSession.Connector.BareLinefeedRejectionEnabled)
				{
					using (BareLinefeedDetector bareLinefeedDetector = new BareLinefeedDetector())
					{
						try
						{
							smtpInSession.TransportMailItem.MimeDocument.WriteTo(bareLinefeedDetector);
						}
						catch (BareLinefeedException)
						{
							if (smtpInSession.SmtpReceivePerformanceCounters != null)
							{
								smtpInSession.SmtpReceivePerformanceCounters.MessagesRefusedForBareLinefeeds.Increment();
							}
							smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.RaisingOnRejectIfNecessary7);
							this.HandleSubmitFailure(SmtpResponse.InvalidContentBareLinefeeds);
							return this.RaiseOnRejectIfNecessary(isAsync);
						}
					}
				}
			}
			try
			{
				smtpInSession.TransportMailItem.Message.Normalize(NormalizeOptions.NormalizeMessageId | NormalizeOptions.MergeAddressHeaders | NormalizeOptions.RemoveDuplicateHeaders, false);
			}
			catch (IOException e3)
			{
				smtpInSession.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
				this.DiscardOnException(e3);
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.RaisingOnRejectIfNecessary6);
				return this.RaiseOnRejectIfNecessary(isAsync);
			}
			catch (ExchangeDataException e4)
			{
				this.DiscardOnException(e4);
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.RaisingOnRejectIfNecessary4);
				return this.RaiseOnRejectIfNecessary(isAsync);
			}
			smtpInSession.TransportMailItem.UpdateCachedHeaders();
			if (!this.PriorityMessageChecksPass())
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.RaisingOnRejectIfNecessary5);
				return this.RaiseOnRejectIfNecessary(isAsync);
			}
			this.messageId = smtpInSession.TransportMailItem.InternetMessageId;
			if (Components.Configuration.ProcessTransportRole == ProcessTransportRole.Hub)
			{
				bool flag = this.agentLoopChecker.IsEnabledInSmtp();
				bool flag2 = this.agentLoopChecker.CheckInSmtp(smtpInSession.TransportMailItem.RootPart.Headers);
				if (flag2)
				{
					MessageTrackingLog.TrackAgentGeneratedMessageRejected(MessageTrackingSource.SMTP, flag, smtpInSession.TransportMailItem);
					if (flag)
					{
						base.SmtpResponse = SmtpResponse.AgentGeneratedMessageDepthExceeded;
						this.StartDiscardingMessage();
					}
				}
			}
			this.EnableEODEvent();
			IAsyncResult asyncResult = this.RaiseEODEvent(null);
			if (!asyncResult.CompletedSynchronously)
			{
				return AsyncReturnType.Async;
			}
			return this.ContinueEndOfData(asyncResult, isAsync);
		}

		protected virtual AsyncReturnType RawDataReceived(byte[] data, int offset, int numBytes)
		{
			if (this.IsBlob)
			{
				throw new InvalidOperationException("RawDataReceived should not be called for BDAT blob.");
			}
			AsyncReturnType asyncReturnType = AsyncReturnType.Sync;
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<int>((long)this.GetHashCode(), "RawDataReceived received {0} bytes", numBytes);
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			bool isDiscardingData = this.SmtpInParser.IsDiscardingData;
			int num;
			this.seenEod = this.SmtpInParser.Write(data, offset, numBytes, out num);
			if (numBytes != num)
			{
				smtpInSession.PutBackReceivedBytes(numBytes - num);
			}
			if (!isDiscardingData && this.SmtpInParser.IsDiscardingData)
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
			this.shadowSession.BeginWrite(data, offset, num, this.seenEod, BaseDataSmtpCommand.shadowWriteCallback, this);
			if (this.FoundEndOfMessage())
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.RawDataFoundEndOfMessage);
				if (!this.discardingMessage)
				{
					try
					{
						if (!this.seenEoh)
						{
							this.bodyStream.Write(data, offset, 0);
							this.bodyStream.Flush();
						}
					}
					catch (IOException ex)
					{
						ExTraceGlobals.SmtpReceiveTracer.TraceDebug<IOException>((long)this.GetHashCode(), "Handled IO exception: {0}", ex);
						smtpInSession.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
						this.DiscardOnException(ex);
					}
					catch (ExchangeDataException ex2)
					{
						ExTraceGlobals.SmtpReceiveTracer.TraceDebug<ExchangeDataException>((long)this.GetHashCode(), "Handled parser exception: {0}", ex2);
						this.DiscardOnException(ex2);
					}
				}
			}
			if (this.eohEventArgs != null)
			{
				IAsyncResult asyncResult = DataBdatHelpers.RaiseEOHEvent(null, smtpInSession, new AsyncCallback(this.EndOfHeadersCallback), this.eohEventArgs);
				if (!asyncResult.CompletedSynchronously)
				{
					asyncReturnType = AsyncReturnType.Async;
				}
				else
				{
					asyncReturnType = this.ContinueEndOfHeaders(asyncResult);
				}
			}
			else if (this.seenEod)
			{
				asyncReturnType = this.OnEod(false);
			}
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<int, AsyncReturnType>((long)this.GetHashCode(), "RawDataReceived consumed {0} bytes, return returnType={1}", num, asyncReturnType);
			return asyncReturnType;
		}

		protected void EndOfHeadersCallback(IAsyncResult ar)
		{
			if (!ar.CompletedSynchronously)
			{
				this.ContinueEndOfHeaders(ar);
			}
		}

		protected void EndOfDataCallback(IAsyncResult ar)
		{
			if (!ar.CompletedSynchronously)
			{
				this.ContinueEndOfData(ar, true);
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

		protected virtual Stream GetFirewalledStream()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			Stream stream = null;
			Header header = null;
			RestrictedHeaderSet restrictedHeaderSet = smtpOutSession.RestrictedHeaderSet;
			if ((restrictedHeaderSet != RestrictedHeaderSet.None && HeaderFirewall.ContainsBlockedHeaders(smtpOutSession.RoutedMailItem.RootPart.Headers, restrictedHeaderSet)) || (!smtpOutSession.Connector.CloudServicesMailEnabled && smtpOutSession.NextHopType.IsSmtpConnectorDeliveryType))
			{
				if (!smtpOutSession.Connector.CloudServicesMailEnabled && smtpOutSession.NextHopType.IsSmtpConnectorDeliveryType && HeaderFirewall.CrossPremisesHeadersPresent(smtpOutSession.RoutedMailItem.RootPart.Headers))
				{
					header = Header.Create("X-CrossPremisesHeadersFilteredBySendConnector");
					header.Value = HeaderFirewall.ComputerName;
				}
				stream = Streams.CreateTemporaryStorageStream();
				smtpOutSession.RoutedMailItem.RootPart.WriteTo(stream, null, new HeaderFirewall.OutputFilter(restrictedHeaderSet, smtpOutSession.NeedToDownConvertMIME, header));
				if (stream != null)
				{
					stream.Seek(0L, SeekOrigin.Begin);
				}
			}
			if (stream == null)
			{
				stream = smtpOutSession.RoutedMailItem.OpenMimeReadStream(smtpOutSession.NeedToDownConvertMIME);
			}
			return stream;
		}

		private static void CommitCallback(IAsyncResult asyncResult)
		{
			BaseDataSmtpCommand baseDataSmtpCommand = (BaseDataSmtpCommand)asyncResult.AsyncState;
			ISmtpInSession smtpInSession = (ISmtpInSession)baseDataSmtpCommand.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.CommitCallback);
			smtpInSession.SetupPoisonContext();
			if (baseDataSmtpCommand.SmtpResponse.StatusCode == "250" && baseDataSmtpCommand.SmtpResponse.EnhancedStatusCode == "2.6.0" && smtpInSession.TransportMailItem != null)
			{
				string recordId = smtpInSession.TransportMailItem.RecordId.ToString();
				if (smtpInSession.IsPeerShadowSession)
				{
					baseDataSmtpCommand.SmtpResponse = SmtpResponse.QueuedMailForRedundancy(SmtpCommand.GetBracketedString(baseDataSmtpCommand.messageId), recordId, smtpInSession.SmtpInServer.Name);
				}
				else
				{
					baseDataSmtpCommand.SmtpResponse = SmtpResponse.QueuedMailForDelivery(SmtpCommand.GetBracketedString(baseDataSmtpCommand.messageId), recordId, smtpInSession.SmtpInServer.Name, baseDataSmtpCommand.smtpCustomDataResponse);
				}
			}
			baseDataSmtpCommand.CommitMailItemCompleted(asyncResult);
			baseDataSmtpCommand.ContinueSubmitMessage();
		}

		private static void ShadowOpenCallback(IAsyncResult asyncResult)
		{
			BaseDataSmtpCommand baseDataSmtpCommand = (BaseDataSmtpCommand)asyncResult.AsyncState;
			if (!baseDataSmtpCommand.shadowSession.EndOpen(asyncResult))
			{
				baseDataSmtpCommand.shadowSession.Close(AckStatus.Fail, SmtpResponse.Empty);
			}
		}

		private static void ShadowWriteCallback(IAsyncResult asyncResult)
		{
			BaseDataSmtpCommand baseDataSmtpCommand = (BaseDataSmtpCommand)asyncResult.AsyncState;
			if (!baseDataSmtpCommand.shadowSession.EndWrite(asyncResult))
			{
				baseDataSmtpCommand.shadowSession.Close(AckStatus.Fail, SmtpResponse.Empty);
			}
		}

		private bool CheckPoisonMessage(HeaderList headers, ISmtpInSession session)
		{
			Header header = headers.FindFirst("Message-ID");
			if (header == null)
			{
				return false;
			}
			string value = header.Value;
			if (!string.IsNullOrEmpty(value) && PoisonMessage.IsMessagePoison(value))
			{
				base.SmtpResponse = SmtpResponse.TooManyRelatedErrors;
				session.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Rejecting Message-ID: {0} because it was detected as poison", new object[]
				{
					value
				});
				return true;
			}
			return false;
		}

		private AsyncReturnType RaiseOnRejectIfNecessary(bool isAsync)
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "RaiseOnRejectIfNecessary");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (isAsync)
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.RaiseOnRejectIfNecessaryAsync);
			}
			else
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.RaiseOnRejectIfNecessarySync);
			}
			if (!this.discardingMessage || this.shouldDisconnect || base.SmtpResponse.SmtpResponseType == SmtpResponseType.Success)
			{
				return this.FinishEodSequence(isAsync);
			}
			IAsyncResult asyncResult = smtpInSession.RaiseOnRejectEvent(null, this.originalEventArgs, base.SmtpResponse, new AsyncCallback(this.OnRejectCallback));
			if (!asyncResult.CompletedSynchronously)
			{
				return AsyncReturnType.Async;
			}
			return this.ContinueOnReject(asyncResult, isAsync);
		}

		private void OnRejectCallback(IAsyncResult ar)
		{
			if (!ar.CompletedSynchronously)
			{
				this.ContinueOnReject(ar, true);
			}
		}

		private AsyncReturnType ContinueOnReject(IAsyncResult ar, bool isAsync)
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "OnRejectCallback");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.ContinueOnReject);
			this.ProcessAgentResponse(ar, null);
			return this.FinishEodSequence(isAsync);
		}

		private AsyncReturnType FinishEodSequence(bool isAsync)
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "FinishEodSequence");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (isAsync)
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.FinishEodSequenceAsync);
			}
			else
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.FinishEodSequenceSync);
			}
			if (base.SmtpResponse.Equals(SmtpResponse.Empty))
			{
				this.SetSuccessResponse();
				if (base.SmtpResponse.Equals(SmtpResponse.Empty))
				{
					base.SmtpResponse = SmtpResponse.DataTransactionFailed;
				}
			}
			base.IsResponseReady = true;
			if (this.SubmitMessageIfReady() == AsyncReturnType.Sync)
			{
				return this.EodDone(isAsync);
			}
			return AsyncReturnType.Async;
		}

		private void CommitMailItemCompleted(IAsyncResult asyncResult)
		{
			SmtpResponse failureSmtpResponse;
			SmtpResponse smtpResponse;
			if (!this.GetCommitResults(asyncResult, out failureSmtpResponse, out smtpResponse))
			{
				this.HandleSubmitFailure(failureSmtpResponse);
				return;
			}
			if (Components.Configuration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery && !smtpResponse.Equals(SmtpResponse.Empty))
			{
				base.SmtpResponse = smtpResponse;
			}
		}

		private void HandleSubmitFailure(SmtpResponse failureSmtpResponse)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.SubmitMessageFailed);
			if (failureSmtpResponse.Equals(SmtpResponse.Empty))
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.SubmitMessageFailedNoResponse);
			}
			base.SmtpResponse = failureSmtpResponse;
			this.StartDiscardingMessage();
		}

		private bool GetCommitResults(IAsyncResult asyncResult, out SmtpResponse failureSmtpResponse, out SmtpResponse enqueueMailItemResponse)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.GetCommitResults);
			bool flag = false;
			failureSmtpResponse = SmtpResponse.Empty;
			enqueueMailItemResponse = SmtpResponse.Empty;
			try
			{
				Exception ex = null;
				if (!this.commitCoordinator.EndCommitMailItem(asyncResult, out failureSmtpResponse, out ex))
				{
					if (ex is ExchangeDataException)
					{
						ExTraceGlobals.SmtpReceiveTracer.TraceError<string>((long)this.GetHashCode(), "GetCommitResults: MIME exception on commit: {0}", ex.Message);
						failureSmtpResponse = SmtpResponse.InvalidContent;
						smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "MIME exception on commit: {0}", new object[]
						{
							ex.Message
						});
						return false;
					}
					if (ex is IOException && ExceptionHelper.IsHandleableTransientCtsException(ex))
					{
						ExTraceGlobals.SmtpReceiveTracer.TraceError<string>((long)this.GetHashCode(), "GetCommitResults: IO exception on commit: {0}", ex.Message);
						smtpInSession.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
						failureSmtpResponse = SmtpResponse.CTSParseError;
						byte[] data = Util.AsciiStringToBytes(string.Format(CultureInfo.InvariantCulture, "IO Exception on commit: {0}", new object[]
						{
							ex.Message
						}));
						smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, data, null);
						return false;
					}
					if (ex == null)
					{
						ExTraceGlobals.SmtpReceiveTracer.TraceError<string>((long)this.GetHashCode(), "GetCommitResults: Unknown error, returning {0}", failureSmtpResponse.ToString());
						smtpInSession.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, Util.AsciiStringToBytes("Unknown error on commit"), null);
						return false;
					}
					throw new LocalizedException(Strings.CommitMailFailed, ex);
				}
				else
				{
					LatencyTracker.EndTrackLatency(LatencyComponent.SmtpReceiveCommit, smtpInSession.TransportMailItem.LatencyTracker);
					if (smtpInSession.IsPeerShadowSession)
					{
						smtpInSession.TrackAndEnqueuePeerShadowMailItem();
					}
					else
					{
						enqueueMailItemResponse = smtpInSession.TrackAndEnqueueMailItem();
					}
					flag = true;
				}
			}
			catch (IOException ex2)
			{
				SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveRejectDueToStorageError, null, new object[]
				{
					smtpInSession.Connector.Name,
					ex2.Message
				});
				smtpInSession.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
				failureSmtpResponse = SmtpResponse.DataTransactionFailed;
			}
			finally
			{
				if (!flag)
				{
					smtpInSession.DeleteTransportMailItem();
				}
				smtpInSession.ReleaseMailItem();
			}
			return flag;
		}

		private void ContinueSubmitMessage()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.ContinueSubmitMessage);
			this.ContinueSubmitMessageIfReady();
		}

		private static bool GetSenderSizeLimit(ISmtpInSession session, out long limit)
		{
			limit = 0L;
			RoutingAddress outer;
			if (!Util.TryGetP2Sender(session.TransportMailItem.RootPart.Headers, out outer))
			{
				return false;
			}
			ProxyAddress innermostAddress = Sender.GetInnermostAddress(outer);
			Result<TransportMiniRecipient> result = session.TransportMailItem.ADRecipientCache.FindAndCacheRecipient(innermostAddress);
			if (result.Data == null)
			{
				return false;
			}
			Unlimited<ByteQuantifiedSize> maxSendSize = result.Data.MaxSendSize;
			if (maxSendSize.IsUnlimited)
			{
				maxSendSize = session.SmtpInServer.TransportSettings.MaxSendSize;
			}
			if (maxSendSize.IsUnlimited)
			{
				limit = long.MaxValue;
				return true;
			}
			limit = (long)maxSendSize.Value.ToBytes();
			return true;
		}

		private bool PriorityMessageChecksPass()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			SmtpResponse smtpResponse;
			if (!smtpInSession.TransportMailItem.ValidateDeliveryPriority(out smtpResponse))
			{
				base.SmtpResponse = smtpResponse;
				base.IsResponseReady = false;
				this.StartDiscardingMessage();
				return false;
			}
			return true;
		}

		private IAsyncResult RaiseEODEvent(object state)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.RaiseEODEvent);
			if (smtpInSession.TransportMailItem != null)
			{
				smtpInSession.AgentLatencyTracker.BeginTrackLatency(LatencyComponent.SmtpReceiveOnEndOfData, smtpInSession.TransportMailItem.LatencyTracker);
			}
			return smtpInSession.AgentSession.BeginRaiseEvent("OnEndOfData", ReceiveMessageEventSourceImpl.Create(smtpInSession.SessionSource, this.eodEventArgs.MailItem), this.eodEventArgs, new AsyncCallback(this.EndOfDataCallback), state);
		}

		private AsyncReturnType ContinueEndOfHeaders(IAsyncResult ar)
		{
			bool flag = ar != null && !ar.CompletedSynchronously;
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<bool>((long)this.GetHashCode(), "EndOfHeadersCallback isAsync is {0}", flag);
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (flag)
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.ContinueEndOfHeadersAsync);
			}
			else
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.ContinueEndOfHeadersSync);
			}
			if (smtpInSession.TransportMailItem != null)
			{
				smtpInSession.AgentLatencyTracker.EndTrackLatency();
			}
			this.ProcessAgentResponse(ar, this.eohEventArgs);
			this.eohEventArgs = null;
			if (this.seenEod)
			{
				return this.OnEod(flag);
			}
			if (flag)
			{
				smtpInSession.RawDataReceivedCompleted();
			}
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug<bool>((long)this.GetHashCode(), "ContinueEndOfHeaders isAsync is {0}", flag);
			if (!flag)
			{
				return AsyncReturnType.Sync;
			}
			return AsyncReturnType.Async;
		}

		private AsyncReturnType ContinueEndOfData(IAsyncResult ar, bool isAsync)
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "EndOfDataCallback");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			ArgumentValidator.ThrowIfNull("session.TransportMailItem", smtpInSession.TransportMailItem);
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.ContinueEndOfData);
			smtpInSession.AgentLatencyTracker.EndTrackLatency();
			try
			{
				if (smtpInSession.TransportMailItem.Message.TnefPart != null)
				{
					Util.ConvertMessageClassificationsFromTnefToHeaders(smtpInSession.TransportMailItem);
				}
				ClassificationUtils.DropStoreLabels(smtpInSession.TransportMailItem.RootPart.Headers);
				this.StampOriginalMessageSize();
			}
			catch (IOException e)
			{
				smtpInSession.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
				this.DiscardOnException(e);
			}
			catch (ExchangeDataException e2)
			{
				this.DiscardOnException(e2);
			}
			this.ProcessAgentResponse(ar, this.eodEventArgs);
			this.eodEventArgs = null;
			return this.RaiseOnRejectIfNecessary(isAsync);
		}

		private void StampOriginalMessageSize()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			HeaderList headers = smtpInSession.TransportMailItem.RootPart.Headers;
			if (headers.FindFirst("X-MS-Exchange-Organization-OriginalSize") == null)
			{
				headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-OriginalSize", smtpInSession.TransportMailItem.MimeSize.ToString(NumberFormatInfo.InvariantInfo)));
			}
		}

		private void DiscardOnException(Exception e)
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceError<Exception>((long)this.GetHashCode(), "MessageWriteStream.Flush OR Close threw exception: {0}", e);
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.DiscardOnException);
			this.ParserException(e);
			this.StartDiscardingMessage();
		}

		private bool P1ChecksPass()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (smtpInSession.TransportMailItem.From == RoutingAddress.NullReversePath)
			{
				return true;
			}
			SmtpResponse smtpResponse;
			if (!BaseDataSmtpCommand.IsSenderOkForClient(smtpInSession, smtpInSession.TransportMailItem.From, true, out smtpResponse))
			{
				base.SmtpResponse = smtpResponse;
				base.IsResponseReady = false;
				this.StartDiscardingMessage();
				return false;
			}
			if (SmtpInAccessChecker.HasZeroProhibitSendQuota(smtpInSession, smtpInSession.TransportMailItem.ADRecipientCache, smtpInSession.TransportMailItem.From, out smtpResponse))
			{
				base.SmtpResponse = smtpResponse;
				base.IsResponseReady = false;
				this.StartDiscardingMessage();
				return false;
			}
			return true;
		}

		private bool P2ChecksPass()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			RoutingAddress routingAddress;
			Util.TryGetP2Sender(smtpInSession.TransportMailItem.RootPart.Headers, out routingAddress);
			if (!routingAddress.Equals(RoutingAddress.NullReversePath) && !routingAddress.Equals(RoutingAddress.Empty) && !routingAddress.Equals(smtpInSession.TransportMailItem.From))
			{
				SmtpResponse smtpResponse;
				if (!BaseDataSmtpCommand.IsSenderOkForClient(smtpInSession, routingAddress, false, out smtpResponse))
				{
					base.SmtpResponse = smtpResponse;
					base.IsResponseReady = false;
					this.StartDiscardingMessage();
					return false;
				}
				if (SmtpInAccessChecker.HasZeroProhibitSendQuota(smtpInSession, smtpInSession.TransportMailItem.ADRecipientCache, routingAddress, out smtpResponse))
				{
					base.SmtpResponse = smtpResponse;
					base.IsResponseReady = false;
					this.StartDiscardingMessage();
					return false;
				}
			}
			return true;
		}

		private bool SendOnBehalfOfChecksPass()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			HeaderList headers = smtpInSession.TransportMailItem.RootPart.Headers;
			RoutingAddress senderAddress = Util.RetrieveRoutingAddress(headers, HeaderId.Sender);
			RoutingAddress routingAddress = Util.RetrieveRoutingAddress(headers, HeaderId.From);
			SmtpResponse smtpResponse;
			if (!SmtpInAccessChecker.VerifySendOnBehalfOfPermissionsInAD(smtpInSession, smtpInSession.TransportMailItem.ADRecipientCache, senderAddress, routingAddress, out smtpResponse))
			{
				base.SmtpResponse = smtpResponse;
				base.IsResponseReady = false;
				this.StartDiscardingMessage();
				return false;
			}
			if (routingAddress.IsValid && routingAddress != RoutingAddress.NullReversePath && SmtpInAccessChecker.HasZeroProhibitSendQuota(smtpInSession, smtpInSession.TransportMailItem.ADRecipientCache, routingAddress, out smtpResponse))
			{
				base.SmtpResponse = smtpResponse;
				base.IsResponseReady = false;
				this.StartDiscardingMessage();
				return false;
			}
			return true;
		}

		private bool CheckSubmissionQuota()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (!smtpInSession.SmtpInServer.IsBridgehead)
			{
				return true;
			}
			if (!SubmissionQuotaChecker.CheckSubmissionQuota(SmtpInSessionState.FromSmtpInSession(smtpInSession)))
			{
				base.SmtpResponse = SmtpResponse.SubmissionQuotaExceeded;
				base.IsResponseReady = false;
				this.StartDiscardingMessage();
				return false;
			}
			return true;
		}

		private bool CheckAttributionAndCreateADRecipientCache(ISmtpInSession session)
		{
			SmtpResponse smtpResponse = DataBdatHelpers.CheckAttributionAndCreateAdRecipientCache(session.TransportMailItem, this.transportAppConfig.SmtpReceiveConfiguration.RejectUnscopedMessages, session.InboundClientProxyState != InboundClientProxyStates.None, false);
			if (!smtpResponse.Equals(SmtpResponse.Empty))
			{
				base.SmtpResponse = smtpResponse;
			}
			return smtpResponse.Equals(SmtpResponse.Empty);
		}

		private void ProcessAgentResponse(IAsyncResult ar, EventArgs args)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			SmtpResponse smtpResponse = smtpInSession.AgentSession.EndRaiseEvent(ar);
			this.shouldDisconnect = (!smtpResponse.IsEmpty || smtpInSession.SessionSource.ShouldDisconnect);
			if (this.shouldDisconnect)
			{
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.SessionDisconnectByAgent);
				if (!smtpResponse.IsEmpty)
				{
					base.SmtpResponse = smtpResponse;
				}
				this.StartDiscardingMessage();
				return;
			}
			if (args != null && !smtpInSession.SessionSource.SmtpResponse.Equals(SmtpResponse.Empty))
			{
				if (smtpInSession.SessionSource.SmtpResponse.SmtpResponseType == SmtpResponseType.Success)
				{
					smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.MessageDiscardedByAgent);
				}
				else
				{
					smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.MessageRejectedByAgent);
				}
				base.SmtpResponse = smtpInSession.SessionSource.SmtpResponse;
				this.originalEventArgs = args;
				this.StartDiscardingMessage();
				smtpInSession.SessionSource.SmtpResponse = SmtpResponse.Empty;
			}
		}

		private void SetOorg()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (!smtpInSession.TransportMailItem.ExposeMessageHeaders)
			{
				throw new InvalidOperationException("SetOorg() invoked on mail item that does not expose headers");
			}
			string oorg = DataBdatHelpers.GetOorg(smtpInSession.TransportMailItem, smtpInSession.Capabilities, smtpInSession.LogSession, smtpInSession.TransportMailItem.RootPart.Headers);
			smtpInSession.TransportMailItem.Oorg = oorg;
		}

		private static bool IsSenderOkForClient(ISmtpInSession session, RoutingAddress senderAddress, bool isMailFromSender, out SmtpResponse failureResponse)
		{
			return SmtpInAccessChecker.VerifySenderOkForClient(session, session.SmtpInServer.Configuration.GetAcceptedDomainTable(session.TransportMailItem.ADRecipientCache.OrganizationId), session.TransportMailItem.ADRecipientCache, session.SmtpInServer.IsBridgehead, senderAddress, session.RemoteWindowsIdentity, session.SmtpInServer.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName, isMailFromSender, out failureResponse);
		}

		internal const NormalizeOptions EndOfDataNormalizeOptions = NormalizeOptions.NormalizeMessageId | NormalizeOptions.MergeAddressHeaders | NormalizeOptions.RemoveDuplicateHeaders;

		protected long originalMessageSize = long.MaxValue;

		protected long messageSizeLimit;

		protected Stream bodyStream;

		protected bool seenEoh;

		protected bool discardingMessage;

		protected string messageId;

		protected bool isFirstChunk;

		protected bool isLastChunk;

		protected ITransportAppConfig transportAppConfig;

		private static readonly AsyncCallback commitCallback = new AsyncCallback(BaseDataSmtpCommand.CommitCallback);

		private static readonly AsyncCallback shadowWriteCallback = new AsyncCallback(BaseDataSmtpCommand.ShadowWriteCallback);

		private static readonly AsyncCallback shadowOpenCallback = new AsyncCallback(BaseDataSmtpCommand.ShadowOpenCallback);

		private readonly string smtpCustomDataResponse;

		private IShadowSession shadowSession;

		private CommitCoordinator commitCoordinator;

		private EndOfHeadersEventArgs eohEventArgs;

		private EndOfDataEventArgs eodEventArgs;

		private EventArgs originalEventArgs;

		private bool shouldDisconnect;

		private bool seenEod;

		private AgentGeneratedMessageLoopChecker agentLoopChecker;
	}
}
