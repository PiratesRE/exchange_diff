using System;
using System.IO;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class TestLogExistenceRequest : NetworkChannelDatabaseRequest
	{
		internal TestLogExistenceRequest(NetworkChannel channel, Guid dbGuid, long logNum) : base(channel, NetworkChannelMessage.MessageType.TestLogExistenceRequest, dbGuid)
		{
			this.m_logGeneration = logNum;
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_logGeneration);
		}

		internal TestLogExistenceRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.TestLogExistenceRequest, packetContent)
		{
			this.m_logGeneration = base.Packet.ExtractInt64();
		}

		public override void Execute()
		{
			try
			{
				string path = base.Channel.MonitoredDatabase.BuildLogFileName(this.m_logGeneration);
				bool logExists = File.Exists(path);
				new TestLogExistenceReply(base.Channel)
				{
					LogExists = logExists
				}.Send();
			}
			catch (IOException ex)
			{
				base.Channel.SendException(ex);
				base.Channel.Close();
			}
		}

		private long m_logGeneration;
	}
}
