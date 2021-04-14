using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class RemoteDestinationInfo : IDestinationInfo
	{
		public Uri Destination { get; private set; }

		public IEnumerable<string> ChannelIds
		{
			get
			{
				return this.channelIds;
			}
		}

		public RemoteDestinationInfo(Uri destination, string channelId)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			this.Destination = destination;
			this.AddChannel(channelId);
		}

		public void AddChannel(string channelId)
		{
			if (string.IsNullOrWhiteSpace(channelId))
			{
				throw new ArgumentException("channelId cannot be null or whitespace.");
			}
			this.channelIds.Add(channelId);
		}

		public override string ToString()
		{
			return string.Format("Uri - {0}, Channels - {1}", this.Destination, string.Join(",", this.ChannelIds));
		}

		private List<string> channelIds = new List<string>();
	}
}
