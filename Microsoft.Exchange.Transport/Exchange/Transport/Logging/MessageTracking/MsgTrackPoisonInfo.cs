using System;
using System.Net;

namespace Microsoft.Exchange.Transport.Logging.MessageTracking
{
	internal class MsgTrackPoisonInfo
	{
		internal IPAddress ClientIPAddress { get; private set; }

		internal string ClientHostName { get; private set; }

		internal IPAddress ServerIPAddress { get; private set; }

		internal string SourceContext { get; private set; }

		public MsgTrackPoisonInfo(IPAddress clientIPAddress, string clientHostName, IPAddress serverIPAddress, string sourceContext)
		{
			this.ClientIPAddress = clientIPAddress;
			this.ClientHostName = clientHostName;
			this.ServerIPAddress = serverIPAddress;
			this.SourceContext = sourceContext;
		}
	}
}
