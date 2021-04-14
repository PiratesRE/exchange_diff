using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Mapi;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SeederServerContext
	{
		public bool IsCatalogSeed
		{
			get
			{
				return this.SeedType == SeedType.Catalog;
			}
		}

		private static FileStream OpenFileStream(SafeFileHandle fileHandle, bool openForRead)
		{
			return LogCopy.OpenFileStream(fileHandle, openForRead);
		}

		internal Guid DatabaseGuid
		{
			get
			{
				return this.m_databaseGuid;
			}
		}

		internal Guid? TargetServerGuid
		{
			get
			{
				return this.m_targetServerGuid;
			}
		}

		internal string TargetServerName
		{
			get
			{
				if (this.m_targetServer == null && this.m_targetServerGuid != null)
				{
					IReplayAdObjectLookup replayAdObjectLookup = Dependencies.ReplayAdObjectLookup;
					this.m_targetServer = replayAdObjectLookup.ServerLookup.FindAdObjectByGuid(this.m_targetServerGuid.Value);
				}
				if (this.m_targetServer != null)
				{
					return this.m_targetServer.Name;
				}
				return this.m_channel.PartnerNodeName;
			}
		}

		public string DatabaseName { get; private set; }

		public SeedType SeedType { get; private set; }

		internal SeederServerContext(NetworkChannel channel, MonitoredDatabase database, Guid? targetServerGuid, SeedType seedType)
		{
			this.m_channel = channel;
			this.m_databaseGuid = database.DatabaseGuid;
			this.DatabaseName = database.DatabaseName;
			this.SeedType = seedType;
			this.m_targetServerGuid = targetServerGuid;
			if (database.Config.IsPassiveCopy)
			{
				this.m_fPassiveSeeding = true;
				switch (seedType)
				{
				case SeedType.Database:
					this.m_passiveSeedingSourceContext = PassiveSeedingSourceContextEnum.Database;
					break;
				case SeedType.Catalog:
					this.m_passiveSeedingSourceContext = PassiveSeedingSourceContextEnum.Catalogue;
					break;
				}
			}
			else
			{
				this.m_passiveSeedingSourceContext = PassiveSeedingSourceContextEnum.None;
			}
			if (!TestSupport.IsCatalogSeedDisabled())
			{
				string indexSystemName = FastIndexVersion.GetIndexSystemName(this.m_databaseGuid);
				this.indexSeederSource = new IndexSeeder(indexSystemName);
			}
		}

		internal void TraceDebug(string format, params object[] args)
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug((long)this.GetHashCode(), format, args);
		}

		internal void TraceError(string format, params object[] args)
		{
			ExTraceGlobals.SeederServerTracer.TraceError((long)this.GetHashCode(), format, args);
		}

		internal ConnectionStatus GetConnectionStatus()
		{
			return new ConnectionStatus(this.m_channel.PartnerNodeName, this.m_channel.NetworkName, null, ConnectionDirection.Outgoing, true);
		}

		public static Exception RunSeedSourceAction(Action op)
		{
			Exception result = null;
			try
			{
				op();
			}
			catch (IOException ex)
			{
				result = ex;
			}
			catch (UnauthorizedAccessException ex2)
			{
				result = ex2;
			}
			catch (SeedingChannelIsClosedException ex3)
			{
				result = ex3;
			}
			catch (SeedingSourceReplicaInstanceNotFoundException ex4)
			{
				result = ex4;
			}
			catch (LocalizedException ex5)
			{
				result = ex5;
			}
			catch (OperationCanceledException ex6)
			{
				result = ex6;
			}
			return result;
		}

		public static void ProcessSourceSideException(Exception ex, NetworkChannel channel)
		{
			if (ex is NetworkTransportException || ex is OperationCanceledException)
			{
				channel.Close();
				return;
			}
			channel.SendException(ex);
		}

		internal bool IsFromSameTargetServer(SeederServerContext context)
		{
			return this.m_targetServerGuid == null || context.TargetServerGuid == null || this.m_targetServerGuid.Equals(context.TargetServerGuid);
		}

		internal void StartSeeding()
		{
			lock (this.m_lock)
			{
				this.CheckSeedingCancelled();
				this.TraceDebug("seeding started", new object[0]);
				this.m_channel.IsSeeding = true;
				if (this.m_fPassiveSeeding)
				{
					if (this.m_setPassiveSeedingCallback == null)
					{
						this.m_setPassiveSeedingCallback = this.GetPassiveSeederStatusCallback();
					}
					this.m_setPassiveSeedingCallback.BeginPassiveSeeding(this.m_passiveSeedingSourceContext, false);
					ReplayEventLogConstants.Tuple_SeedingSourceBegin.LogEvent(null, new object[]
					{
						this.DatabaseName,
						this.TargetServerName
					});
					ReplayCrimsonEvents.PassiveSeedingSourceBegin.Log<Guid, string, string, string>(this.m_databaseGuid, this.DatabaseName, this.TargetServerName, string.Empty);
				}
				else
				{
					if (this.m_setActiveSeedingCallback == null)
					{
						this.m_setActiveSeedingCallback = this.GetActiveSeederStatusCallback();
					}
					if (this.m_setActiveSeedingCallback != null)
					{
						this.m_setActiveSeedingCallback.BeginActiveSeeding(this.SeedType);
					}
					ReplayEventLogConstants.Tuple_ActiveSeedingSourceBegin.LogEvent(null, new object[]
					{
						this.DatabaseName,
						this.TargetServerName
					});
					ReplayCrimsonEvents.ActiveSeedingSourceBegin.Log<Guid, string, string, string>(this.m_databaseGuid, this.DatabaseName, this.TargetServerName, string.Empty);
				}
				this.m_fSeedingInProgress = true;
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2269523261U);
			}
		}

		internal bool IsSeeding()
		{
			bool fSeedingInProgress;
			lock (this.m_lock)
			{
				fSeedingInProgress = this.m_fSeedingInProgress;
			}
			return fSeedingInProgress;
		}

		internal void Close()
		{
			lock (this.m_lock)
			{
				if (!this.m_fClosed)
				{
					this.TraceDebug("Seeder server context is being closed.", new object[0]);
					this.m_fSeedingCancelled = true;
					if (this.m_channel.IsSeeding)
					{
						this.m_channel.Abort();
					}
					if (!string.IsNullOrEmpty(this.seedingHandle) && !this.ciSeedingIsCancelled)
					{
						bool cancelNeeded = false;
						long num = 0L;
						try
						{
							num = this.PerformCiSeedingAction(delegate
							{
								int progress = this.indexSeederSource.GetProgress(this.seedingHandle);
								if (progress >= 0 && progress < 100)
								{
									cancelNeeded = true;
									this.indexSeederSource.Cancel(this.seedingHandle, "Seeder server context is being closed.");
									this.ciSeedingIsCancelled = true;
								}
							});
						}
						catch (NetworkTransportException arg)
						{
							string errText = string.Format("SeederServerContext.Close failed: {0}", arg);
							SeederServerContext.Tracer.TraceError((long)this.GetHashCode(), errText);
							ReplayCrimsonEvents.CISeedingSourceError.Log<Guid, string, string, string>(this.DatabaseGuid, this.DatabaseName, this.TargetServerName, errText);
							if (!this.ciSeedingIsCancelled)
							{
								cancelNeeded = true;
								try
								{
									this.PerformCiSeedingAction(delegate
									{
										this.indexSeederSource.Cancel(this.seedingHandle, errText);
										this.ciSeedingIsCancelled = true;
									});
								}
								catch (NetworkTransportException arg2)
								{
									errText = string.Format("SeederServerContext.Close second attempt failed: {0}", arg2);
									ReplayCrimsonEvents.CISeedingSourceError.Log<Guid, string, string, string>(this.DatabaseGuid, this.DatabaseName, this.TargetServerName, errText);
								}
							}
						}
						SeederServerContext.Tracer.TraceError<long, bool>((long)this.GetHashCode(), "CI Check/Cancel took {0}ms. Cancel={1}", num, cancelNeeded);
						if (cancelNeeded)
						{
							string text = string.Format("CI seed cancelled by Close() in {0}ms", num);
							if (this.m_fPassiveSeeding)
							{
								ReplayCrimsonEvents.PassiveSeedingSourceCancel.Log<Guid, string, string, string>(this.m_databaseGuid, this.DatabaseName, this.TargetServerName, text);
							}
							else
							{
								ReplayCrimsonEvents.ActiveSeedingSourceCancel.Log<Guid, string, string, string>(this.m_databaseGuid, this.DatabaseName, this.TargetServerName, text);
							}
						}
					}
					if (this.IsSeeding())
					{
						if (this.m_fPassiveSeeding)
						{
							this.m_setPassiveSeedingCallback.EndPassiveSeeding();
							ReplayEventLogConstants.Tuple_SeedingSourceEnd.LogEvent(null, new object[]
							{
								this.DatabaseName,
								this.TargetServerName
							});
							ReplayCrimsonEvents.PassiveSeedingSourceEnd.Log<Guid, string, string, string>(this.m_databaseGuid, this.DatabaseName, this.TargetServerName, string.Empty);
						}
						else
						{
							if (this.m_setActiveSeedingCallback != null)
							{
								this.m_setActiveSeedingCallback.EndActiveSeeding();
							}
							ReplayEventLogConstants.Tuple_ActiveSeedingSourceEnd.LogEvent(null, new object[]
							{
								this.DatabaseName,
								this.TargetServerName
							});
							ReplayCrimsonEvents.ActiveSeedingSourceEnd.Log<Guid, string, string, string>(this.m_databaseGuid, this.DatabaseName, this.TargetServerName, string.Empty);
						}
					}
					this.m_fSeedingInProgress = false;
					if (this.indexSeederSource != null)
					{
						this.indexSeederSource.Dispose();
					}
					this.m_fClosed = true;
				}
			}
		}

		internal void CancelSeeding(LocalizedString message)
		{
			lock (this.m_lock)
			{
				this.TraceDebug("Seeder server context is being cancelled.", new object[0]);
				this.m_fSeedingCancelled = true;
				if (this.m_channel.IsSeeding)
				{
					this.m_channel.Abort();
				}
				if (this.IsSeeding())
				{
					if (this.m_fPassiveSeeding)
					{
						this.m_setPassiveSeedingCallback.EndPassiveSeeding();
						ReplayEventLogConstants.Tuple_SeedingSourceCancel.LogEvent(null, new object[]
						{
							this.DatabaseName,
							this.TargetServerName,
							message
						});
						ReplayCrimsonEvents.PassiveSeedingSourceCancel.Log<Guid, string, string, LocalizedString>(this.m_databaseGuid, this.DatabaseName, this.TargetServerName, message);
					}
					else
					{
						if (this.m_setActiveSeedingCallback != null)
						{
							this.m_setActiveSeedingCallback.EndActiveSeeding();
						}
						ReplayEventLogConstants.Tuple_ActiveSeedingSourceCancel.LogEvent(null, new object[]
						{
							this.DatabaseName,
							this.TargetServerName,
							message
						});
						ReplayCrimsonEvents.ActiveSeedingSourceCancel.Log<Guid, string, string, LocalizedString>(this.m_databaseGuid, this.DatabaseName, this.TargetServerName, message);
					}
				}
				this.m_fSeedingInProgress = false;
			}
		}

		private void CheckSeedingCancelled()
		{
			if (this.m_fSeedingCancelled)
			{
				throw new OperationCanceledException();
			}
		}

		private MonitoredDatabase GetMonitoredDatabase()
		{
			MonitoredDatabase monitoredDatabase = MonitoredDatabase.FindMonitoredDatabase(this.m_channel.LocalNodeName, this.DatabaseGuid);
			if (monitoredDatabase == null)
			{
				throw new SourceDatabaseNotFoundException(this.DatabaseGuid, this.m_channel.LocalNodeName);
			}
			return monitoredDatabase;
		}

		public void SendLogFiles()
		{
			MonitoredDatabase monitoredDatabase = this.GetMonitoredDatabase();
			string suffix = '.' + monitoredDatabase.Config.LogExtension;
			DirectoryInfo di = new DirectoryInfo(monitoredDatabase.Config.SourceLogPath);
			long num = ShipControl.HighestGenerationInDirectory(di, monitoredDatabase.Config.LogFilePrefix, suffix);
			long num2 = ShipControl.LowestGenerationInDirectory(di, monitoredDatabase.Config.LogFilePrefix, suffix, false);
			for (long num3 = num2; num3 <= num; num3 += 1L)
			{
				this.CheckSeedingCancelled();
				try
				{
					monitoredDatabase.SendLog(num3, this.m_channel, null);
				}
				catch (FileIOonSourceException ex)
				{
					ExTraceGlobals.SeederServerTracer.TraceDebug<long, Exception>((long)this.GetHashCode(), "failed to send generation 0x{0:X} because {1}", num3, ex.InnerException);
					this.m_channel.SendException(ex.InnerException);
					this.CancelSeeding(ex.LocalizedString);
					return;
				}
			}
			NotifyEndOfLogReply notifyEndOfLogReply = new NotifyEndOfLogReply(this.m_channel, NetworkChannelMessage.MessageType.NotifyEndOfLogReply, num, DateTime.UtcNow);
			this.TraceDebug("reached the end of log files", new object[0]);
			notifyEndOfLogReply.Send();
		}

		public void SendDatabaseFile()
		{
			ReplayStopwatch replayStopwatch = new ReplayStopwatch();
			replayStopwatch.Start();
			bool flag = false;
			SafeFileHandle safeFileHandle = null;
			try
			{
				this.TraceDebug("PassiveSeedDatabaseFileRequest. Opening up the backup context for {0}.", new object[]
				{
					this.DatabaseGuid
				});
				MonitoredDatabase monitoredDatabase = this.GetMonitoredDatabase();
				string databaseFullPath = monitoredDatabase.GetDatabaseFullPath();
				int num = 0;
				for (;;)
				{
					this.CheckSeedingCancelled();
					try
					{
						using (IStoreMountDismount storeMountDismountInstance = Dependencies.GetStoreMountDismountInstance(null))
						{
							storeMountDismountInstance.UnmountDatabase(Guid.Empty, monitoredDatabase.Config.IdentityGuid, 16);
							this.TraceDebug("dismounted the replayer database", new object[0]);
						}
					}
					catch (MapiExceptionNotFound)
					{
						this.TraceDebug("replay database is not mounted", new object[0]);
					}
					catch (MapiExceptionTimeout mapiExceptionTimeout)
					{
						this.TraceError("Rethrowing timeout exception: {0}", new object[]
						{
							mapiExceptionTimeout
						});
						throw;
					}
					catch (MapiRetryableException ex)
					{
						if (num++ < 3)
						{
							this.TraceDebug("got {0}, but we will keep retrying for 3 times", new object[]
							{
								ex.ToString()
							});
							Thread.Sleep(1000);
							continue;
						}
						throw;
					}
					catch (MapiPermanentException ex2)
					{
						if (num++ < 3)
						{
							this.TraceDebug("got {0}, but we will keep retrying for 3 times", new object[]
							{
								ex2.ToString()
							});
							Thread.Sleep(1000);
							continue;
						}
						throw;
					}
					break;
				}
				safeFileHandle = this.OpenFile(databaseFullPath, true);
				this.m_passiveDatabaseStream = SeederServerContext.OpenFileStream(safeFileHandle, true);
				SeedDatabaseFileReply seedDatabaseFileReply = new SeedDatabaseFileReply(this.m_channel);
				seedDatabaseFileReply.FileSize = new FileInfo(databaseFullPath).Length;
				seedDatabaseFileReply.LastWriteUtc = DateTime.UtcNow;
				seedDatabaseFileReply.Send();
				ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "PassiveSeedDatabaseFileRequest. Sending the data for {0}.", this.DatabaseGuid);
				this.m_channel.SendSeedingDataTransferReply(seedDatabaseFileReply, new ReadDatabaseCallback(this.ReadDbCallback));
				flag = true;
			}
			finally
			{
				if (this.m_passiveDatabaseStream != null)
				{
					this.m_passiveDatabaseStream.Dispose();
				}
				ExTraceGlobals.SeederServerTracer.TraceDebug<long, string>((long)this.GetHashCode(), "PassiveSeedDatabaseFile finished streaming after {0} sec. Operation successful: {1}", replayStopwatch.ElapsedMilliseconds / 1000L, flag.ToString());
				if (safeFileHandle != null)
				{
					safeFileHandle.Dispose();
				}
			}
		}

		private SafeFileHandle OpenFile(string filename, bool openForRead)
		{
			FileMode creationDisposition;
			FileShare fileShare;
			FileAccess fileAccess;
			if (openForRead)
			{
				creationDisposition = FileMode.Open;
				fileShare = FileShare.None;
				fileAccess = FileAccess.Read;
			}
			else
			{
				creationDisposition = FileMode.Create;
				fileShare = FileShare.None;
				fileAccess = FileAccess.Write;
			}
			FileFlags flags = FileFlags.FILE_FLAG_NO_BUFFERING;
			SafeFileHandle safeFileHandle = NativeMethods.CreateFile(filename, fileAccess, fileShare, IntPtr.Zero, creationDisposition, flags, IntPtr.Zero);
			if (safeFileHandle.IsInvalid)
			{
				throw new IOException(string.Format("CreateFile({0}) = {1}", filename, Marshal.GetLastWin32Error().ToString()), new Win32Exception());
			}
			return safeFileHandle;
		}

		private int ReadDbCallback(byte[] buf, ulong readOffset, int bytesToRead)
		{
			if (buf == null)
			{
				throw new ArgumentNullException("buf");
			}
			if (buf.Length < bytesToRead)
			{
				throw new ArgumentException("buf");
			}
			if (this.m_passiveDatabaseStream == null)
			{
				throw new InvalidOperationException("file handle is null");
			}
			this.CheckSeedingCancelled();
			return this.m_passiveDatabaseStream.Read(buf, 0, bytesToRead);
		}

		internal void SeedToEndpoint(string endpoint, string reason)
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "SeedToEndpoint databaseGuid ({0}), endpoint ({1}).", this.m_databaseGuid, endpoint);
			this.ciSeedStartTimeUtc = ExDateTime.UtcNow;
			long arg = this.PerformCiSeedingAction(delegate
			{
				this.seedingHandle = this.indexSeederSource.SeedToEndPoint(endpoint, reason);
				ExTraceGlobals.SeederServerTracer.TraceDebug<string>((long)this.GetHashCode(), "IndexSeeder.SeedToEndPoint returned handle {0}", this.seedingHandle);
				SeedCiFileReply seedCiFileReply = new SeedCiFileReply(this.m_channel, this.seedingHandle);
				seedCiFileReply.Send();
			});
			ExTraceGlobals.SeederServerTracer.TraceDebug<long>((long)this.GetHashCode(), "SeedToEndpoint finished call after {0}ms.", arg);
		}

		internal void HandleCancelCiFileRequest(string reason)
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "CancelSeeding databaseGuid ({0}), handle ({1}).", this.m_databaseGuid, this.seedingHandle);
			long arg = this.PerformCiSeedingAction(delegate
			{
				this.indexSeederSource.Cancel(this.seedingHandle, reason);
				this.ciSeedingIsCancelled = true;
				ExTraceGlobals.SeederServerTracer.TraceDebug((long)this.GetHashCode(), "IndexSeeder.Cancel() is called");
				CancelCiFileReply cancelCiFileReply = new CancelCiFileReply(this.m_channel);
				cancelCiFileReply.Send();
			});
			ExTraceGlobals.SeederServerTracer.TraceDebug<long>((long)this.GetHashCode(), "CancelSeeding finished call after {0}ms.", arg);
		}

		internal void GetSeedingProgress(string handle)
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "GetSeedingProgress databaseGuid ({0}), handle ({1}).", this.m_databaseGuid, this.seedingHandle);
			long arg = this.PerformCiSeedingAction(delegate
			{
				int num = -1;
				if (RegistryParameters.TestDelayCatalogSeedSec > 0)
				{
					double totalSeconds = (ExDateTime.UtcNow - this.ciSeedStartTimeUtc).TotalSeconds;
					double num2 = (double)RegistryParameters.TestDelayCatalogSeedSec;
					if (totalSeconds < num2)
					{
						num = (int)(totalSeconds * 100.0 / num2);
					}
				}
				if (num < 0 || num >= 100)
				{
					num = this.indexSeederSource.GetProgress(this.seedingHandle);
				}
				ExTraceGlobals.SeederServerTracer.TraceDebug<int>((long)this.GetHashCode(), "IndexSeeder.GetProgress returned {0}", num);
				ProgressCiFileReply progressCiFileReply = new ProgressCiFileReply(this.m_channel, num);
				progressCiFileReply.Send();
			});
			ExTraceGlobals.SeederServerTracer.TraceDebug<long>((long)this.GetHashCode(), "GetSeedingProgress finished call after {0}ms.", arg);
		}

		private long PerformCiSeedingAction(Action action)
		{
			ReplayStopwatch replayStopwatch = new ReplayStopwatch();
			replayStopwatch.Start();
			Exception ex = null;
			try
			{
				action();
			}
			catch (PerformingFastOperationException ex2)
			{
				ex = ex2;
			}
			catch (FileIOonSourceException ex3)
			{
				ex = ex3;
			}
			finally
			{
				replayStopwatch.Stop();
			}
			if (ex != null)
			{
				ReplayCrimsonEvents.CISeedingSourceError.Log<Guid, string, string, string>(this.DatabaseGuid, this.DatabaseName, this.TargetServerName, ex.ToString());
				this.m_channel.SendException(ex);
			}
			return replayStopwatch.ElapsedMilliseconds;
		}

		private ISetPassiveSeeding GetPassiveSeederStatusCallback()
		{
			IReplicaInstanceManager replicaInstanceManager = Dependencies.ReplayCoreManager.ReplicaInstanceManager;
			ISetPassiveSeeding passiveSeederStatusCallback = replicaInstanceManager.GetPassiveSeederStatusCallback(this.DatabaseGuid);
			if (passiveSeederStatusCallback == null)
			{
				this.TraceError("GetPassiveSeederStatusCallback(): The valid RI is not running. The RI might be present after a retry.", new object[0]);
				throw new SeedingSourceReplicaInstanceNotFoundException(this.DatabaseGuid, Environment.MachineName);
			}
			return passiveSeederStatusCallback;
		}

		public void LinkWithNewActiveRIStatus(ISetActiveSeeding riStatus)
		{
			lock (this.m_lock)
			{
				if (this.IsSeeding())
				{
					if (this.m_setPassiveSeedingCallback != null)
					{
						this.m_setPassiveSeedingCallback.EndPassiveSeeding();
						this.m_setPassiveSeedingCallback = null;
					}
					this.m_fPassiveSeeding = false;
					this.m_passiveSeedingSourceContext = PassiveSeedingSourceContextEnum.None;
					this.m_setActiveSeedingCallback = riStatus;
					riStatus.BeginActiveSeeding(this.SeedType);
				}
			}
		}

		public void LinkWithNewPassiveRIStatus(ISetPassiveSeeding riStatus)
		{
			lock (this.m_lock)
			{
				if (this.IsSeeding())
				{
					if (this.m_setActiveSeedingCallback != null)
					{
						this.m_setActiveSeedingCallback.EndActiveSeeding();
						this.m_setActiveSeedingCallback = null;
					}
					this.m_fPassiveSeeding = true;
					this.m_passiveSeedingSourceContext = PassiveSeedingSourceContextEnum.Catalogue;
					this.m_setPassiveSeedingCallback = riStatus;
					riStatus.BeginPassiveSeeding(this.m_passiveSeedingSourceContext, true);
				}
			}
		}

		private ISetActiveSeeding GetActiveSeederStatusCallback()
		{
			if (TestSupport.IsZerobox())
			{
				return null;
			}
			IReplicaInstanceManager replicaInstanceManager = Dependencies.ReplayCoreManager.ReplicaInstanceManager;
			ISetActiveSeeding activeSeederStatusCallback = replicaInstanceManager.GetActiveSeederStatusCallback(this.DatabaseGuid);
			if (activeSeederStatusCallback == null)
			{
				this.TraceError("GetActiveSeederStatusCallback(): The valid RI is not running. The RI might be present after a retry.", new object[0]);
				throw new SeedingSourceReplicaInstanceNotFoundException(this.DatabaseGuid, Environment.MachineName);
			}
			return activeSeederStatusCallback;
		}

		private static readonly Trace Tracer = ExTraceGlobals.SeederServerTracer;

		private NetworkChannel m_channel;

		private IIndexSeederSource indexSeederSource;

		private Guid m_databaseGuid;

		private FileStream m_passiveDatabaseStream;

		private bool m_fSeedingCancelled;

		private Guid? m_targetServerGuid;

		private IADServer m_targetServer;

		private bool m_fPassiveSeeding;

		private PassiveSeedingSourceContextEnum m_passiveSeedingSourceContext;

		private bool m_fClosed;

		private bool m_fSeedingInProgress;

		private ISetPassiveSeeding m_setPassiveSeedingCallback;

		private ISetActiveSeeding m_setActiveSeedingCallback;

		private object m_lock = new object();

		private string seedingHandle;

		private bool ciSeedingIsCancelled;

		private ExDateTime ciSeedStartTimeUtc;
	}
}
