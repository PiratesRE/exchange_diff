using System;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SeedPageReaderMultiplePageRequest : NetworkChannelDatabaseRequest
	{
		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_databaseName);
			base.Packet.Append(this.m_databasePath);
			base.Packet.Append(this.m_numPages);
		}

		internal SeedPageReaderMultiplePageRequest(NetworkChannel channel, Guid databaseGuid, string databaseName, string databasePath, long numPages) : base(channel, NetworkChannelMessage.MessageType.SeedPageReaderMultiplePageRequest, databaseGuid)
		{
			this.m_databaseName = databaseName;
			this.m_databasePath = databasePath;
			this.m_numPages = numPages;
		}

		internal SeedPageReaderMultiplePageRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.SeedPageReaderMultiplePageRequest, packetContent)
		{
			this.m_databaseName = base.Packet.ExtractString();
			this.m_databasePath = base.Packet.ExtractString();
			this.m_numPages = base.Packet.ExtractInt64();
		}

		public override void Execute()
		{
			SeedPageReaderMultiplePageRequest.Tracer.TraceDebug<Guid, long>((long)this.GetHashCode(), "SeedPageReaderMultiplePageRequest: databaseGuid ({0}), numPages ({1}).", base.DatabaseGuid, this.m_numPages);
			bool flag = false;
			ReplayStopwatch replayStopwatch = new ReplayStopwatch();
			Exception ex = null;
			int i = (int)this.m_numPages;
			byte[] array = new byte[4];
			try
			{
				SeederPageReaderServerContext seederPageReaderServerContext = base.Channel.GetSeederPageReaderServerContext(this.m_databaseName, this.m_databasePath);
				replayStopwatch.Start();
				while (i > 0)
				{
					base.Channel.Read(array, 0, 4);
					i--;
					uint num = BitConverter.ToUInt32(array, 0);
					long lowGeneration;
					long highGeneration;
					byte[] pageBytes = seederPageReaderServerContext.DatabaseReader.ReadOnePage((long)((ulong)num), out lowGeneration, out highGeneration);
					new SeedPageReaderSinglePageReply(base.Channel)
					{
						PageNumber = (long)((ulong)num),
						LowGeneration = lowGeneration,
						HighGeneration = highGeneration,
						PageBytes = pageBytes
					}.Send();
				}
				flag = true;
			}
			catch (JetErrorFileIOBeyondEOFException ex2)
			{
				ex = ex2;
			}
			catch (FailedToReadDatabasePage failedToReadDatabasePage)
			{
				ex = failedToReadDatabasePage;
			}
			catch (NetworkTransportException ex3)
			{
				ex = ex3;
			}
			finally
			{
				SeedPageReaderMultiplePageRequest.Tracer.TraceDebug<long, long, string>((long)this.GetHashCode(), "SeedPageReader finished reading and sending {0} pages after {1} sec. Operation successful: {2}", this.m_numPages, replayStopwatch.ElapsedMilliseconds / 1000L, flag.ToString());
			}
			if (!flag)
			{
				SeedPageReaderMultiplePageRequest.Tracer.TraceError<Exception>((long)this.GetHashCode(), "SeedPageReaderMultiplePageRequest.Execute failed: {0}", ex);
				if (ex is NetworkTransportException)
				{
					base.Channel.KeepAlive = false;
					return;
				}
				base.Channel.SendException(ex);
				while (i > 0)
				{
					base.Channel.Read(array, 0, 4);
					i--;
				}
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.IncrementalReseederTracer;

		private readonly long m_numPages;

		private readonly string m_databaseName;

		private readonly string m_databasePath;
	}
}
