using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class MessageContentHandler : IMessageHandler
	{
		public MessageContentHandler(SmtpInSessionState sessionStateToUse, ISmtpInStreamBuilder streamBuilderToUse)
		{
			ArgumentValidator.ThrowIfNull("sessionStateToUse", sessionStateToUse);
			ArgumentValidator.ThrowIfNull("streamBuilderToUse", streamBuilderToUse);
			this.sessionState = sessionStateToUse;
			this.streamBuilder = streamBuilderToUse;
		}

		public MessageHandlerResult Process(CommandContext commandContext, out SmtpResponse smtpResponse)
		{
			ArgumentValidator.ThrowIfNull("commandContext", commandContext);
			int num;
			bool flag;
			SmtpResponse smtpResponse2;
			if (!this.WriteContentsToStream(commandContext, out num, out flag, out smtpResponse2))
			{
				this.finalResponse = smtpResponse2;
				this.sessionState.StartDiscardingMessage();
			}
			if (commandContext.OriginalLength != num)
			{
				this.sessionState.PutBackReceivedBytes(commandContext.OriginalLength - num);
			}
			if (!flag)
			{
				smtpResponse = SmtpResponse.Empty;
				return MessageHandlerResult.MoreDataRequired;
			}
			if (!this.finalResponse.IsEmpty)
			{
				smtpResponse = this.finalResponse;
				return MessageHandlerResult.Failure;
			}
			smtpResponse = SmtpResponse.Empty;
			return MessageHandlerResult.Complete;
		}

		private bool WriteContentsToStream(CommandContext commandContext, out int numberOfBytesWritten, out bool eodFound, out SmtpResponse failureResponse)
		{
			numberOfBytesWritten = 0;
			failureResponse = SmtpResponse.Empty;
			try
			{
				eodFound = this.streamBuilder.Write(commandContext, out numberOfBytesWritten);
			}
			catch (Exception exception)
			{
				eodFound = this.streamBuilder.IsEodSeen;
				if (DataBdatHelpers.IsFilterableException(exception))
				{
					failureResponse = DataBdatHelpers.HandleFilterableException(exception, this.sessionState);
					return false;
				}
				throw;
			}
			return true;
		}

		private readonly SmtpInSessionState sessionState;

		private readonly ISmtpInStreamBuilder streamBuilder;

		private SmtpResponse finalResponse = SmtpResponse.Empty;
	}
}
