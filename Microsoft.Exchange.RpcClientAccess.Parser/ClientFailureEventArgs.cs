using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal class ClientFailureEventArgs : ClientTimeStampedEventArgs
	{
		public uint FailureCode { get; private set; }

		public ClientFailureEventArgs(uint blockTimeSinceRequest, ClientPerformanceEventType eventType, uint failureCode) : base(blockTimeSinceRequest, eventType)
		{
			this.FailureCode = failureCode;
		}
	}
}
