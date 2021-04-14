using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal sealed class StateTransition<StateType, EventType> : IEquatable<StateTransition<StateType, EventType>>
	{
		public StateType FromState { get; private set; }

		public EventType EventOccurred { get; private set; }

		public StateTransition(StateType fromState, EventType eventOccurred)
		{
			this.FromState = fromState;
			this.EventOccurred = eventOccurred;
		}

		public override int GetHashCode()
		{
			int num = 17;
			int num2 = 31;
			StateType fromState = this.FromState;
			int num3 = num + num2 * fromState.GetHashCode();
			int num4 = 31;
			EventType eventOccurred = this.EventOccurred;
			return num3 + num4 * eventOccurred.GetHashCode();
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as StateTransition<StateType, EventType>);
		}

		public bool Equals(StateTransition<StateType, EventType> other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			if (base.GetType() != other.GetType())
			{
				return false;
			}
			StateType fromState = this.FromState;
			if (fromState.Equals(other.FromState))
			{
				EventType eventOccurred = this.EventOccurred;
				return eventOccurred.Equals(other.EventOccurred);
			}
			return false;
		}
	}
}
