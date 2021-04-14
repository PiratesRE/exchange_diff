using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CancelCiFileRequest : NetworkChannelDatabaseRequest
	{
		internal CancelCiFileRequest(NetworkChannel channel, Guid dbGuid) : base(channel, NetworkChannelMessage.MessageType.CancelCiFileRequest, dbGuid)
		{
		}

		internal CancelCiFileRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.CancelCiFileRequest, packetContent)
		{
		}

		public override void Execute()
		{
			Exception ex = SeederServerContext.RunSeedSourceAction(delegate
			{
				SeederServerContext seederServerContext = base.Channel.GetSeederServerContext(base.DatabaseGuid);
				seederServerContext.HandleCancelCiFileRequest(null);
			});
			if (ex != null)
			{
				ReplayCrimsonEvents.CISeedingSourceError.Log<Guid, string, string, string>(base.DatabaseGuid, string.Empty, string.Empty, ex.ToString());
				SeederServerContext.ProcessSourceSideException(ex, base.Channel);
			}
		}
	}
}
