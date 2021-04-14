using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class StateMachineUtils<TState> where TState : struct
	{
		public static void AddStateChangeTransition(TState fromState, SmtpInStateMachineEvents eventOccurred, TState toState, ICollection<KeyValuePair<StateTransition<TState, SmtpInStateMachineEvents>, TState>> stateTransitions)
		{
			stateTransitions.Add(StateMachineUtils<TState>.CreateStateChangeTransition(fromState, eventOccurred, toState));
		}

		public static void AddNoStateChangeTransition(TState fromAndToState, SmtpInStateMachineEvents eventOccurred, ICollection<KeyValuePair<StateTransition<TState, SmtpInStateMachineEvents>, TState>> stateTransitions)
		{
			stateTransitions.Add(StateMachineUtils<TState>.CreateStateChangeTransition(fromAndToState, eventOccurred, fromAndToState));
		}

		private static KeyValuePair<StateTransition<TState, SmtpInStateMachineEvents>, TState> CreateStateChangeTransition(TState fromState, SmtpInStateMachineEvents eventOccurred, TState toState)
		{
			return new KeyValuePair<StateTransition<TState, SmtpInStateMachineEvents>, TState>(StateMachineUtils<TState>.NewStateTransition(fromState, eventOccurred), toState);
		}

		private static StateTransition<TState, SmtpInStateMachineEvents> NewStateTransition(TState fromState, SmtpInStateMachineEvents eventOccurred)
		{
			return new StateTransition<TState, SmtpInStateMachineEvents>(fromState, eventOccurred);
		}
	}
}
