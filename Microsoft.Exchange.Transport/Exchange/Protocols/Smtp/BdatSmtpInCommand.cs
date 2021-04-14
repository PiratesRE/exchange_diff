using System;
using System.Linq;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class BdatSmtpInCommand : DataBdatCommandBase
	{
		public BdatSmtpInCommand(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate) : base(sessionState, awaitCompletedDelegate)
		{
		}

		protected override ParseResult Parse(CommandContext commandContext, out string agentEventTopic, out ReceiveCommandEventArgs agentEventArgs)
		{
			if (this.sessionState.TransportMailItem == null)
			{
				agentEventTopic = null;
				agentEventArgs = null;
				return new ParseResult(ParsingStatus.ProtocolError, SmtpResponse.BadCommandSequence, true);
			}
			this.parseResult = BdatSmtpCommandParser.Parse(commandContext, this.sessionState, (this.sessionState.BdatState != null) ? this.sessionState.BdatState.AccumulatedChunkSize : 0L, out this.chunkSize);
			if (this.parseResult.IsFailed)
			{
				agentEventTopic = null;
				agentEventArgs = null;
				this.sessionState.StartDiscardingMessage();
				return new ParseResult(this.parseResult.ParsingStatus, this.parseResult.SmtpResponse, true);
			}
			agentEventTopic = "OnDataCommand";
			agentEventArgs = new DataCommandEventArgs(this.sessionState)
			{
				MailItem = this.sessionState.TransportMailItemWrapper
			};
			this.isLastChunk = BdatSmtpCommandParser.IsLastChunk(this.parseResult.ParsingStatus);
			if (this.sessionState.DiscardingMessage)
			{
				this.parseResult = ParseResult.BadCommandSequence;
			}
			return ParseResult.ParsingComplete;
		}

		protected override SmtpInStateMachineEvents GetCommandFailureEvent()
		{
			return SmtpInStateMachineEvents.BdatFailed;
		}

		protected override long AccumulatedMessageSize
		{
			get
			{
				long num = 0L;
				if (this.streamBuilder != null)
				{
					num = this.streamBuilder.TotalBytesWritten;
				}
				if (this.sessionState.TransportMailItem != null)
				{
					num += this.sessionState.TransportMailItem.MimeSize;
				}
				return num;
			}
		}

		protected override ParseResult PreProcess()
		{
			bool flag = this.IsProcessingMessageContextBlob();
			this.streamBuilder = new SmtpInBdatStreamBuilder
			{
				ChunkSize = this.chunkSize
			};
			if (flag)
			{
				this.messageHandler = new MessageContextBlobHandler(this.sessionState, this.sessionState.ExpectedMessageContextBlobs.Dequeue(), this.chunkSize);
			}
			else
			{
				this.messageHandler = new MessageContentHandler(this.sessionState, this.streamBuilder);
			}
			if (!this.sessionState.InitializeBdatState(this.streamBuilder, this.chunkSize, this.messageSizeLimit))
			{
				return ParseResult.DataTransactionFailed;
			}
			if (this.isLastChunk && this.AccumulatedMessageSize + this.chunkSize <= 0L)
			{
				return ParseResult.InvalidLastChunk;
			}
			if (this.isLastChunk && this.chunkSize == 0L)
			{
				return ParseResult.ParsingComplete;
			}
			if (!base.IsEohSeen && !flag)
			{
				this.sessionState.TransportMailItem.MimeDocument.EndOfHeaders = new MimeDocument.EndOfHeadersCallback(base.HandleEoh);
			}
			return ParseResult.MoreDataRequired;
		}

		protected override bool TryGetInitialResponse(out SmtpResponse initialResponse)
		{
			initialResponse = SmtpResponse.Empty;
			return false;
		}

		protected override bool ValidateAccumulatedSize(out SmtpResponse failureResponse)
		{
			failureResponse = DataBdatHelpers.ValidateHeaderSize(this.sessionState, this.sessionState.BdatState.AccumulatedChunkSize, this.sessionState.BdatState.IsEohSeen);
			if (failureResponse.IsEmpty)
			{
				failureResponse = DataBdatHelpers.ValidateMessageSize(this.sessionState, this.sessionState.BdatState.MessageSizeLimit, this.sessionState.BdatState.OriginalMessageSize, this.sessionState.BdatState.AccumulatedChunkSize, this.sessionState.BdatState.IsEohSeen);
			}
			return failureResponse.IsEmpty;
		}

		protected override bool ShouldProcessEoh()
		{
			return !this.sessionState.BdatState.IsEohSeen && this.sessionState.TransportMailItem.MimeDocument.EndOfHeaders != null;
		}

		protected override bool ShouldProcessEod()
		{
			return this.isLastChunk && !this.sessionState.DiscardingMessage;
		}

		protected override void PostEoh()
		{
			this.sessionState.BdatState.UpdateState(this.sessionState.TransportMailItem.InternetMessageId, this.originalMessageSize, this.messageSizeLimit, true);
		}

		protected override void PostEod()
		{
			if (this.isLastChunk && !this.IsProcessingMessageContextBlob())
			{
				this.sessionState.UpdateSmtpAvailabilityPerfCounter(LegitimateSmtpAvailabilityCategory.SuccessfulSubmission);
				this.sessionState.ReleaseMailItem();
			}
		}

		protected override bool ShouldHandleBareLineFeedInBody()
		{
			return false;
		}

		protected override ParseAndProcessResult<SmtpInStateMachineEvents> GetFinalResult(ParseAndProcessResult<SmtpInStateMachineEvents> eodResult)
		{
			if (this.sessionState.DiscardingMessage)
			{
				if (this.parseResult.IsFailed)
				{
					return new ParseAndProcessResult<SmtpInStateMachineEvents>(this.parseResult, SmtpInStateMachineEvents.BdatFailed);
				}
				return DataBdatHelpers.CreateResultFromResponse(SmtpResponse.DataTransactionFailed, SmtpInStateMachineEvents.BdatFailed);
			}
			else
			{
				if (!eodResult.SmtpResponse.IsEmpty)
				{
					return eodResult;
				}
				if (!this.isLastChunk)
				{
					return DataBdatHelpers.CreateResultFromResponse(SmtpResponse.OctetsReceived(this.chunkSize), SmtpInStateMachineEvents.BdatProcessed);
				}
				return DataBdatHelpers.CreateResultFromResponse(SmtpResponse.QueuedMailForDelivery(SmtpCommand.GetBracketedString(this.sessionState.TransportMailItem.InternetMessageId)), SmtpInStateMachineEvents.BdatLastProcessed);
			}
		}

		protected override ParseAndProcessResult<SmtpInStateMachineEvents> GetFailureResult(ParsingStatus parsingStatus, SmtpResponse failureResponse, out bool shouldAbortTransaction)
		{
			if (this.isLastChunk)
			{
				shouldAbortTransaction = true;
			}
			else
			{
				shouldAbortTransaction = false;
			}
			return new ParseAndProcessResult<SmtpInStateMachineEvents>(parsingStatus, failureResponse, this.GetCommandFailureEvent(), false);
		}

		protected override SmtpInStateMachineEvents GetSuccessEvent()
		{
			if (!this.isLastChunk)
			{
				return SmtpInStateMachineEvents.BdatProcessed;
			}
			return SmtpInStateMachineEvents.BdatLastProcessed;
		}

		private bool IsProcessingMessageContextBlob()
		{
			return this.sessionState.ExpectedMessageContextBlobs.Any<IInboundMessageContextBlob>();
		}

		private long chunkSize;

		private bool isLastChunk;

		private ParseResult parseResult;
	}
}
