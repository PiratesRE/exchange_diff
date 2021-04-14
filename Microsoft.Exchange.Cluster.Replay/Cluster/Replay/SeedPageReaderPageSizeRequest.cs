using System;
using System.IO;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SeedPageReaderPageSizeRequest : NetworkChannelDatabaseRequest
	{
		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_databaseName);
			base.Packet.Append(this.m_databasePath);
		}

		internal SeedPageReaderPageSizeRequest(NetworkChannel channel, Guid databaseGuid, string databaseName, string databasePath) : base(channel, NetworkChannelMessage.MessageType.SeedPageReaderPageSizeRequest, databaseGuid)
		{
			this.m_databaseName = databaseName;
			this.m_databasePath = databasePath;
		}

		internal SeedPageReaderPageSizeRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.SeedPageReaderPageSizeRequest, packetContent)
		{
			this.m_databaseName = base.Packet.ExtractString();
			this.m_databasePath = base.Packet.ExtractString();
		}

		public override void Execute()
		{
			ExTraceGlobals.IncrementalReseederTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedPageReaderPageSizeRequest: databaseGuid ({0}).", base.DatabaseGuid);
			bool flag = false;
			ReplayStopwatch replayStopwatch = new ReplayStopwatch();
			try
			{
				SeederPageReaderServerContext seederPageReaderServerContext = base.Channel.GetSeederPageReaderServerContext(this.m_databaseName, this.m_databasePath);
				replayStopwatch.Start();
				long pageSize = seederPageReaderServerContext.DatabaseReader.PageSize;
				SeedPageReaderPageSizeReply seedPageReaderPageSizeReply = new SeedPageReaderPageSizeReply(base.Channel);
				ExTraceGlobals.IncrementalReseederTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedPageReaderPageSizeRequest. Sending the response for {0}.", base.DatabaseGuid);
				seedPageReaderPageSizeReply.PageSize = pageSize;
				seedPageReaderPageSizeReply.Send();
				flag = true;
			}
			catch (IOException ex)
			{
				base.Channel.SendException(ex);
			}
			catch (TransientException ex2)
			{
				base.Channel.SendException(ex2);
			}
			finally
			{
				ExTraceGlobals.IncrementalReseederTracer.TraceDebug<long, string>((long)this.GetHashCode(), "SeedPageReaderPageRequest finished fetching the page size after {0} msec. Operation successful: {1}", replayStopwatch.ElapsedMilliseconds, flag.ToString());
			}
		}

		private string m_databaseName;

		private string m_databasePath;
	}
}
