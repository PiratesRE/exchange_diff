using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MessageSecurity.MessageClassifications;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal abstract class DataBdatCommandBase : SmtpInCommandBase
	{
		protected DataBdatCommandBase(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
			this.messageSizeLimit = (long)sessionState.ReceiveConnector.MaxMessageSize.ToBytes();
		}

		protected virtual long AccumulatedMessageSize
		{
			get
			{
				if (this.streamBuilder != null)
				{
					return this.streamBuilder.TotalBytesWritten;
				}
				return 0L;
			}
		}

		private protected bool IsEohSeen { protected get; private set; }

		protected abstract ParseResult PreProcess();

		protected abstract bool TryGetInitialResponse(out SmtpResponse initialResponse);

		protected abstract bool ValidateAccumulatedSize(out SmtpResponse failureResponse);

		protected abstract bool ShouldProcessEoh();

		protected abstract bool ShouldProcessEod();

		protected abstract void PostEoh();

		protected abstract void PostEod();

		protected abstract bool ShouldHandleBareLineFeedInBody();

		protected abstract ParseAndProcessResult<SmtpInStateMachineEvents> GetFinalResult(ParseAndProcessResult<SmtpInStateMachineEvents> eodResult);

		protected abstract ParseAndProcessResult<SmtpInStateMachineEvents> GetFailureResult(ParsingStatus parsingStatus, SmtpResponse failureResponse, out bool shouldAbortTransaction);

		protected abstract SmtpInStateMachineEvents GetSuccessEvent();

		protected override LatencyComponent LatencyComponent
		{
			get
			{
				return LatencyComponent.SmtpReceiveOnDataCommand;
			}
		}

		protected override async Task<ParseAndProcessResult<SmtpInStateMachineEvents>> ProcessAsync(CancellationToken cancellationToken)
		{
			this.sessionState.Tracer.TraceDebug((long)this.sessionState.GetHashCode(), "DataBdatCommandBase.ProcessAsync");
			SmtpResponse finalResponse = SmtpResponse.Empty;
			ParseResult preProcessResult = this.PreProcess();
			if (preProcessResult.IsFailed)
			{
				this.sessionState.StartDiscardingMessage();
				finalResponse = preProcessResult.SmtpResponse;
				if (preProcessResult.DisconnectClient)
				{
					return new ParseAndProcessResult<SmtpInStateMachineEvents>(preProcessResult.ParsingStatus, preProcessResult.SmtpResponse, SmtpInStateMachineEvents.SendResponseAndDisconnectClient, preProcessResult.DisconnectClient);
				}
			}
			SmtpResponse initialResponse;
			if (this.TryGetInitialResponse(out initialResponse))
			{
				this.sessionState.ProtocolLogSession.LogSend(initialResponse.ToByteArray());
				object error = await base.WriteToClientAsync(initialResponse);
				base.OnAwaitCompleted(cancellationToken);
				if (error != null)
				{
					this.sessionState.HandleNetworkError(error);
					return DataBdatCommandBase.NetworkError;
				}
			}
			CommandContext commandContext = null;
			MessageHandlerResult handlerResult = MessageHandlerResult.MoreDataRequired;
			ParseAndProcessResult<SmtpInStateMachineEvents> agentResult = SmtpInCommandBase.SmtpResponseEmptyResult;
			SmtpResponse failureResponse;
			while (handlerResult == MessageHandlerResult.MoreDataRequired)
			{
				NetworkConnection.LazyAsyncResultWithTimeout readResult = await Util.ReadAsync(this.sessionState);
				base.OnAwaitCompleted(cancellationToken);
				if (Util.IsReadFailure(readResult))
				{
					this.sessionState.HandleNetworkError(readResult.Result);
					return DataBdatCommandBase.NetworkError;
				}
				commandContext = CommandContext.FromAsyncResult(readResult);
				handlerResult = this.messageHandler.Process(commandContext, out failureResponse);
				if (handlerResult == MessageHandlerResult.Failure)
				{
					this.sessionState.StartDiscardingMessage();
					finalResponse = failureResponse;
				}
				if (!this.sessionState.DiscardingMessage)
				{
					if (!this.ValidateAccumulatedSize(out failureResponse))
					{
						this.sessionState.StartDiscardingMessage();
						finalResponse = failureResponse;
					}
					else
					{
						ParseAndProcessResult<SmtpInStateMachineEvents> eohResult = await this.HandleEohAsync(commandContext, cancellationToken);
						base.OnAwaitCompleted(cancellationToken);
						if (agentResult.SmtpResponse.IsEmpty && !eohResult.SmtpResponse.IsEmpty)
						{
							agentResult = eohResult;
							this.sessionState.StartDiscardingMessage();
						}
					}
				}
			}
			ParseAndProcessResult<SmtpInStateMachineEvents> result;
			if (!finalResponse.IsEmpty || !this.EohResponse.IsEmpty)
			{
				finalResponse = (finalResponse.IsEmpty ? this.EohResponse : finalResponse);
				result = await this.HandleCommandFailureAsync(commandContext, ParsingStatus.Complete, finalResponse, cancellationToken);
			}
			else if (!agentResult.SmtpResponse.IsEmpty)
			{
				this.HandleMessageRejection(agentResult.SmtpResponse);
				result = agentResult;
			}
			else if (!this.FlushContents(out failureResponse))
			{
				this.HandleMessageRejection(failureResponse);
				result = await this.HandleCommandFailureAsync(commandContext, ParsingStatus.Complete, failureResponse, cancellationToken);
			}
			else
			{
				agentResult = await this.HandleEohAsync(commandContext, cancellationToken);
				base.OnAwaitCompleted(cancellationToken);
				if (!agentResult.SmtpResponse.IsEmpty)
				{
					this.HandleMessageRejection(agentResult.SmtpResponse);
					result = agentResult;
				}
				else
				{
					ParseAndProcessResult<SmtpInStateMachineEvents> eodResult = await this.HandleEodAsync(commandContext, cancellationToken);
					base.OnAwaitCompleted(cancellationToken);
					if (DataBdatCommandBase.IsFailureResponseType(eodResult.SmtpResponse))
					{
						this.HandleMessageRejection(eodResult.SmtpResponse);
						result = eodResult;
					}
					else
					{
						ParseAndProcessResult<SmtpInStateMachineEvents> finalResult = this.GetFinalResult(eodResult);
						if (finalResult.SmtpResponse.SmtpResponseType != SmtpResponseType.Success)
						{
							this.HandleMessageRejection(finalResult.SmtpResponse);
							result = await this.HandleCommandFailureAsync(commandContext, finalResult.ParsingStatus, finalResult.SmtpResponse, cancellationToken);
						}
						else
						{
							this.PostEod();
							result = finalResult;
						}
					}
				}
			}
			return result;
		}

		protected void HandleEoh(MimePart part, out bool stopLoading)
		{
			stopLoading = false;
			if (!this.ShouldProcessEoh())
			{
				return;
			}
			this.sessionState.TransportMailItem.MimeDocument.EndOfHeaders = null;
			if (this.IsEohSeen)
			{
				this.sessionState.Tracer.TraceError((long)this.sessionState.GetHashCode(), "HandleEoh callback got called more than once");
				return;
			}
			this.IsEohSeen = true;
			SmtpResponse eohResponse;
			if (!this.ShouldContinueEndOfHeaders(part, out eohResponse))
			{
				if (!eohResponse.IsEmpty)
				{
					this.EohResponse = eohResponse;
				}
				return;
			}
			eohResponse = this.PerformHeaderValidation(part);
			if (!eohResponse.IsEmpty)
			{
				this.EohResponse = eohResponse;
				return;
			}
			DataBdatHelpers.UpdateDagSelectorPerformanceCounters(part.Headers, this.sessionState.Configuration.RoutingConfiguration.CheckDagSelectorHeader, this.sessionState.ReceivePerfCounters);
			this.sessionState.TransportMailItem.Oorg = DataBdatHelpers.GetOorg(this.sessionState.TransportMailItem, this.sessionState.Capabilities, this.sessionState.ProtocolLogSession, this.sessionState.TransportMailItem.RootPart.Headers);
			this.eohEventArgs = new EndOfHeadersEventArgs(this.sessionState)
			{
				MailItem = this.sessionState.TransportMailItemWrapper,
				Headers = this.sessionState.TransportMailItem.RootPart.Headers
			};
			this.PostEoh();
		}

		protected bool SetupMessageStream(bool allowBinaryContent)
		{
			Stream bodyStream;
			if (this.sessionState.SetupMessageStream(allowBinaryContent, out bodyStream))
			{
				this.streamBuilder.BodyStream = bodyStream;
				return true;
			}
			this.streamBuilder.IsDiscardingData = true;
			return false;
		}

		private async Task<ParseAndProcessResult<SmtpInStateMachineEvents>> HandleEodAsync(CommandContext commandContext, CancellationToken cancellationToken)
		{
			ParseAndProcessResult<SmtpInStateMachineEvents> result2;
			if (!this.ShouldProcessEod())
			{
				result2 = SmtpInCommandBase.SmtpResponseEmptyResult;
			}
			else
			{
				if (this.sessionState.TransportMailItem != null)
				{
					this.sessionState.TransportMailItem.MimeSize += this.streamBuilder.TotalBytesWritten;
				}
				SmtpResponse result;
				if (!this.CloseMessageStream(out result))
				{
					result2 = await this.RaiseOnRejectEventAsync(commandContext, ParsingStatus.Complete, result, null, cancellationToken);
				}
				else
				{
					this.sessionState.TransportMailItem.ExposeMessage = true;
					if (!this.sessionState.DiscardingMessage && this.sessionState.TransportMailItem != null && this.sessionState.TransportMailItem.RootPart != null && !DataBdatHelpers.SendOnBehalfOfChecksPass(this.sessionState, out result))
					{
						result2 = await this.RaiseOnRejectEventAsync(commandContext, ParsingStatus.Complete, result, null, cancellationToken);
					}
					else if ((this.ShouldHandleBareLineFeedInBody() && !DataBdatCommandBase.HandleBareLineFeedInBody(this.sessionState, out result)) || !this.NormalizeMime(out result))
					{
						result2 = await this.RaiseOnRejectEventAsync(commandContext, ParsingStatus.Complete, result, null, cancellationToken);
					}
					else
					{
						this.sessionState.TransportMailItem.UpdateCachedHeaders();
						if (!this.sessionState.IsValidMessagePriority(out result))
						{
							result2 = await this.RaiseOnRejectEventAsync(commandContext, ParsingStatus.Complete, result, null, cancellationToken);
						}
						else
						{
							this.eodEventArgs = new EndOfDataEventArgs(this.sessionState)
							{
								MailItem = this.sessionState.TransportMailItemWrapper
							};
							ParseAndProcessResult<SmtpInStateMachineEvents> eodResult = await this.RaiseEodEventAsync(commandContext, cancellationToken);
							if (eodResult.SmtpResponse.SmtpResponseType != SmtpResponseType.Success && !eodResult.SmtpResponse.IsEmpty)
							{
								result2 = eodResult;
							}
							else if (!this.ModifyTransportMailItemProperties(out result))
							{
								result2 = await this.RaiseOnRejectEventAsync(commandContext, ParsingStatus.Complete, result, this.eohEventArgs, cancellationToken);
							}
							else
							{
								result2 = await this.CommitMessageAsync(cancellationToken);
							}
						}
					}
				}
			}
			return result2;
		}

		private bool CheckPoisonMessage(HeaderList headers)
		{
			Header header = headers.FindFirst("Message-ID");
			if (header == null)
			{
				return false;
			}
			string value = header.Value;
			if (!string.IsNullOrEmpty(value) && this.sessionState.IsMessagePoison(value))
			{
				this.sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Rejecting Message-ID: {0} because it was detected as poison", new object[]
				{
					value
				});
				return true;
			}
			return false;
		}

		private bool FlushContents(out SmtpResponse failureResponse)
		{
			if (!this.ShouldProcessEod())
			{
				failureResponse = SmtpResponse.Empty;
				return true;
			}
			bool result;
			try
			{
				if (this.streamBuilder != null)
				{
					this.streamBuilder.BodyStream.Flush();
				}
				failureResponse = SmtpResponse.Empty;
				result = true;
			}
			catch (IOException ex)
			{
				this.sessionState.Tracer.TraceDebug<IOException>((long)this.sessionState.GetHashCode(), "Handled IO exception: {0}", ex);
				this.sessionState.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
				failureResponse = DataBdatHelpers.HandleFilterableException(ex, this.sessionState);
				result = false;
			}
			catch (ExchangeDataException ex2)
			{
				this.sessionState.Tracer.TraceDebug<ExchangeDataException>((long)this.sessionState.GetHashCode(), "Handled parser exception: {0}", ex2);
				failureResponse = DataBdatHelpers.HandleFilterableException(ex2, this.sessionState);
				result = false;
			}
			return result;
		}

		private bool ShouldContinueEndOfHeaders(MimePart part, out SmtpResponse failureResponse)
		{
			if (this.sessionState.DiscardingMessage)
			{
				failureResponse = SmtpResponse.Empty;
				return false;
			}
			if (part == null)
			{
				failureResponse = SmtpResponse.DataTransactionFailed;
				this.sessionState.Tracer.TraceError((long)this.sessionState.GetHashCode(), "MimePart is null");
				this.sessionState.StartDiscardingMessage();
				return false;
			}
			if (this.CheckPoisonMessage(part.Headers))
			{
				failureResponse = SmtpResponse.TooManyRelatedErrors;
				this.sessionState.StartDiscardingMessage();
				return false;
			}
			failureResponse = SmtpResponse.Empty;
			return true;
		}

		private SmtpResponse PerformSizeValidations(MimePart part)
		{
			if (SmtpInSessionUtils.HasSMTPBypassMessageSizeLimitPermission(this.sessionState.CombinedPermissions))
			{
				return SmtpResponse.Empty;
			}
			long num;
			if (SmtpInSessionUtils.HasSMTPAcceptOrgHeadersPermission(this.sessionState.CombinedPermissions) && DataBdatHelpers.TryGetOriginalSize(part.Headers, out num))
			{
				this.originalMessageSize = num;
			}
			SmtpResponse result;
			if (!this.TryUpdateSizeLimits(out result))
			{
				return result;
			}
			if (DataBdatHelpers.ShouldOnlyCheckMessageSizeAfterEoh(this.sessionState) && DataBdatHelpers.MessageSizeExceeded(this.AccumulatedMessageSize, this.originalMessageSize, this.messageSizeLimit, this.sessionState.CombinedPermissions))
			{
				this.sessionState.StartDiscardingMessage();
				if (this.sessionState.ReceivePerfCounters != null)
				{
					this.sessionState.ReceivePerfCounters.MessagesRefusedForSize.Increment();
				}
				return SmtpResponse.MessageTooLarge;
			}
			return SmtpResponse.Empty;
		}

		private bool TryUpdateSizeLimits(out SmtpResponse failureResponse)
		{
			if (!this.ShouldUpdateSizeLimits())
			{
				failureResponse = SmtpResponse.Empty;
				return true;
			}
			try
			{
				long num;
				if (DataBdatHelpers.GetSenderSizeLimit(this.sessionState.TransportMailItem, this.sessionState.Configuration.TransportConfiguration.MaxSendSize, out num))
				{
					this.messageSizeLimit = num;
				}
			}
			catch (ADTransientException)
			{
				this.sessionState.Tracer.TraceDebug((long)this.sessionState.GetHashCode(), "SMTP rejected a mail due to a transient AD error");
				this.sessionState.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToADDown);
				this.sessionState.StartDiscardingMessage();
				failureResponse = SmtpResponse.DataTransactionFailed;
				return false;
			}
			failureResponse = SmtpResponse.Empty;
			return true;
		}

		private bool ShouldUpdateSizeLimits()
		{
			return Util.IsHubOrFrontEndRole(this.sessionState.Configuration.TransportConfiguration.ProcessTransportRole) && !SmtpInSessionUtils.IsAnonymous(this.sessionState.RemoteIdentity);
		}

		private SmtpResponse PerformP1AndP2Checks()
		{
			if (this.CanP1AndP2ChecksBeBypassed(this.sessionState.CombinedPermissions))
			{
				return SmtpResponse.Empty;
			}
			SmtpResponse result = DataBdatHelpers.ValidateMailFromAddress(this.sessionState);
			if (!result.IsEmpty)
			{
				return result;
			}
			result = DataBdatHelpers.ValidateFromAddressInHeader(this.sessionState);
			if (!result.IsEmpty)
			{
				return result;
			}
			return SmtpResponse.Empty;
		}

		private bool CanP1AndP2ChecksBeBypassed(Permission permissions)
		{
			return SmtpInSessionUtils.HasSMTPAcceptAnySenderPermission(permissions) && SmtpInSessionUtils.HasSMTPAcceptAuthoritativeDomainSenderPermission(permissions);
		}

		private SmtpResponse UpdateTransportMailItem(SmtpInSessionState state, string internetMessageId, int localLoopDeferralSeconds)
		{
			state.TransportMailItem.ExposeMessageHeaders = true;
			state.TransportMailItem.InternetMessageId = internetMessageId;
			state.TransportMailItem.SetMimeDefaultEncoding();
			SmtpResponse result = DataBdatHelpers.CheckAttributionAndCreateAdRecipientCache(state.TransportMailItem, state.Configuration.TransportConfiguration.RejectUnscopedMessages, false, false);
			if (!result.IsEmpty)
			{
				state.StartDiscardingMessage();
				return result;
			}
			if (localLoopDeferralSeconds > 0)
			{
				state.TransportMailItem.DeferUntil = DateTime.UtcNow.AddSeconds((double)localLoopDeferralSeconds);
				state.TransportMailItem.DeferReason = DeferReason.LoopDetected;
				LatencyTracker.BeginTrackLatency(LatencyComponent.Deferral, state.TransportMailItem.LatencyTracker);
				MessageTrackingLog.TrackDefer(MessageTrackingSource.SMTP, state.TransportMailItem, null);
			}
			return SmtpResponse.Empty;
		}

		private void FilterAndPatchHeaders(MimePart part, out string messageId)
		{
			RestrictedHeaderSet blocked = SmtpInSessionUtils.RestrictedHeaderSetFromPermissions(this.sessionState.CombinedPermissions);
			HeaderFirewall.Filter(this.sessionState.TransportMailItem.RootPart.Headers, blocked);
			DataBdatHelpers.PatchHeaders(part.Headers, this.sessionState, out messageId);
			this.sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "receiving message with InternetMessageId {0}", new object[]
			{
				messageId
			});
		}

		private SmtpResponse PerformHeaderValidation(MimePart part)
		{
			int localLoopDeferralSeconds = 0;
			SmtpResponse result = DataBdatHelpers.PerformHeaderSizeAndPartialMessageChecks(part.Headers, this.sessionState, this.streamBuilder.EohPos);
			if (result.IsEmpty)
			{
				result = DataBdatHelpers.CheckMaxHopCounts(part.Headers, this.sessionState, out localLoopDeferralSeconds);
			}
			if (!result.IsEmpty)
			{
				this.sessionState.StartDiscardingMessage();
				return result;
			}
			string internetMessageId;
			this.FilterAndPatchHeaders(part, out internetMessageId);
			result = this.UpdateTransportMailItem(this.sessionState, internetMessageId, localLoopDeferralSeconds);
			if (!result.IsEmpty)
			{
				this.sessionState.StartDiscardingMessage();
				return result;
			}
			result = DataBdatHelpers.CheckMessageSubmitPermissions(this.sessionState);
			if (!result.IsEmpty)
			{
				this.sessionState.StartDiscardingMessage();
				return result;
			}
			result = this.PerformP1AndP2Checks();
			if (!result.IsEmpty)
			{
				this.sessionState.StartDiscardingMessage();
				return result;
			}
			result = this.PerformSizeValidations(part);
			if (!result.IsEmpty)
			{
				this.sessionState.StartDiscardingMessage();
				return result;
			}
			if (Util.IsHubOrFrontEndRole(this.sessionState.Configuration.TransportConfiguration.ProcessTransportRole) && !SubmissionQuotaChecker.CheckSubmissionQuota(this.sessionState))
			{
				this.sessionState.StartDiscardingMessage();
				return SmtpResponse.SubmissionQuotaExceeded;
			}
			return SmtpResponse.Empty;
		}

		private async Task<ParseAndProcessResult<SmtpInStateMachineEvents>> HandleEohAsync(CommandContext commandContext, CancellationToken cancellationToken)
		{
			ParseAndProcessResult<SmtpInStateMachineEvents> result;
			if (this.eohEventArgs == null || !this.EohResponse.IsEmpty)
			{
				result = SmtpInCommandBase.SmtpResponseEmptyResult;
			}
			else
			{
				this.sessionState.SmtpAgentSession.LatencyTracker.BeginTrackLatency(LatencyComponent.SmtpReceiveOnEndOfHeaders, this.sessionState.TransportMailItem.LatencyTracker);
				ParseAndProcessResult<SmtpInStateMachineEvents> endOfHeaderResult = await base.RaiseAgentEventAsync("OnEndOfHeaders", this.eohEventArgs, commandContext, ParseResult.ParsingComplete, cancellationToken, ReceiveMessageEventSourceImpl.Create(this.sessionState, this.eohEventArgs.MailItem));
				base.OnAwaitCompleted(cancellationToken);
				this.sessionState.SmtpAgentSession.LatencyTracker.EndTrackLatency();
				this.eohEventArgs = null;
				result = endOfHeaderResult;
			}
			return result;
		}

		private bool CloseMessageStream(out SmtpResponse failureResponse)
		{
			try
			{
				if (this.streamBuilder.BodyStream != null)
				{
					this.streamBuilder.BodyStream.Close();
					this.streamBuilder.BodyStream = null;
				}
			}
			catch (IOException exception)
			{
				this.sessionState.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
				this.streamBuilder.BodyStream = null;
				failureResponse = DataBdatHelpers.HandleFilterableException(exception, this.sessionState);
				return false;
			}
			catch (ExchangeDataException exception2)
			{
				this.streamBuilder.BodyStream = null;
				failureResponse = DataBdatHelpers.HandleFilterableException(exception2, this.sessionState);
				return false;
			}
			failureResponse = SmtpResponse.Empty;
			return true;
		}

		private static bool HandleBareLineFeedInBody(SmtpInSessionState sessionState, out SmtpResponse failureResponse)
		{
			if ((sessionState.TransportMailItem.MimeDocument.ComplianceStatus & MimeComplianceStatus.BareLinefeedInBody) != MimeComplianceStatus.Compliant)
			{
				if (sessionState.ReceivePerfCounters != null)
				{
					sessionState.ReceivePerfCounters.MessagesReceivedWithBareLinefeeds.Increment();
				}
				if (!sessionState.ReceiveConnector.BareLinefeedRejectionEnabled)
				{
					failureResponse = SmtpResponse.Empty;
					return true;
				}
				using (BareLinefeedDetector bareLinefeedDetector = new BareLinefeedDetector())
				{
					try
					{
						sessionState.TransportMailItem.MimeDocument.WriteTo(bareLinefeedDetector);
					}
					catch (BareLinefeedException)
					{
						if (sessionState.ReceivePerfCounters != null)
						{
							sessionState.ReceivePerfCounters.MessagesRefusedForBareLinefeeds.Increment();
						}
						failureResponse = SmtpResponse.InvalidContentBareLinefeeds;
						return false;
					}
				}
			}
			failureResponse = SmtpResponse.Empty;
			return true;
		}

		private bool NormalizeMime(out SmtpResponse failureResponse)
		{
			try
			{
				this.sessionState.TransportMailItem.Message.Normalize(NormalizeOptions.NormalizeMessageId | NormalizeOptions.MergeAddressHeaders | NormalizeOptions.RemoveDuplicateHeaders, false);
			}
			catch (IOException exception)
			{
				this.sessionState.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
				failureResponse = DataBdatHelpers.HandleFilterableException(exception, this.sessionState);
				return false;
			}
			catch (ExchangeDataException exception2)
			{
				failureResponse = DataBdatHelpers.HandleFilterableException(exception2, this.sessionState);
				return false;
			}
			failureResponse = SmtpResponse.Empty;
			return true;
		}

		private async Task<ParseAndProcessResult<SmtpInStateMachineEvents>> RaiseOnRejectEventAsync(CommandContext commandContext, ParsingStatus parsingStatus, SmtpResponse smtpResponse, ReceiveEventArgs eventArgs, CancellationToken cancellationToken)
		{
			this.sessionState.AbortMailTransaction();
			SmtpResponse agentResponse = await base.RaiseRejectEventAsync(commandContext, parsingStatus, smtpResponse, eventArgs, cancellationToken);
			return new ParseAndProcessResult<SmtpInStateMachineEvents>(parsingStatus, agentResponse, this.GetCommandFailureEvent(), false);
		}

		private async Task<ParseAndProcessResult<SmtpInStateMachineEvents>> RaiseEodEventAsync(CommandContext commandContext, CancellationToken cancellationToken)
		{
			this.sessionState.SmtpAgentSession.LatencyTracker.BeginTrackLatency(LatencyComponent.SmtpReceiveOnEndOfData, this.sessionState.TransportMailItem.LatencyTracker);
			ParseAndProcessResult<SmtpInStateMachineEvents> endOfDataResult = await base.RaiseAgentEventAsync("OnEndOfData", this.eodEventArgs, commandContext, ParseResult.ParsingComplete, cancellationToken, ReceiveMessageEventSourceImpl.Create(this.sessionState, this.eodEventArgs.MailItem));
			base.OnAwaitCompleted(cancellationToken);
			this.sessionState.SmtpAgentSession.LatencyTracker.EndTrackLatency();
			return endOfDataResult;
		}

		private void StampOriginalMessageSize()
		{
			HeaderList headers = this.sessionState.TransportMailItem.RootPart.Headers;
			if (headers.FindFirst("X-MS-Exchange-Organization-OriginalSize") == null)
			{
				headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-OriginalSize", this.sessionState.TransportMailItem.MimeSize.ToString(NumberFormatInfo.InvariantInfo)));
			}
		}

		private bool ModifyTransportMailItemProperties(out SmtpResponse failureResponse)
		{
			try
			{
				if (this.sessionState.TransportMailItem.Message.TnefPart != null)
				{
					Util.ConvertMessageClassificationsFromTnefToHeaders(this.sessionState.TransportMailItem);
				}
				ClassificationUtils.DropStoreLabels(this.sessionState.TransportMailItem.RootPart.Headers);
				this.StampOriginalMessageSize();
			}
			catch (IOException exception)
			{
				this.sessionState.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
				failureResponse = DataBdatHelpers.HandleFilterableException(exception, this.sessionState);
				return false;
			}
			catch (ExchangeDataException exception2)
			{
				failureResponse = DataBdatHelpers.HandleFilterableException(exception2, this.sessionState);
				return false;
			}
			failureResponse = SmtpResponse.Empty;
			return true;
		}

		private async Task<ParseAndProcessResult<SmtpInStateMachineEvents>> CommitMessageAsync(CancellationToken cancellationToken)
		{
			SmtpResponse failureResponse;
			bool shouldRejectMailItem = CommandParsingHelper.ShouldRejectMailItem(this.sessionState.TransportMailItem.From, this.sessionState, true, out failureResponse);
			if (failureResponse.Equals(SmtpResponse.InsufficientResource))
			{
				this.sessionState.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToBackPressure);
			}
			ParseAndProcessResult<SmtpInStateMachineEvents> result2;
			if (shouldRejectMailItem)
			{
				result2 = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, failureResponse, this.GetCommandFailureEvent(), false);
			}
			else
			{
				this.sessionState.CloseMessageWriteStream();
				if (!string.IsNullOrEmpty(this.sessionState.PeerSessionPrimaryServer) && this.sessionState.TransportMailItem.Priority == DeliveryPriority.Normal)
				{
					this.sessionState.TransportMailItem.PrioritizationReason = "ShadowRedundancy";
					this.sessionState.TransportMailItem.Priority = DeliveryPriority.None;
				}
				LatencyTracker.EndTrackLatency(LatencyComponent.SmtpReceiveDataInternal, this.sessionState.TransportMailItem.LatencyTracker);
				LatencyTracker.BeginTrackLatency(LatencyComponent.SmtpReceiveCommit, this.sessionState.TransportMailItem.LatencyTracker);
				CommitCoordinator commitCoordinator = new CommitCoordinator(this.sessionState.ServerState.MailItemStorage, this.shadowSession, this.sessionState.Configuration.TransportConfiguration.ProcessTransportRole);
				try
				{
					SmtpResponse commitResponse = await commitCoordinator.CommitMailItemAsync(this.sessionState.TransportMailItem);
					base.OnAwaitCompleted(cancellationToken);
					if (!commitResponse.IsEmpty)
					{
						this.sessionState.AbortMailTransaction();
						return new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, commitResponse, this.GetCommandFailureEvent(), false);
					}
				}
				catch (Exception commitException)
				{
					this.sessionState.AbortMailTransaction();
					return this.HandleCommitException(commitException);
				}
				LatencyTracker.EndTrackLatency(LatencyComponent.SmtpReceiveCommit, this.sessionState.TransportMailItem.LatencyTracker);
				SmtpResponse result = this.sessionState.TrackAndEnqueueMailItem();
				if (this.sessionState.Configuration.TransportConfiguration.ProcessTransportRole == ProcessTransportRole.MailboxDelivery && DataBdatCommandBase.IsFailureResponseType(result))
				{
					this.sessionState.AbortMailTransaction();
					result2 = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, result, this.GetCommandFailureEvent(), false);
				}
				else
				{
					result2 = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, result, this.GetSuccessEvent(), false);
				}
			}
			return result2;
		}

		private ParseAndProcessResult<SmtpInStateMachineEvents> HandleCommitException(Exception commitException)
		{
			if (commitException is ExchangeDataException)
			{
				this.sessionState.Tracer.TraceError<string>((long)this.sessionState.GetHashCode(), "GetCommitResults: MIME exception on commit: {0}", commitException.Message);
				this.sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "MIME exception on commit: {0}", new object[]
				{
					commitException.Message
				});
				return new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.InvalidContent, this.GetCommandFailureEvent(), false);
			}
			if (commitException is IOException && ExceptionHelper.IsHandleableTransientCtsException(commitException))
			{
				this.sessionState.Tracer.TraceError<string>((long)this.sessionState.GetHashCode(), "GetCommitResults: IO exception on commit: {0}", commitException.Message);
				this.sessionState.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.RejectDueToIOException);
				byte[] data = Util.AsciiStringToBytes(string.Format(CultureInfo.InvariantCulture, "IO Exception on commit: {0}", new object[]
				{
					commitException.Message
				}));
				this.sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, data, null);
				return new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.CTSParseError, this.GetCommandFailureEvent(), false);
			}
			throw new LocalizedException(Strings.CommitMailFailed, commitException);
		}

		private void HandleMessageRejection(SmtpResponse failureResponse)
		{
			bool flag;
			this.GetFailureResult(ParsingStatus.Complete, failureResponse, out flag);
			if (flag)
			{
				this.sessionState.AbortMailTransaction();
			}
			this.sessionState.EventLog.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveMessageRejected, null, new object[]
			{
				this.sessionState.ReceiveConnector.Name,
				failureResponse
			});
		}

		private async Task<ParseAndProcessResult<SmtpInStateMachineEvents>> HandleCommandFailureAsync(CommandContext commandContext, ParsingStatus parsingStatus, SmtpResponse failureResponse, CancellationToken cancellationToken)
		{
			bool shouldAbortTransaction;
			ParseAndProcessResult<SmtpInStateMachineEvents> failureResult = this.GetFailureResult(parsingStatus, failureResponse, out shouldAbortTransaction);
			ParseAndProcessResult<SmtpInStateMachineEvents> result;
			if (shouldAbortTransaction)
			{
				result = await this.RaiseOnRejectEventAsync(commandContext, failureResult.ParsingStatus, failureResult.SmtpResponse, null, cancellationToken);
			}
			else
			{
				result = failureResult;
			}
			return result;
		}

		private static bool IsFailureResponseType(SmtpResponse smtpResponseToCheck)
		{
			return smtpResponseToCheck.SmtpResponseType == SmtpResponseType.PermanentError || smtpResponseToCheck.SmtpResponseType == SmtpResponseType.TransientError;
		}

		public static readonly ParseAndProcessResult<SmtpInStateMachineEvents> NetworkError = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.Empty, SmtpInStateMachineEvents.NetworkError, false);

		protected ISmtpInStreamBuilder streamBuilder;

		protected IMessageHandler messageHandler;

		protected long messageSizeLimit;

		protected long originalMessageSize = long.MaxValue;

		private EndOfHeadersEventArgs eohEventArgs;

		private EndOfDataEventArgs eodEventArgs;

		private readonly IShadowSession shadowSession = new NullShadowSession();

		private SmtpResponse EohResponse = SmtpResponse.Empty;
	}
}
