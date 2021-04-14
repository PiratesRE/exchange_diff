using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal class ClientPerformanceEventArgs
	{
		public ClientPerformanceEventType EventType { get; set; }

		public ClientPerformanceEventArgs(ClientPerformanceEventType eventType)
		{
			this.EventType = eventType;
		}
	}
}
