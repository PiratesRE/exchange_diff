using System;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal struct TransitionLogEntry
	{
		internal TransitionLogEntry(long sequence, uint currentState, uint signal)
		{
			this.sequence = sequence;
			this.currentState = currentState;
			this.signal = signal;
		}

		internal long Sequence
		{
			get
			{
				return this.sequence;
			}
		}

		internal uint CurrentState
		{
			get
			{
				return this.currentState;
			}
		}

		internal uint Signal
		{
			get
			{
				return this.signal;
			}
		}

		private readonly long sequence;

		private readonly uint currentState;

		private readonly uint signal;
	}
}
