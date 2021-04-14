using System;

namespace Microsoft.Exchange.TextMatching
{
	internal abstract class StateNode
	{
		public StateNode(int state, bool finalState)
		{
			this.state = state;
			this.finalState = finalState;
		}

		public int State
		{
			get
			{
				return this.state;
			}
		}

		public bool IsFinal
		{
			get
			{
				return this.finalState;
			}
			set
			{
				this.finalState = value;
			}
		}

		private int state = -1;

		private bool finalState;
	}
}
