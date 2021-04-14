using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public interface IDestinationInfo
	{
		Uri Destination { get; }

		IEnumerable<string> ChannelIds { get; }

		void AddChannel(string channelId);
	}
}
