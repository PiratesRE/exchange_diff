using System;
using System.IO;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class PassiveSeedDatabaseFileRequest : NetworkChannelDatabaseRequest
	{
		internal PassiveSeedDatabaseFileRequest(NetworkChannel channel, Guid dbGuid, Guid targetServerGuid) : base(channel, NetworkChannelMessage.MessageType.PassiveDatabaseFileRequest, dbGuid)
		{
			this.m_serverGuid = targetServerGuid;
		}

		internal PassiveSeedDatabaseFileRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.PassiveDatabaseFileRequest, packetContent)
		{
			this.m_serverGuid = base.Packet.ExtractGuid();
		}

		public override void Execute()
		{
			string databaseName = base.Channel.MonitoredDatabase.DatabaseName;
			string partnerNodeName = base.Channel.PartnerNodeName;
			ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "PassiveSeedDatabaseFileRequest: databaseGuid ({0}).", base.DatabaseGuid);
			Exception ex = null;
			try
			{
				SeederServerContext seederServerContext = base.Channel.CreateSeederServerContext(base.DatabaseGuid, new Guid?(this.m_serverGuid), SeedType.Database);
				seederServerContext.SendDatabaseFile();
			}
			catch (OperationCanceledException ex2)
			{
				ex = ex2;
			}
			catch (SeedingChannelIsClosedException ex3)
			{
				ex = ex3;
			}
			catch (SeedingSourceReplicaInstanceNotFoundException ex4)
			{
				ex = ex4;
			}
			catch (IOException ex5)
			{
				ex = ex5;
			}
			catch (FileIOonSourceException ex6)
			{
				ex = ex6;
			}
			catch (NetworkTransportException ex7)
			{
				ex = ex7;
			}
			catch (MapiPermanentException ex8)
			{
				ex = ex8;
			}
			catch (TaskServerException ex9)
			{
				ex = ex9;
			}
			catch (TransientException ex10)
			{
				ex = ex10;
			}
			if (ex != null)
			{
				ExTraceGlobals.SeederServerTracer.TraceError<string, Exception>((long)this.GetHashCode(), "PassiveSeedDatabaseFileRequest({0}): Failed: {1}", databaseName, ex);
				ReplayCrimsonEvents.PassiveSeedSourceFailedToSendEDB.Log<string, string, string, string>(databaseName, Environment.MachineName, partnerNodeName, ex.ToString());
				if (!(ex is NetworkTransportException))
				{
					base.Channel.SendException(ex);
				}
			}
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_serverGuid);
		}

		private Guid m_serverGuid;
	}
}
