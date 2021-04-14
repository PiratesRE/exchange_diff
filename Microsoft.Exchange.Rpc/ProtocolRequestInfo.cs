using System;

namespace Microsoft.Exchange.Rpc
{
	public abstract class ProtocolRequestInfo
	{
		public abstract string[] RequestIds { get; }

		public abstract string[] Cookies { get; }

		public abstract string ClientAddress { get; }

		public ProtocolRequestInfo()
		{
		}
	}
}
