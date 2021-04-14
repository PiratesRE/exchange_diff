using System;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	internal sealed class RpcAttemptedCounters : IRpcCounters
	{
		public void IncrementCounter(IRpcCounterData counterData)
		{
			this.rpcAttemptedCounter++;
		}

		public override string ToString()
		{
			return string.Format("R={0}", this.rpcAttemptedCounter);
		}

		private int rpcAttemptedCounter;
	}
}
