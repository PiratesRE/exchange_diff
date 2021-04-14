using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SeedLogCopyRequest : NetworkChannelDatabaseRequest
	{
		internal SeedLogCopyRequest(NetworkChannel channel, Guid dbGuid) : base(channel, NetworkChannelMessage.MessageType.SeedLogCopyRequest, dbGuid)
		{
		}

		internal SeedLogCopyRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.SeedLogCopyRequest, packetContent)
		{
		}

		public override void Execute()
		{
			Exception ex = SeederServerContext.RunSeedSourceAction(delegate
			{
				SeederServerContext seederServerContext = base.Channel.GetSeederServerContext(base.DatabaseGuid);
				seederServerContext.SendLogFiles();
			});
			if (ex != null)
			{
				ReplayCrimsonEvents.LogSeedingSourceError.Log<Guid, string, string, string>(base.DatabaseGuid, string.Empty, base.Channel.PartnerNodeName, ex.ToString());
				SeederServerContext.ProcessSourceSideException(ex, base.Channel);
			}
		}
	}
}
