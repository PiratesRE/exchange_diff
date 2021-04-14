using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class PassiveReadRequest
	{
		public NetworkChannel Channel { get; set; }

		public PassiveBlockMode Manager { get; set; }

		public bool CompletedSynchronously { get; set; }

		public bool CompletionWasProcessed { get; set; }

		public PassiveReadRequest(PassiveBlockMode mgr, NetworkChannel channel)
		{
			this.Channel = channel;
			this.Manager = mgr;
		}
	}
}
