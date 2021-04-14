using System;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	internal interface IRpcCounters
	{
		void IncrementCounter(IRpcCounterData counterData);
	}
}
