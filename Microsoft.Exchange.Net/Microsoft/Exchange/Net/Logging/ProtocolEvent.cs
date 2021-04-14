using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Logging
{
	internal struct ProtocolEvent
	{
		public ProtocolEvent(string id)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("id", id);
			this.id = id;
		}

		public override string ToString()
		{
			return this.id;
		}

		public static readonly ProtocolEvent Connect = new ProtocolEvent("+");

		public static readonly ProtocolEvent Disconnect = new ProtocolEvent("-");

		public static readonly ProtocolEvent Send = new ProtocolEvent(">");

		public static readonly ProtocolEvent Receive = new ProtocolEvent("<");

		public static readonly ProtocolEvent Information = new ProtocolEvent("*");

		private string id;
	}
}
