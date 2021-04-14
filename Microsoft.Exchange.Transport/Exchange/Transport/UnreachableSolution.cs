using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport
{
	internal class UnreachableSolution : NextHopSolution
	{
		public UnreachableSolution(NextHopSolutionKey key) : base(key)
		{
			this.reasons = UnreachableReason.None;
		}

		public UnreachableReason Reasons
		{
			get
			{
				return this.reasons;
			}
		}

		public void AddUnreachableReason(UnreachableReason reason)
		{
			this.reasons |= reason;
		}

		private UnreachableReason reasons;
	}
}
