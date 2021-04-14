using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ProgressCiFileRequest : NetworkChannelDatabaseRequest
	{
		internal ProgressCiFileRequest(NetworkChannel channel, Guid dbGuid, string handle) : base(channel, NetworkChannelMessage.MessageType.ProgressCiFileRequest, dbGuid)
		{
			this.handle = handle;
		}

		internal ProgressCiFileRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.ProgressCiFileRequest, packetContent)
		{
			this.handle = base.Packet.ExtractString();
		}

		public override void Execute()
		{
			Exception ex = SeederServerContext.RunSeedSourceAction(delegate
			{
				SeederServerContext seederServerContext = base.Channel.GetSeederServerContext(base.DatabaseGuid);
				seederServerContext.GetSeedingProgress(this.handle);
			});
			if (ex != null)
			{
				ReplayCrimsonEvents.CISeedingSourceError.Log<Guid, string, string, string>(base.DatabaseGuid, string.Empty, string.Empty, ex.ToString());
				SeederServerContext.ProcessSourceSideException(ex, base.Channel);
			}
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.handle);
		}

		private readonly string handle;
	}
}
