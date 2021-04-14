using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal abstract class SmtpInStateMachine<TState, TEvent> : FiniteStateMachine<TState, TEvent> where TState : struct where TEvent : struct
	{
		protected SmtpInStateMachine(SmtpInSessionState sessionState, TState startState, Dictionary<StateTransition<TState, TEvent>, TState> stateTransitions) : base(startState, stateTransitions)
		{
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			ArgumentValidator.ThrowIfNull("stateTransitions", stateTransitions);
			this.sessionState = sessionState;
			this.commandFactory = new Lazy<ISmtpInCommandFactory<TEvent>>(new Func<ISmtpInCommandFactory<TEvent>>(this.CreateCommandFactory));
		}

		public async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			try
			{
				bool continueExecution = await this.ProcessNewConnection(cancellationToken);
				this.OnAwaitCompleted(cancellationToken);
				if (!continueExecution)
				{
					this.sessionState.DisconnectReason = DisconnectReason.DroppedSession;
					await this.OnDisconnectingAsync(cancellationToken);
					this.OnAwaitCompleted(cancellationToken);
				}
				else
				{
					object writeResult = await this.WriteToClientAsync(this.Banner, true);
					this.OnAwaitCompleted(cancellationToken);
					if (writeResult != null)
					{
						this.sessionState.HandleNetworkError(writeResult);
						await this.OnDisconnectingAsync(cancellationToken);
						this.OnAwaitCompleted(cancellationToken);
					}
					else
					{
						while (!this.ReachedEndState)
						{
							NetworkConnection.LazyAsyncResultWithTimeout readResult = await Util.ReadLineAsync(this.sessionState);
							this.OnAwaitCompleted(cancellationToken);
							if (Util.IsReadFailure(readResult))
							{
								this.sessionState.HandleNetworkError(readResult.Result);
								this.MoveToNextStateAndLogOnFailure(this.NetworkErrorEvent);
								break;
							}
							CommandContext commandContext = CommandContext.FromAsyncResult(readResult);
							SmtpResponse smtpResponse = await this.ProcessCommandLineAsync(commandContext, cancellationToken);
							this.OnAwaitCompleted(cancellationToken);
							this.OnCommandCompleted(commandContext, smtpResponse);
							if (!smtpResponse.IsEmpty)
							{
								writeResult = await this.WriteToClientAsync(smtpResponse, false);
								this.OnAwaitCompleted(cancellationToken);
								if (this.IsWriteFailure(writeResult))
								{
									this.sessionState.HandleNetworkError(writeResult);
									this.MoveToNextStateAndLogOnFailure(this.NetworkErrorEvent);
									break;
								}
							}
						}
						if (this.sessionState.DisconnectReason == DisconnectReason.None)
						{
							this.sessionState.DisconnectReason = DisconnectReason.Local;
						}
						await this.OnDisconnectingAsync(cancellationToken);
						this.OnAwaitCompleted(cancellationToken);
					}
				}
			}
			catch (OperationCanceledException)
			{
				this.IsCancelled = true;
				this.sessionState.ServerState.Tracer.TraceDebug<long>(this.sessionState.SessionId, "Session ID {0} was cancelled", this.sessionState.SessionId);
			}
			catch (Exception ex)
			{
				this.SetupPoisonContext();
				this.sessionState.ServerState.EventLog.LogEvent(TransportEventLogConstants.Tuple_SmtpReceiveCatchAll, null, new object[]
				{
					this.sessionState.NetworkConnection.RemoteEndPoint.Address,
					ex
				});
				throw;
			}
			finally
			{
				this.sessionState.Dispose();
				this.sessionState = null;
			}
		}

		public bool IsCancelled { get; private set; }

		public string StateTransitionHistoryToString()
		{
			return string.Join(Environment.NewLine, from transition in this.stateTransitionHistory.ToArray()
			select string.Format("{0} -> {1} ({2})", transition.Item1, transition.Item3, transition.Item2));
		}

		protected abstract ISmtpInCommandFactory<TEvent> CreateCommandFactory();

		protected abstract bool ReachedEndState { get; }

		protected abstract TEvent NetworkErrorEvent { get; }

		protected abstract SmtpResponse Banner { get; }

		protected abstract int MaxCommandLength { get; }

		protected abstract TEvent GetCompletedEventForCommand(SmtpInCommand commandType);

		protected abstract Task<SmtpResponse> OnNewConnectionAsync(IPEndPoint remoteEndPoint, CancellationToken cancellationToken);

		protected abstract Task<SmtpResponse> OnUnrecognizedCommandAsync(CommandContext commandContext);

		protected abstract Task<SmtpResponse> OnBadCommandSequenceAsync(CommandContext commandContext);

		protected abstract void OnCommandReceived(CommandContext commandContext);

		protected abstract void OnCommandCompleted(CommandContext commandContext, SmtpResponse smtpResponse);

		protected abstract Task OnDisconnectingAsync(CancellationToken cancellationToken);

		protected virtual void OnAwaitCompleted(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
		}

		protected override void OnStateTransition(TState currentState, TEvent eventOccurred, TState nextState)
		{
			this.stateTransitionHistory.Enqueue(Tuple.Create<TState, TEvent, TState>(currentState, eventOccurred, nextState));
		}

		protected void SetupPoisonContext()
		{
			if (this.sessionState.TransportMailItem != null && !string.IsNullOrEmpty(this.sessionState.TransportMailItem.InternetMessageId))
			{
				PoisonMessage.Context = new MessageContext(this.sessionState.TransportMailItem.RecordId, this.sessionState.TransportMailItem.InternetMessageId, MessageProcessingSource.SmtpReceive);
				return;
			}
			PoisonMessage.Context = null;
		}

		private async Task<bool> ProcessNewConnection(CancellationToken cancellationToken)
		{
			SmtpResponse smtpResponse = await this.OnNewConnectionAsync(this.sessionState.NetworkConnection.RemoteEndPoint, cancellationToken);
			this.OnAwaitCompleted(cancellationToken);
			bool result;
			if (!smtpResponse.IsEmpty)
			{
				await this.WriteToClientAsync(smtpResponse, true);
				this.OnAwaitCompleted(cancellationToken);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		private async Task<SmtpResponse> ProcessCommandLineAsync(CommandContext commandContext, CancellationToken cancellationToken)
		{
			this.OnCommandReceived(commandContext);
			SmtpResponse result;
			if (commandContext.Length > this.MaxCommandLength)
			{
				result = SmtpResponse.CommandTooLong;
			}
			else
			{
				SmtpInCommand commandType = commandContext.IdentifySmtpCommand();
				if (commandType == SmtpInCommand.UNKNOWN)
				{
					commandContext.LogReceivedCommand(this.sessionState.ProtocolLogSession);
					SmtpResponse smtpResponse = await this.OnUnrecognizedCommandAsync(commandContext);
					this.OnAwaitCompleted(cancellationToken);
					this.sessionState.ProtocolLogSession.LogSend(smtpResponse.ToByteArray());
					result = smtpResponse;
				}
				else
				{
					Tuple<ISmtpInCommand<TEvent>, SmtpResponse> commandOrSmtpResponse = await this.CreateCommandAsync(commandType, commandContext, cancellationToken);
					this.OnAwaitCompleted(cancellationToken);
					if (commandOrSmtpResponse.Item1 == null)
					{
						this.sessionState.ProtocolLogSession.LogSend(commandOrSmtpResponse.Item2.ToByteArray());
						result = commandOrSmtpResponse.Item2;
					}
					else
					{
						SmtpResponse response = await this.ParseAndProcessCommandAsync(commandOrSmtpResponse.Item1, commandContext, cancellationToken);
						this.OnAwaitCompleted(cancellationToken);
						result = response;
					}
				}
			}
			return result;
		}

		private async Task<Tuple<ISmtpInCommand<TEvent>, SmtpResponse>> CreateCommandAsync(SmtpInCommand commandType, CommandContext commandContext, CancellationToken cancellationToken)
		{
			Tuple<ISmtpInCommand<TEvent>, SmtpResponse> result;
			if (!base.IsValidStateTransition(this.GetCompletedEventForCommand(commandType)))
			{
				commandContext.LogReceivedCommand(this.sessionState.ProtocolLogSession);
				SmtpResponse smtpResponse = await this.OnBadCommandSequenceAsync(commandContext);
				this.OnAwaitCompleted(cancellationToken);
				result = Tuple.Create<ISmtpInCommand<TEvent>, SmtpResponse>(null, smtpResponse);
			}
			else
			{
				ISmtpInCommand<TEvent> command = this.commandFactory.Value.CreateCommand(commandType);
				if (command == null)
				{
					commandContext.LogReceivedCommand(this.sessionState.ProtocolLogSession);
					SmtpResponse smtpResponse2 = await this.OnUnrecognizedCommandAsync(commandContext);
					this.OnAwaitCompleted(cancellationToken);
					result = Tuple.Create<ISmtpInCommand<TEvent>, SmtpResponse>(null, smtpResponse2);
				}
				else
				{
					result = Tuple.Create<ISmtpInCommand<TEvent>, SmtpResponse>(command, SmtpResponse.Empty);
				}
			}
			return result;
		}

		private async Task<SmtpResponse> ParseAndProcessCommandAsync(ISmtpInCommand<TEvent> command, CommandContext commandContext, CancellationToken cancellationToken)
		{
			ParseAndProcessResult<TEvent> parseAndProcessResult = await command.ParseAndProcessAsync(commandContext, cancellationToken);
			this.OnAwaitCompleted(cancellationToken);
			command.LogSmtpResponse(parseAndProcessResult.SmtpResponse);
			this.MoveToNextStateAndLogOnFailure(parseAndProcessResult.SmtpEvent);
			return parseAndProcessResult.SmtpResponse;
		}

		private void MoveToNextStateAndLogOnFailure(TEvent eventOccurred)
		{
			if (!base.TryMoveToNextState(eventOccurred))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(string.Format("Invalid state transition: CurrentState: {0}, eventOccurred: {1}", base.CurrentState, eventOccurred));
				stringBuilder.AppendLine("Recent state transition history for this session:");
				foreach (Tuple<TState, TEvent, TState> tuple in this.stateTransitionHistory.ToArray())
				{
					stringBuilder.AppendLine(string.Format("CurrentState: {0}, eventOccurred: {1}, nextState: {2}", tuple.Item1, tuple.Item2, tuple.Item3));
				}
				Exception ex = new InvalidOperationException(stringBuilder.ToString());
				bool flag;
				ExWatson.SendThrottledGenericWatsonReport("E12", ExWatson.ApplicationVersion.ToString(), ExWatson.AppName, "15.00.1497.012", Assembly.GetExecutingAssembly().GetName().Name, ex.GetType().Name, ex.StackTrace, ex.GetHashCode().ToString(CultureInfo.InvariantCulture), ex.TargetSite.Name, "details", TimeSpan.FromHours(1.0), out flag);
			}
		}

		private bool IsWriteFailure(object writeResult)
		{
			return writeResult != null;
		}

		private Task<object> WriteToClientAsync(SmtpResponse smtpResponse, bool logResponse)
		{
			if (logResponse)
			{
				this.sessionState.ProtocolLogSession.LogSend(smtpResponse.ToByteArray());
			}
			return Util.WriteToClientAsync(this.sessionState.NetworkConnection, smtpResponse);
		}

		private readonly DiagnosticsHistoryQueue<Tuple<TState, TEvent, TState>> stateTransitionHistory = new DiagnosticsHistoryQueue<Tuple<TState, TEvent, TState>>(100);

		private readonly Lazy<ISmtpInCommandFactory<TEvent>> commandFactory;

		protected SmtpInSessionState sessionState;
	}
}
