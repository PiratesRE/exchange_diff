using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal abstract class TransportSmtpInStateMachine<TState> : SmtpInStateMachine<TState, SmtpInStateMachineEvents> where TState : struct
	{
		protected TransportSmtpInStateMachine(SmtpInSessionState sessionState, TState startState, Dictionary<StateTransition<TState, SmtpInStateMachineEvents>, TState> stateTransitions) : base(sessionState, startState, stateTransitions)
		{
		}

		protected override SmtpInStateMachineEvents NetworkErrorEvent
		{
			get
			{
				return SmtpInStateMachineEvents.NetworkError;
			}
		}

		protected override SmtpResponse Banner
		{
			get
			{
				return Util.SmtpBanner(this.sessionState.ReceiveConnector, () => this.sessionState.Configuration.TransportConfiguration.PhysicalMachineName, this.sessionState.Configuration.TransportConfiguration.Version, this.sessionState.UtcNow, true);
			}
		}

		protected override int MaxCommandLength
		{
			get
			{
				return 32768;
			}
		}

		protected override async Task<SmtpResponse> OnNewConnectionAsync(IPEndPoint remoteEndPoint, CancellationToken cancellationToken)
		{
			this.IncrementConnectionPerformanceCounters();
			SmtpResponse result;
			if (this.sessionState.ServerState.RejectCommands)
			{
				this.sessionState.Tracer.TraceDebug<long>((long)this.GetHashCode(), "Session (id={0}) disconnected: RejectCommands==true", this.sessionState.NetworkConnection.ConnectionId);
				if (this.sessionState.ServerState.RejectionSmtpResponse.Equals(SmtpResponse.InsufficientResource))
				{
					this.sessionState.UpdateAvailabilityPerfCounters(LegitimateSmtpAvailabilityCategory.RejectDueToBackPressure);
				}
				result = this.sessionState.ServerState.RejectionSmtpResponse;
			}
			else
			{
				SmtpResponse smtpResponseFromMexRuntime = await this.sessionState.SmtpAgentSession.RaiseEventAsync("OnConnectEvent", ConnectEventSourceImpl.Create(this.sessionState), new ConnectEventArgs(this.sessionState));
				this.OnAwaitCompleted(cancellationToken);
				if (!smtpResponseFromMexRuntime.IsEmpty)
				{
					result = smtpResponseFromMexRuntime;
				}
				else if (this.sessionState.ShouldDisconnect)
				{
					result = (this.sessionState.SmtpResponse.IsEmpty ? SmtpResponse.ConnectionDroppedByAgentError : this.sessionState.SmtpResponse);
				}
				else
				{
					result = SmtpResponse.Empty;
				}
			}
			return result;
		}

		protected override async Task OnDisconnectingAsync(CancellationToken cancellationToken)
		{
			await this.sessionState.SmtpAgentSession.RaiseEventAsync("OnDisconnectEvent", DisconnectEventSourceImpl.Create(this.sessionState), new DisconnectEventArgs(this.sessionState));
			this.OnAwaitCompleted(cancellationToken);
			this.sessionState.OnDisconnect();
			this.DecrementConnectionPerformanceCounters();
		}

		protected override Task<SmtpResponse> OnUnrecognizedCommandAsync(CommandContext commandContext)
		{
			return CompletedTasks.SmtpResponseUnrecognizedCommand;
		}

		protected override Task<SmtpResponse> OnBadCommandSequenceAsync(CommandContext commandContext)
		{
			return CompletedTasks.SmtpResponseBadCommandSequence;
		}

		protected override void OnCommandReceived(CommandContext commandContext)
		{
		}

		protected override void OnCommandCompleted(CommandContext commandContext, SmtpResponse smtpResponse)
		{
		}

		private void IncrementConnectionPerformanceCounters()
		{
			ISmtpReceivePerfCounters smtpReceivePerfCounterInstance = this.sessionState.ReceiveConnectorStub.SmtpReceivePerfCounterInstance;
			if (smtpReceivePerfCounterInstance != null)
			{
				smtpReceivePerfCounterInstance.ConnectionsCurrent.Increment();
				smtpReceivePerfCounterInstance.ConnectionsTotal.Increment();
			}
		}

		private void DecrementConnectionPerformanceCounters()
		{
			ISmtpReceivePerfCounters smtpReceivePerfCounterInstance = this.sessionState.ReceiveConnectorStub.SmtpReceivePerfCounterInstance;
			if (smtpReceivePerfCounterInstance != null)
			{
				smtpReceivePerfCounterInstance.ConnectionsCurrent.Decrement();
				if (this.sessionState.NetworkConnection.IsTls)
				{
					smtpReceivePerfCounterInstance.TlsConnectionsCurrent.Decrement();
				}
			}
		}
	}
}
