using System;
using System.IO;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SeedPageReaderSinglePageRequest : NetworkChannelDatabaseRequest
	{
		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_databaseName);
			base.Packet.Append(this.m_databasePath);
			base.Packet.Append(this.m_pageno);
		}

		internal SeedPageReaderSinglePageRequest(NetworkChannel channel, Guid databaseGuid, string databaseName, string databasePath, uint pageno) : base(channel, NetworkChannelMessage.MessageType.SeedPageReaderSinglePageRequest, databaseGuid)
		{
			this.m_databaseName = databaseName;
			this.m_databasePath = databasePath;
			this.m_pageno = pageno;
		}

		internal SeedPageReaderSinglePageRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.SeedPageReaderSinglePageRequest, packetContent)
		{
			this.m_databaseName = base.Packet.ExtractString();
			this.m_databasePath = base.Packet.ExtractString();
			this.m_pageno = base.Packet.ExtractUInt32();
		}

		public override void Execute()
		{
			ExTraceGlobals.IncrementalReseederTracer.TraceDebug<Guid, uint>((long)this.GetHashCode(), "SeedPageReaderSinglePageRequest: databaseGuid ({0}), pageno ({1}).", base.DatabaseGuid, this.m_pageno);
			bool flag = false;
			ReplayStopwatch replayStopwatch = new ReplayStopwatch();
			try
			{
				SeederPageReaderServerContext seederPageReaderServerContext = base.Channel.GetSeederPageReaderServerContext(this.m_databaseName, this.m_databasePath);
				replayStopwatch.Start();
				ExTraceGlobals.IncrementalReseederTracer.TraceDebug<uint, Guid>((long)this.GetHashCode(), "SeedPageReaderSinglePageRequest. Reading page {0} from {1}.", this.m_pageno, base.DatabaseGuid);
				long lowGeneration;
				long highGeneration;
				byte[] pageBytes = seederPageReaderServerContext.DatabaseReader.ReadOnePage((long)((ulong)this.m_pageno), out lowGeneration, out highGeneration);
				SeedPageReaderSinglePageReply seedPageReaderSinglePageReply = new SeedPageReaderSinglePageReply(base.Channel);
				seedPageReaderSinglePageReply.PageNumber = (long)((ulong)this.m_pageno);
				seedPageReaderSinglePageReply.LowGeneration = lowGeneration;
				seedPageReaderSinglePageReply.HighGeneration = highGeneration;
				seedPageReaderSinglePageReply.PageBytes = pageBytes;
				ExTraceGlobals.IncrementalReseederTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedPageReaderSinglePageRequest. Sending the data for {0}.", base.DatabaseGuid);
				seedPageReaderSinglePageReply.Send();
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
				ExTraceGlobals.IncrementalReseederTracer.TraceDebug<uint, long, string>((long)this.GetHashCode(), "SeedPageReader finished reading and sending page {0} after {1} sec. Operation successful: {2}", this.m_pageno, replayStopwatch.ElapsedMilliseconds / 1000L, flag.ToString());
			}
		}

		private uint m_pageno;

		private string m_databaseName;

		private string m_databasePath;
	}
}
