using System;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.RpcEndpoint;
using Microsoft.Exchange.Search.Fast;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CiFilesSeederInstance : SeederInstanceBase, IReplicaSeederCallback
	{
		public CiFilesSeederInstance(RpcSeederArgs rpcArgs, ConfigurationArgs configArgs) : base(rpcArgs, configArgs)
		{
			ITopologyConfigurationSession adSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 73, ".ctor", "f:\\15.00.1497\\sources\\dev\\cluster\\src\\Replay\\seeder\\cifileseederinstance.cs");
			this.targetServer = CiFilesSeederInstance.GetLocalServer(adSession);
			if (!string.IsNullOrEmpty(rpcArgs.SourceMachineName) && !SharedHelper.StringIEquals(rpcArgs.SourceMachineName, configArgs.SourceMachine))
			{
				this.m_fPassiveSeeding = true;
			}
			Server server = this.m_fPassiveSeeding ? CiFilesSeederInstance.GetServerByName(adSession, rpcArgs.SourceMachineName) : CiFilesSeederInstance.GetServerByName(adSession, configArgs.SourceMachine);
			string indexSystemName = FastIndexVersion.GetIndexSystemName(this.ConfigArgs.IdentityGuid);
			this.targetIndexSeeder = new IndexSeeder(indexSystemName);
			this.sourceSeederProvider = new CiFileSeederProvider(server.Fqdn, this.targetServer.Fqdn, this.ConfigArgs.IdentityGuid);
			this.sourceSeederProvider.NetworkName = this.SeederArgs.NetworkId;
			this.sourceSeederProvider.CompressOverride = this.SeederArgs.CompressOverride;
			this.sourceSeederProvider.EncryptOverride = this.SeederArgs.EncryptOverride;
			base.ReadSeedTestHook();
			ExTraceGlobals.SeederServerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "CiFilesSeederInstance constructed with the following arguments: {0}; {1}", this.SeederArgs.ToString(), this.ConfigArgs.ToString());
		}

		public override string Identity
		{
			get
			{
				return SafeInstanceTable<CiFilesSeederInstance>.GetIdentityFromGuid(this.SeederArgs.InstanceGuid);
			}
		}

		private int PerfmonSeedingPercent
		{
			set
			{
				if (value < 0 || value > 100)
				{
					throw new ArgumentOutOfRangeException("percentage must between 0 and 100");
				}
				if (base.SeederPerfmonInstance != null)
				{
					base.SeederPerfmonInstance.CiSeedingPercent.RawValue = (long)value;
				}
			}
		}

		private bool PerfmonSeedingInProgress
		{
			set
			{
				if (base.SeederPerfmonInstance != null)
				{
					base.SeederPerfmonInstance.CiSeedingInProgress.RawValue = (value ? 1L : 0L);
				}
			}
		}

		public static Server GetServerByName(string serverName)
		{
			ITopologyConfigurationSession adSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 168, "GetServerByName", "f:\\15.00.1497\\sources\\dev\\cluster\\src\\Replay\\seeder\\cifileseederinstance.cs");
			return CiFilesSeederInstance.GetServerByName(adSession, serverName);
		}

		public void PrepareCiFileSeeding()
		{
			try
			{
				SeederState seederState;
				SeederState seederState2;
				if (!this.UpdateState(SeederState.SeedPrepared, out seederState, out seederState2))
				{
					base.LogError(new SeederOperationAbortedException());
				}
			}
			catch (TaskServerTransientException ex)
			{
				base.LogError(ex);
			}
			catch (TaskServerException ex2)
			{
				base.LogError(ex2);
			}
			base.CheckOperationCancelled();
		}

		public void CancelCiFileSeed()
		{
			lock (this.locker)
			{
				if (!this.m_fcancelled)
				{
					SeederState seederState;
					SeederState seederState2;
					if (this.UpdateState(SeederState.SeedCancelled, out seederState, out seederState2) && seederState2 == SeederState.SeedInProgress)
					{
						this.PerformSeedingAction(delegate(CiFileSeederProvider provider)
						{
							provider.CancelSeeding();
						});
					}
					this.m_fcancelled = true;
				}
			}
		}

		public void ReportProgress(string edbName, long edbSize, long bytesRead, long bytesWritten)
		{
			lock (this.locker)
			{
				this.m_seederStatus.FileFullPath = edbName;
				this.m_seederStatus.BytesTotal = edbSize;
				this.m_seederStatus.BytesRead = bytesRead;
				this.m_seederStatus.BytesWritten = bytesWritten;
				ExTraceGlobals.SeederServerTracer.TraceDebug<int>((long)this.GetHashCode(), "CiFilesSeederInstance.UpdateProgress: Progress percentage = {0}%", this.m_seederStatus.PercentComplete);
				this.PerfmonSeedingPercent = this.m_seederStatus.PercentComplete;
			}
			this.TestHook();
		}

		public bool IsBackupCancelled()
		{
			return this.m_fcancelled;
		}

		protected override void ResetPerfmonSeedingProgress()
		{
			this.PerfmonSeedingPercent = 0;
			this.PerfmonSeedingInProgress = false;
		}

		protected override void CloseSeeding(bool wasSeedSuccessful)
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "CiFilesSeederInstance.CloseSeeding( {0} ) called for {1} ({2})", wasSeedSuccessful.ToString(), this.ConfigArgs.Name, this.Identity);
			if (wasSeedSuccessful)
			{
				lock (this.locker)
				{
					SeederState seederState;
					SeederState seederState2;
					this.UpdateState(SeederState.SeedSuccessful, out seederState, out seederState2);
					this.m_seederStatus.ErrorInfo = new RpcErrorExceptionInfo();
					this.m_lastErrorMessage = null;
					ReplayEventLogConstants.Tuple_CiSeedInstanceSuccess.LogEvent(null, new object[]
					{
						this.ConfigArgs.Name
					});
				}
				this.IncreasePerfmonSeedingSuccesses();
			}
			else
			{
				this.IncreasePerfmonSeedingFailures();
			}
			this.PerfmonSeedingInProgress = false;
			this.Cleanup();
		}

		protected override void Cleanup()
		{
			if (this.targetIndexSeeder != null)
			{
				this.targetIndexSeeder.Dispose();
				this.targetIndexSeeder = null;
			}
			if (this.sourceSeederProvider != null)
			{
				this.sourceSeederProvider.Close();
				this.sourceSeederProvider = null;
			}
		}

		protected override void SeedThreadProcInternal()
		{
			base.CheckOperationCancelled();
			this.PerfmonSeedingInProgress = true;
			ReplayCrimsonEvents.CISeedingBegins.Log<Guid, string, string>(base.DatabaseGuid, base.DatabaseName, this.SeederArgs.Flags.ToString());
			Exception ex = null;
			try
			{
				string targetEndPoint = this.GetSeedingEndPoint();
				int length = targetEndPoint.IndexOf('/');
				string oldValue = targetEndPoint.Substring(0, length);
				targetEndPoint = targetEndPoint.Replace(oldValue, this.targetServer.Fqdn);
				ExTraceGlobals.SeederServerTracer.TraceDebug<string>((long)this.GetHashCode(), "CiFilesSeederInstance: GetSeedingEndPoint returned {0}", targetEndPoint);
				base.CheckOperationCancelled();
				this.PerformSeedingAction(delegate(CiFileSeederProvider provider)
				{
					provider.SeedCatalog(targetEndPoint, this, this.SeederArgs.Flags.ToString());
				});
				ReplayCrimsonEvents.CISeedingTryResumeIndexing.Log<Guid, string, string>(base.DatabaseGuid, base.DatabaseName, string.Empty);
				this.TryResumeIndexing();
				this.CloseSeeding(true);
			}
			catch (SeederServerException ex2)
			{
				ex = ex2;
				throw;
			}
			finally
			{
				string text = "Success";
				if (ex is SeederOperationAbortedException)
				{
					text = "Aborted";
				}
				else if (ex != null)
				{
					text = "Failed";
				}
				ReplayCrimsonEvents.CISeedingCompleted.Log<Guid, string, string, string, string>(base.DatabaseGuid, base.DatabaseName, this.SeederArgs.Flags.ToString(), text, (ex == null) ? string.Empty : ex.ToString());
			}
		}

		protected override void CallFailedDbSeed(ExEventLog.EventTuple tuple, Exception ex)
		{
		}

		private static Server GetLocalServer(ITopologyConfigurationSession adSession)
		{
			return adSession.FindServerByName(Environment.MachineName);
		}

		private static Server GetServerByName(ITopologyConfigurationSession adSession, string serverName)
		{
			serverName = SharedHelper.GetNodeNameFromFqdn(serverName);
			return adSession.FindServerByName(serverName);
		}

		private void IncreasePerfmonSeedingSuccesses()
		{
			if (base.SeederPerfmonInstance != null)
			{
				base.SeederPerfmonInstance.CiSeedingSuccesses.Increment();
			}
		}

		private void IncreasePerfmonSeedingFailures()
		{
			if (base.SeederPerfmonInstance != null)
			{
				base.SeederPerfmonInstance.CiSeedingFailures.Increment();
			}
		}

		private string GetSeedingEndPoint()
		{
			PerformingFastOperationException ex = null;
			ExDateTime utcNow = ExDateTime.UtcNow;
			TimeSpan t = TimeSpan.FromSeconds((double)RegistryParameters.WaitForCatalogReadyTimeoutInSec);
			TimeSpan timeout = TimeSpan.FromSeconds((double)RegistryParameters.CheckCatalogReadyIntervalInSec);
			string seedingEndPoint;
			for (;;)
			{
				try
				{
					seedingEndPoint = this.targetIndexSeeder.GetSeedingEndPoint();
					break;
				}
				catch (PerformingFastOperationException ex2)
				{
					ExTraceGlobals.SeederServerTracer.TraceError<string, PerformingFastOperationException>((long)this.GetHashCode(), "CiFilesSeederInstance: Failed to get seeding endpoint for database ({0}): {1}", this.ConfigArgs.Name, ex2);
					ex = ex2;
				}
				ExDateTime utcNow2 = ExDateTime.UtcNow;
				if (utcNow2 - utcNow >= t)
				{
					base.LogError(ex);
				}
				base.CheckOperationCancelled();
				Thread.Sleep(timeout);
			}
			return seedingEndPoint;
		}

		private void PerformSeedingAction(Action<CiFileSeederProvider> action)
		{
			try
			{
				action(this.sourceSeederProvider);
			}
			catch (PerformingFastOperationException ex)
			{
				base.LogError(ex);
			}
			catch (NetworkRemoteException innerException)
			{
				base.LogError(new PerformingFastOperationException(innerException));
			}
			catch (NetworkTransportException innerException2)
			{
				base.LogError(new PerformingFastOperationException(innerException2));
			}
		}

		private void TryResumeIndexing()
		{
			if (RegistryParameters.IgnoreCatalogHealthSetByCI)
			{
				return;
			}
			ExTraceGlobals.SeederServerTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "CiFilesSeederInstance: Notify search service to resume indexing for database {0} ({1})", this.ConfigArgs.Name, this.ConfigArgs.IdentityGuid);
			SearchServiceRpcClient searchServiceRpcClient = null;
			bool discard = false;
			try
			{
				searchServiceRpcClient = RpcConnectionPool.GetSearchRpcClient();
				searchServiceRpcClient.ResumeIndexing(this.ConfigArgs.IdentityGuid);
			}
			catch (RpcException arg)
			{
				ExTraceGlobals.SeederServerTracer.TraceError<RpcException>((long)this.GetHashCode(), "CiFilesSeederInstance: ResumeIndexing threw: {0}", arg);
				discard = true;
			}
			finally
			{
				if (searchServiceRpcClient != null)
				{
					RpcConnectionPool.ReturnSearchRpcClientToCache(ref searchServiceRpcClient, discard);
				}
			}
		}

		private void TestHook()
		{
			if (this.m_testHookSeedDelayPerCallback > 0)
			{
				Thread.Sleep(this.m_testHookSeedDelayPerCallback);
			}
		}

		private readonly Server targetServer;

		private CiFileSeederProvider sourceSeederProvider;

		private IIndexSeederTarget targetIndexSeeder;

		private object locker = new object();
	}
}
