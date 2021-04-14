using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SeedCiFileRequest : NetworkChannelDatabaseRequest
	{
		internal SeedCiFileRequest(NetworkChannel channel, Guid dbGuid, string endpoint) : this(channel, NetworkChannelMessage.MessageType.SeedCiFileRequest, dbGuid, endpoint, null)
		{
		}

		internal SeedCiFileRequest(NetworkChannel channel, byte[] packetContent) : this(channel, NetworkChannelMessage.MessageType.SeedCiFileRequest, packetContent)
		{
			this.endpoint = base.Packet.ExtractString();
		}

		protected SeedCiFileRequest(NetworkChannel channel, NetworkChannelMessage.MessageType messageType, Guid dbGuid, string endpoint, string reason) : base(channel, messageType, dbGuid)
		{
			this.endpoint = endpoint;
			this.reason = reason;
		}

		protected SeedCiFileRequest(NetworkChannel channel, NetworkChannelMessage.MessageType messageType, byte[] packetContent) : base(channel, messageType, packetContent)
		{
		}

		public override void Execute()
		{
			Exception ex = SeederServerContext.RunSeedSourceAction(delegate
			{
				SeederServerContext seederServerContext = base.Channel.CreateSeederServerContext(base.DatabaseGuid, null, SeedType.Catalog);
				seederServerContext.SeedToEndpoint(this.endpoint, this.reason);
				ReplayCrimsonEvents.CISeedingSourceBeginSucceeded.Log<Guid, string, string, string>(base.DatabaseGuid, seederServerContext.DatabaseName, this.endpoint, string.Empty);
			});
			if (ex != null)
			{
				ReplayCrimsonEvents.CISeedingSourceBeginFailed.Log<Guid, string, string, string>(base.DatabaseGuid, string.Empty, this.endpoint, ex.ToString());
				SeederServerContext.ProcessSourceSideException(ex, base.Channel);
			}
		}

		protected override void Serialize()
		{
			base.Serialize();
			this.SerializeAdditionalProperties();
		}

		protected virtual void SerializeAdditionalProperties()
		{
			base.Packet.Append(this.endpoint);
		}

		protected string reason;

		protected string endpoint;
	}
}
