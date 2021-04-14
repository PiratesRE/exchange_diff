using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal abstract class FiniteStateMachine<TStateType, TEventType>
	{
		public TStateType StartState { get; private set; }

		public TStateType CurrentState { get; private set; }

		protected FiniteStateMachine(TStateType startState, IReadOnlyDictionary<StateTransition<TStateType, TEventType>, TStateType> stateTransitions)
		{
			ArgumentValidator.ThrowIfNull("stateTransitions", stateTransitions);
			this.StartState = startState;
			this.CurrentState = startState;
			this.stateTransitions = stateTransitions;
			this.ValidateStateMachineConstruction(startState);
		}

		public bool TryMoveToNextState(TEventType eventOccurred)
		{
			TStateType tstateType;
			if (!this.TryGetStateTransition(eventOccurred, out tstateType))
			{
				return false;
			}
			this.OnStateTransition(this.CurrentState, eventOccurred, tstateType);
			this.CurrentState = tstateType;
			return true;
		}

		protected bool TryGetStateTransition(TEventType eventOccurred, out TStateType nextState)
		{
			return this.stateTransitions.TryGetValue(new StateTransition<TStateType, TEventType>(this.CurrentState, eventOccurred), out nextState);
		}

		protected bool IsValidStateTransition(TEventType eventOccurred)
		{
			TStateType tstateType;
			return this.stateTransitions.TryGetValue(new StateTransition<TStateType, TEventType>(this.CurrentState, eventOccurred), out tstateType);
		}

		protected abstract void OnStateTransition(TStateType currentState, TEventType eventOccurred, TStateType nextState);

		private void ValidateStateMachineConstruction(TStateType startState)
		{
			if (!this.stateTransitions.Any<KeyValuePair<StateTransition<TStateType, TEventType>, TStateType>>())
			{
				throw new ConfigurationErrorsException("State machine must contain at least one transition");
			}
			if (!this.stateTransitions.Any(delegate(KeyValuePair<StateTransition<TStateType, TEventType>, TStateType> stateTransition)
			{
				TStateType fromState = stateTransition.Key.FromState;
				return fromState.Equals(startState);
			}))
			{
				throw new ConfigurationErrorsException(string.Format("State machine must contain a transition from the starting state {0}", startState));
			}
			if ((from stateTransition in this.stateTransitions
			select stateTransition.Key.GetType()).Any((Type stateTransitionType) => stateTransitionType != typeof(StateTransition<TStateType, TEventType>)))
			{
				throw new ConfigurationErrorsException(string.Format("All state transitions in the state machine must be of type StateTransition<{0}, {1}>", typeof(TStateType), typeof(TEventType)));
			}
		}

		private readonly IReadOnlyDictionary<StateTransition<TStateType, TEventType>, TStateType> stateTransitions;
	}
}
