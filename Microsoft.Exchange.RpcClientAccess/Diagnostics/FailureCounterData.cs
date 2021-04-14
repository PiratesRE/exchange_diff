using System;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	internal class FailureCounterData : IRpcCounterData
	{
		public uint FailureCode { get; set; }
	}
}
