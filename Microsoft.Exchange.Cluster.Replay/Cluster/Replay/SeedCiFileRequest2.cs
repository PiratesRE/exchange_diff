using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SeedCiFileRequest2 : SeedCiFileRequest
	{
		internal SeedCiFileRequest2(NetworkChannel channel, Guid dbGuid, string endpoint, string reason) : base(channel, NetworkChannelMessage.MessageType.SeedCiFileRequest2, dbGuid, endpoint, reason)
		{
		}

		internal SeedCiFileRequest2(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.SeedCiFileRequest2, packetContent)
		{
			string xml = base.Packet.ExtractString();
			SeedCiFileRequestPayload seedCiFileRequestPayload;
			Exception ex = DataContractSerializeHelper.DeserializeFromXmlString<SeedCiFileRequestPayload>(xml, out seedCiFileRequestPayload);
			if (ex != null)
			{
				ReplayCrimsonEvents.CISeedingSourceBeginFailed.Log<Guid, string, string, Exception>(base.DatabaseGuid, string.Empty, base.Channel.PartnerNodeName, ex);
				SeederServerContext.ProcessSourceSideException(ex, base.Channel);
				return;
			}
			this.endpoint = seedCiFileRequestPayload.Endpoint;
			this.reason = seedCiFileRequestPayload.Reason;
		}

		protected override void SerializeAdditionalProperties()
		{
			SeedCiFileRequestPayload toSerialize = new SeedCiFileRequestPayload(this.endpoint, this.reason);
			string str;
			Exception ex = DataContractSerializeHelper.SerializeToXmlString(toSerialize, out str);
			if (ex != null)
			{
				throw new SeederServerException(ex.Message, ex);
			}
			base.Packet.Append(str);
		}
	}
}
