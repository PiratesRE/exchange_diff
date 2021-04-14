using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal abstract class SmtpInCommandBase : ISmtpInCommand<SmtpInStateMachineEvents>
	{
		protected SmtpInCommandBase(SmtpInSessionState sessionState, AwaitCompletedDelegate awaitCompletedDelegate)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			ArgumentValidator.ThrowIfNull("awaitCompletedDelegate", awaitCompletedDelegate);
			this.sessionState = sessionState;
			this.networkConnection = sessionState.NetworkConnection;
			this.awaitCompletedDelegate = awaitCompletedDelegate;
		}

		public async Task<ParseAndProcessResult<SmtpInStateMachineEvents>> ParseAndProcessAsync(CommandContext commandContext, CancellationToken cancellationToken)
		{
			string agentEventTopic;
			ReceiveCommandEventArgs agentEventArgs;
			ParseResult parseResult = this.Parse(commandContext, out agentEventTopic, out agentEventArgs);
			this.LogCommandReceived(commandContext);
			ParseAndProcessResult<SmtpInStateMachineEvents> result2;
			if (parseResult.IsFailed)
			{
				SmtpResponse smtpResponse = await this.RaiseRejectEventAsync(commandContext, parseResult.ParsingStatus, parseResult.SmtpResponse, null, cancellationToken);
				if (parseResult.DisconnectClient || this.sessionState.ShouldDisconnect)
				{
					if (this.sessionState.DisconnectReason == DisconnectReason.None)
					{
						this.sessionState.DisconnectReason = (this.sessionState.ShouldDisconnect ? DisconnectReason.DroppedSession : DisconnectReason.Local);
					}
					result2 = new ParseAndProcessResult<SmtpInStateMachineEvents>(parseResult.ParsingStatus, smtpResponse, SmtpInStateMachineEvents.SendResponseAndDisconnectClient, true);
				}
				else
				{
					result2 = new ParseAndProcessResult<SmtpInStateMachineEvents>(parseResult.ParsingStatus, smtpResponse, this.GetCommandFailureEvent(), false);
				}
			}
			else
			{
				using (new AutoLatencyTracker(this.sessionState.SmtpAgentSession.LatencyTracker, this.LatencyComponent, this.MailItemLatencyTracker))
				{
					ParseAndProcessResult<SmtpInStateMachineEvents> result = await this.RaiseAgentCommandEventAsync(agentEventTopic, agentEventArgs, commandContext, parseResult, cancellationToken);
					if (!result.SmtpResponse.IsEmpty)
					{
						return result;
					}
				}
				result2 = await this.ProcessAsync(cancellationToken);
			}
			return result2;
		}

		public virtual void LogSmtpResponse(SmtpResponse smtpResponse)
		{
			if (!smtpResponse.IsEmpty)
			{
				this.sessionState.ProtocolLogSession.LogSend(smtpResponse.ToByteArray());
				if (smtpResponse.SmtpResponseType == SmtpResponseType.TransientError || smtpResponse.SmtpResponseType == SmtpResponseType.PermanentError)
				{
					string text = null;
					if (this.sessionState.TransportMailItem != null)
					{
						text = this.sessionState.TransportMailItem.InternetMessageId;
					}
					if (!string.IsNullOrEmpty(text))
					{
						this.sessionState.ProtocolLogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "InternetMessageId: {0}", new object[]
						{
							text
						});
					}
				}
			}
		}

		protected Task<object> WriteToClientAsync(SmtpResponse smtpResponse)
		{
			return Util.WriteToClientAsync(this.networkConnection, smtpResponse);
		}

		protected void OnAwaitCompleted(CancellationToken cancellationToken)
		{
			this.awaitCompletedDelegate(cancellationToken);
		}

		protected virtual void LogCommandReceived(CommandContext command)
		{
			command.LogReceivedCommand(this.sessionState.ProtocolLogSession);
		}

		protected virtual LatencyComponent LatencyComponent
		{
			get
			{
				return LatencyComponent.None;
			}
		}

		protected abstract ParseResult Parse(CommandContext commandContext, out string agentEventTopic, out ReceiveCommandEventArgs agentEventArgs);

		protected abstract Task<ParseAndProcessResult<SmtpInStateMachineEvents>> ProcessAsync(CancellationToken cancellationToken);

		protected async Task<ParseAndProcessResult<SmtpInStateMachineEvents>> RaiseAgentEventAsync(string eventTopic, ReceiveEventArgs eventArgs, CommandContext commandContext, ParseResult parseResult, CancellationToken cancellationToken, ReceiveEventSource receiveEventSource = null)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("eventTopic", eventTopic);
			ArgumentValidator.ThrowIfNull("eventArgs", eventArgs);
			ArgumentValidator.ThrowIfNull("commandContext", commandContext);
			SmtpResponse smtpResponseFromMexRuntime = await this.sessionState.SmtpAgentSession.RaiseEventAsync(eventTopic, receiveEventSource ?? ReceiveCommandEventSourceImpl.Create(this.sessionState), eventArgs);
			this.awaitCompletedDelegate(cancellationToken);
			ParseAndProcessResult<SmtpInStateMachineEvents> result;
			if (!smtpResponseFromMexRuntime.IsEmpty)
			{
				result = this.CreateUnhandledAgentExceptionResult(parseResult.ParsingStatus, smtpResponseFromMexRuntime);
			}
			else if (this.sessionState.ShouldDisconnect)
			{
				SmtpResponse smtpResponse = this.sessionState.SmtpResponse.IsEmpty ? SmtpResponse.ConnectionDroppedByAgentError : this.sessionState.SmtpResponse;
				result = new ParseAndProcessResult<SmtpInStateMachineEvents>(parseResult.ParsingStatus, smtpResponse, SmtpInStateMachineEvents.SendResponseAndDisconnectClient, false);
			}
			else if (!this.sessionState.SmtpResponse.IsEmpty)
			{
				SmtpResponse finalSmtpResponse = await this.RaiseRejectEventAsync(commandContext, parseResult.ParsingStatus, this.sessionState.SmtpResponse, eventArgs, cancellationToken);
				this.sessionState.SmtpResponse = SmtpResponse.Empty;
				result = new ParseAndProcessResult<SmtpInStateMachineEvents>(parseResult.ParsingStatus, finalSmtpResponse, this.GetCommandFailureEvent(), false);
			}
			else
			{
				result = SmtpInCommandBase.SmtpResponseEmptyResult;
			}
			return result;
		}

		protected ParseAndProcessResult<SmtpInStateMachineEvents> CreateUnhandledAgentExceptionResult(ParsingStatus parsingStatus, SmtpResponse smtpResponse)
		{
			return new ParseAndProcessResult<SmtpInStateMachineEvents>(parsingStatus, smtpResponse, this.GetCommandFailureEvent(), false);
		}

		protected async Task<SmtpResponse> RaiseRejectEventAsync(CommandContext commandContext, ParsingStatus parsingStatus, SmtpResponse smtpResponse, ReceiveEventArgs originalEventArgs, CancellationToken cancellationToken)
		{
			SmtpResponse smtpResponseFromMexRuntime = await this.InvokeRaiseRejectEventAsync(parsingStatus, smtpResponse, commandContext, originalEventArgs);
			this.awaitCompletedDelegate(cancellationToken);
			SmtpResponse result;
			if (!smtpResponseFromMexRuntime.IsEmpty)
			{
				result = smtpResponseFromMexRuntime;
			}
			else
			{
				result = smtpResponse;
			}
			return result;
		}

		protected virtual SmtpInStateMachineEvents GetCommandFailureEvent()
		{
			return SmtpInStateMachineEvents.CommandFailed;
		}

		private LatencyTracker MailItemLatencyTracker
		{
			get
			{
				if (this.sessionState.TransportMailItem != null)
				{
					return this.sessionState.TransportMailItem.LatencyTracker;
				}
				return null;
			}
		}

		private Task<ParseAndProcessResult<SmtpInStateMachineEvents>> RaiseAgentCommandEventAsync(string agentEventTopic, ReceiveCommandEventArgs agentEventArgs, CommandContext commandContext, ParseResult parseResult, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(agentEventTopic) || agentEventArgs == null)
			{
				return SmtpInCommandBase.SmtpResponseEmptyResultTask;
			}
			return this.RaiseAgentEventAsync(agentEventTopic, agentEventArgs, commandContext, parseResult, cancellationToken, null);
		}

		private Task<SmtpResponse> InvokeRaiseRejectEventAsync(ParsingStatus parsingStatus, SmtpResponse smtpResponse, CommandContext commandContext, EventArgs originalEventArgs)
		{
			return this.sessionState.SmtpAgentSession.RaiseEventAsync("OnReject", RejectEventSourceImpl.Create(this.sessionState), new RejectEventArgs(this.sessionState)
			{
				RawCommand = commandContext.Command,
				ParsingStatus = EnumConverter.InternalToPublic(parsingStatus),
				SmtpResponse = smtpResponse,
				OriginalArguments = originalEventArgs
			});
		}

		protected static readonly ParseAndProcessResult<SmtpInStateMachineEvents> SmtpResponseEmptyResult = new ParseAndProcessResult<SmtpInStateMachineEvents>(ParsingStatus.Complete, SmtpResponse.Empty, SmtpInStateMachineEvents.CommandFailed, false);

		protected static readonly Task<ParseAndProcessResult<SmtpInStateMachineEvents>> SmtpResponseEmptyResultTask = Task.FromResult<ParseAndProcessResult<SmtpInStateMachineEvents>>(SmtpInCommandBase.SmtpResponseEmptyResult);

		protected readonly INetworkConnection networkConnection;

		protected readonly SmtpInSessionState sessionState;

		private readonly AwaitCompletedDelegate awaitCompletedDelegate;
	}
}
