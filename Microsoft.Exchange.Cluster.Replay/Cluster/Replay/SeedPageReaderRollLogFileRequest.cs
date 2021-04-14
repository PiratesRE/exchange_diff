using System;
using System.IO;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SeedPageReaderRollLogFileRequest : NetworkChannelDatabaseRequest
	{
		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_databaseName);
			base.Packet.Append(this.m_databasePath);
		}

		internal SeedPageReaderRollLogFileRequest(NetworkChannel channel, Guid databaseGuid, string databaseName, string databasePath) : base(channel, NetworkChannelMessage.MessageType.SeedPageReaderRollLogFileRequest, databaseGuid)
		{
			this.m_databaseName = databaseName;
			this.m_databasePath = databasePath;
		}

		internal SeedPageReaderRollLogFileRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.SeedPageReaderRollLogFileRequest, packetContent)
		{
			this.m_databaseName = base.Packet.ExtractString();
			this.m_databasePath = base.Packet.ExtractString();
		}

		public override void Execute()
		{
			ExTraceGlobals.IncrementalReseederTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedPageReaderRollLogFileRequest: databaseGuid ({0}).", base.DatabaseGuid);
			bool flag = false;
			ReplayStopwatch replayStopwatch = new ReplayStopwatch();
			try
			{
				SeederPageReaderServerContext seederPageReaderServerContext = base.Channel.GetSeederPageReaderServerContext(this.m_databaseName, this.m_databasePath);
				replayStopwatch.Start();
				seederPageReaderServerContext.DatabaseReader.ForceNewLog();
				SeedPageReaderRollLogFileReply seedPageReaderRollLogFileReply = new SeedPageReaderRollLogFileReply(base.Channel);
				ExTraceGlobals.IncrementalReseederTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedPageReaderRollLogFileRequest. Sending the response for {0}.", base.DatabaseGuid);
				seedPageReaderRollLogFileReply.Send();
				flag = true;
			}
			catch (TransientException ex)
			{
				base.Channel.SendException(ex);
			}
			catch (IOException ex2)
			{
				base.Channel.SendException(ex2);
			}
			finally
			{
				ExTraceGlobals.IncrementalReseederTracer.TraceDebug<long, string>((long)this.GetHashCode(), "SeedPageReaderRollLogFile finished rolling a log file after {0} sec. Operation successful: {1}", replayStopwatch.ElapsedMilliseconds / 1000L, flag.ToString());
			}
		}

		private string m_databaseName;

		private string m_databasePath;
	}
}
