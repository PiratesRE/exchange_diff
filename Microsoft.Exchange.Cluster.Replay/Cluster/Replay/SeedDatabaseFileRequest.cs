using System;
using System.IO;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.ReplicaSeeder;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SeedDatabaseFileRequest : NetworkChannelDatabaseRequest
	{
		internal SeedDatabaseFileRequest(NetworkChannel channel, Guid dbGuid, uint readSizeHint) : base(channel, NetworkChannelMessage.MessageType.SeedDatabaseFileRequest, dbGuid)
		{
			this.m_readHintSize = readSizeHint;
		}

		internal SeedDatabaseFileRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.SeedDatabaseFileRequest, packetContent)
		{
			this.m_readHintSize = base.Packet.ExtractUInt32();
		}

		internal uint ReadSizeHint
		{
			get
			{
				return this.m_readHintSize;
			}
			set
			{
				this.m_readHintSize = value;
			}
		}

		public override void Execute()
		{
			string databaseName = base.Channel.MonitoredDatabase.DatabaseName;
			string partnerNodeName = base.Channel.PartnerNodeName;
			ExTraceGlobals.SeederServerTracer.TraceDebug<string, uint>((long)this.GetHashCode(), "SeedDatabaseFileRequest({0}): readHintSize ({1}).", databaseName, this.m_readHintSize);
			bool flag = false;
			base.Channel.IsSeeding = true;
			Exception ex = null;
			try
			{
				base.Channel.CreateSeederServerContext(base.DatabaseGuid, null, SeedType.Database);
				ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedDatabaseFileRequest. Opening up the backup context for {0}.", base.DatabaseGuid);
				long fileSize;
				this.m_backupHandle = SeedDatabaseFileRequest.OpenSeedStreamer(base.DatabaseGuid, this.m_readHintSize, out fileSize);
				SeedDatabaseFileReply seedDatabaseFileReply = new SeedDatabaseFileReply(base.Channel);
				seedDatabaseFileReply.FileSize = fileSize;
				seedDatabaseFileReply.LastWriteUtc = DateTime.UtcNow;
				seedDatabaseFileReply.Send();
				ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedDatabaseFileRequest. Sending the data for {0}.", base.DatabaseGuid);
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3343265085U);
				base.Channel.SendSeedingDataTransferReply(seedDatabaseFileReply, new ReadDatabaseCallback(this.ReadDbCallback));
				flag = true;
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
			catch (FailedToOpenBackupFileHandleException ex6)
			{
				ex = ex6;
			}
			catch (FileIOonSourceException ex7)
			{
				ex = ex7;
			}
			catch (NetworkTransportException ex8)
			{
				ex = ex8;
			}
			catch (ADExternalException ex9)
			{
				ex = ex9;
			}
			catch (ADOperationException ex10)
			{
				ex = ex10;
			}
			catch (ADTransientException ex11)
			{
				ex = ex11;
			}
			catch (TaskServerException ex12)
			{
				ex = ex12;
			}
			catch (TransientException ex13)
			{
				ex = ex13;
			}
			finally
			{
				if (ex != null)
				{
					ExTraceGlobals.SeederServerTracer.TraceError<string, Exception>((long)this.GetHashCode(), "SeedDatabaseFileRequest({0}): Failed: {1}", databaseName, ex);
					ReplayCrimsonEvents.ActiveSeedSourceFailedToSendEDB.Log<string, string, string, string>(databaseName, Environment.MachineName, partnerNodeName, ex.ToString());
				}
				if (this.m_backupHandle != null)
				{
					int lastError = flag ? 0 : 42;
					this.m_backupHandle.SetLastError(lastError);
					this.m_backupHandle.Close();
					this.m_backupHandle = null;
				}
				if (ex != null)
				{
					if (ex is NetworkTransportException)
					{
						base.Channel.KeepAlive = false;
					}
					else
					{
						base.Channel.SendException(ex);
					}
				}
			}
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_readHintSize);
		}

		private static EseDatabaseBackupReader OpenSeedStreamer(Guid dbGuid, uint readHintSize, out long initialFileSizeBytes)
		{
			string machineName = Environment.MachineName;
			initialFileSizeBytes = 0L;
			string dbName;
			string sourceFileToBackupFullPath;
			SeedHelper.GetDatabaseNameAndPath(dbGuid, out dbName, out sourceFileToBackupFullPath);
			CReplicaSeederInterop.SetupNativeLogger();
			EseDatabaseBackupReader esedatabaseBackupReader = EseDatabaseBackupReader.GetESEDatabaseBackupReader(machineName, dbName, dbGuid, null, sourceFileToBackupFullPath, readHintSize);
			initialFileSizeBytes = esedatabaseBackupReader.SourceFileSizeBytes;
			return esedatabaseBackupReader;
		}

		private int ReadDbCallback(byte[] buf, ulong readOffset, int bytesToRead)
		{
			if (buf == null)
			{
				throw new ArgumentNullException("buf");
			}
			if (buf.Length != 65536)
			{
				throw new ArgumentException("buf");
			}
			if (this.m_backupHandle == null)
			{
				throw new ArgumentNullException("m_backupHandle");
			}
			return this.m_backupHandle.PerformDatabaseRead((long)readOffset, buf, bytesToRead);
		}

		private uint m_readHintSize;

		private EseDatabaseBackupReader m_backupHandle;
	}
}
