using System;
using System.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class DataSmtpInCommand : DataBdatCommandBase
	{
		public DataSmtpInCommand(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
		}

		protected override ParseResult Parse(CommandContext commandContext, out string agentEventTopic, out ReceiveCommandEventArgs agentEventArgs)
		{
			ParseResult result = DataSmtpCommandParser.Parse(commandContext, this.sessionState);
			if (result.IsFailed)
			{
				agentEventTopic = null;
				agentEventArgs = null;
			}
			else
			{
				agentEventTopic = "OnDataCommand";
				agentEventArgs = new DataCommandEventArgs(this.sessionState)
				{
					MailItem = this.sessionState.TransportMailItemWrapper
				};
			}
			return result;
		}

		protected override SmtpInStateMachineEvents GetCommandFailureEvent()
		{
			return SmtpInStateMachineEvents.DataFailed;
		}

		protected override ParseResult PreProcess()
		{
			bool allowBinaryContent = this.sessionState.ReceiveConnector.EightBitMimeEnabled || !this.sessionState.Configuration.TransportConfiguration.InboundApplyMissingEncoding;
			this.streamBuilder = new SmtpInDataStreamBuilder();
			this.messageHandler = new MessageContentHandler(this.sessionState, this.streamBuilder);
			if (!base.SetupMessageStream(allowBinaryContent))
			{
				this.sessionState.StartDiscardingMessage();
				return ParseResult.DataTransactionFailed;
			}
			this.sessionState.TransportMailItem.MimeDocument.EndOfHeaders = new MimeDocument.EndOfHeadersCallback(base.HandleEoh);
			return ParseResult.MoreDataRequired;
		}

		protected override bool TryGetInitialResponse(out SmtpResponse initialResponse)
		{
			initialResponse = SmtpResponse.StartData;
			return true;
		}

		protected override bool ValidateAccumulatedSize(out SmtpResponse failureResponse)
		{
			failureResponse = DataBdatHelpers.ValidateHeaderSize(this.sessionState, this.AccumulatedMessageSize, base.IsEohSeen);
			if (failureResponse.IsEmpty)
			{
				failureResponse = DataBdatHelpers.ValidateMessageSize(this.sessionState, this.messageSizeLimit, this.originalMessageSize, this.AccumulatedMessageSize, base.IsEohSeen);
			}
			return failureResponse.IsEmpty;
		}

		protected override bool ShouldProcessEoh()
		{
			return !base.IsEohSeen;
		}

		protected override bool ShouldProcessEod()
		{
			return !this.sessionState.DiscardingMessage;
		}

		protected override void PostEoh()
		{
		}

		protected override void PostEod()
		{
			this.sessionState.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.SuccessfulSubmission);
			this.sessionState.ReleaseMailItem();
		}

		protected override bool ShouldHandleBareLineFeedInBody()
		{
			return true;
		}

		protected override ParseAndProcessResult<SmtpInStateMachineEvents> GetFinalResult(ParseAndProcessResult<SmtpInStateMachineEvents> eodResult)
		{
			if (this.sessionState.DiscardingMessage)
			{
				return DataBdatHelpers.CreateResultFromResponse(SmtpResponse.DataTransactionFailed, SmtpInStateMachineEvents.DataFailed);
			}
			if (!eodResult.SmtpResponse.IsEmpty)
			{
				return eodResult;
			}
			string recordId = this.sessionState.TransportMailItem.RecordId.ToString(CultureInfo.InvariantCulture);
			SmtpResponse response = SmtpResponse.QueuedMailForDelivery(SmtpCommand.GetBracketedString(this.sessionState.TransportMailItem.InternetMessageId), recordId, this.sessionState.Configuration.TransportConfiguration.PhysicalMachineName, this.sessionState.Configuration.TransportConfiguration.SmtpDataResponse);
			return DataBdatHelpers.CreateResultFromResponse(response, SmtpInStateMachineEvents.DataProcessed);
		}

		protected override ParseAndProcessResult<SmtpInStateMachineEvents> GetFailureResult(ParsingStatus parsingStatus, SmtpResponse failureResponse, out bool shouldAbortTransaction)
		{
			shouldAbortTransaction = true;
			return new ParseAndProcessResult<SmtpInStateMachineEvents>(parsingStatus, failureResponse, SmtpInStateMachineEvents.DataFailed, false);
		}

		protected override SmtpInStateMachineEvents GetSuccessEvent()
		{
			return SmtpInStateMachineEvents.DataProcessed;
		}
	}
}
