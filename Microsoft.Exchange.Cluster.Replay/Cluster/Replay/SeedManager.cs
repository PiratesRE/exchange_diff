using System;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SeedManager : IServiceComponent
	{
		public SeedManager(IReplicaInstanceManager replicaInstanceManager)
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug((long)this.GetHashCode(), "SeedManager instance is now being constructed.");
			this.m_replicaInstanceManager = replicaInstanceManager;
			this.m_seederInstances = new SeederInstances();
			this.m_cleaner = new SeederInstanceCleaner(this.m_seederInstances);
		}

		internal SeedManager(IReplicaInstanceManager replicaInstanceManager, SeederInstances seederInstances, int maxDurationMs)
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug((long)this.GetHashCode(), "SeedManager instance is now being constructed using the test constructor.");
			this.m_replicaInstanceManager = replicaInstanceManager;
			this.m_seederInstances = seederInstances;
			this.m_cleaner = new SeederInstanceCleaner(this.m_seederInstances, maxDurationMs);
		}

		public string Name
		{
			get
			{
				return "Seed Manager";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.SeedManager;
			}
		}

		public bool IsCritical
		{
			get
			{
				return false;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return !ThirdPartyManager.IsInitialized || !ThirdPartyManager.IsThirdPartyReplicationEnabled;
			}
		}

		public bool IsRetriableOnError
		{
			get
			{
				return true;
			}
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		public void Invoke(Action toInvoke)
		{
			toInvoke();
		}

		public bool Started
		{
			get
			{
				return this.m_fStarted;
			}
		}

		public bool Start()
		{
			bool fStarted;
			lock (this)
			{
				if (!this.m_fStarted)
				{
					EseHelper.GlobalInit();
					this.m_cleaner.Start();
					this.m_fStarted = true;
				}
				else
				{
					ExTraceGlobals.SeederServerTracer.TraceDebug((long)this.GetHashCode(), "SeedManager is already started.");
				}
				fStarted = this.m_fStarted;
			}
			return fStarted;
		}

		public void Stop()
		{
			lock (this)
			{
				if (this.m_fStarted)
				{
					ExTraceGlobals.SeederServerTracer.TraceDebug((long)this.GetHashCode(), "SeedManager is now being stopped.");
					this.m_cleaner.Stop();
					this.m_cleaner = null;
					this.StopInstances();
					if (this.m_serverSeeder != null)
					{
						this.m_serverSeeder.Stop();
						this.m_serverSeeder = null;
					}
					this.m_fStarted = false;
				}
			}
		}

		public void BeginServerLevelSeed(RpcSeederArgs seederArgs)
		{
			lock (this)
			{
				if (!this.m_fStarted)
				{
					ExTraceGlobals.SeederServerTracer.TraceError((long)this.GetHashCode(), "SeedManager.BeginServerLevelSeed(): Skipping seeds because SeedManager is being stopped.");
					ReplayCrimsonEvents.FullServerSeedSkippedShutdown.Log();
					throw new FullServerSeedSkippedShutdownException();
				}
				if (this.m_serverSeeder != null && !this.m_serverSeeder.StopCalled)
				{
					ExTraceGlobals.SeederServerTracer.TraceError((long)this.GetHashCode(), "SeedManager.BeginServerLevelSeed(): Another server-seed is in progress.");
					ReplayCrimsonEvents.FullServerSeedAlreadyInProgress.Log();
					throw new FullServerSeedInProgressException();
				}
				this.m_serverSeeder = new FullServerReseeder(seederArgs);
				this.m_serverSeeder.Start();
			}
		}

		public void PrepareDbSeedAndBegin(RpcSeederArgs seederArgs)
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug((long)this.GetHashCode(), "SeedManager: PrepareDbSeedAndBegin() called.");
			SeederInstanceContainer seederInstanceContainer;
			bool flag = this.m_seederInstances.TryGetInstance(seederArgs.InstanceGuid, out seederInstanceContainer);
			if (flag)
			{
				this.ThrowExceptionForExistingInstance(seederArgs, seederInstanceContainer);
			}
			ExTraceGlobals.SeederServerTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "SeedManager: A SeederInstanceContainer does NOT already exist for DB '{0}' ({1}).", seederArgs.DatabaseName, seederArgs.InstanceGuid);
			Dependencies.ADConfig.Refresh("SeedManager.PrepareDbSeedAndBegin");
			ReplayConfiguration replayConfiguration;
			this.CheckDbValidReplicationTarget(seederArgs, out replayConfiguration);
			ConfigurationArgs configurationArgs = new ConfigurationArgs(replayConfiguration, this.m_replicaInstanceManager);
			seederInstanceContainer = new SeederInstanceContainer(seederArgs, configurationArgs);
			try
			{
				this.m_seederInstances.AddInstance(seederInstanceContainer);
				ReplayEventLogConstants.Tuple_SeedInstancePrepareAdded.LogEvent(null, new object[]
				{
					configurationArgs.Name,
					seederArgs.ToString()
				});
			}
			catch (ArgumentException arg)
			{
				ExTraceGlobals.SeederServerTracer.TraceError<string, ArgumentException>((long)this.GetHashCode(), "SeedManager: SeederInstanceContainer for db '{0}' has already been added. This indicates another PrepareDbSeed() call got to add it just before this one. Ex: {1}", replayConfiguration.Name, arg);
				throw new SeederInstanceAlreadyAddedException(seederInstanceContainer.SeedingSource);
			}
			try
			{
				seederInstanceContainer.PrepareDbSeed();
				ReplayEventLogConstants.Tuple_SeedInstancePrepareSucceeded.LogEvent(null, new object[]
				{
					configurationArgs.Name
				});
			}
			finally
			{
				SeederState seedState = seederInstanceContainer.SeedState;
				if (seedState != SeederState.SeedPrepared)
				{
					this.m_seederInstances.RemoveInstance(seederInstanceContainer);
					ExTraceGlobals.SeederServerTracer.TraceDebug<string, SeederState>((long)this.GetHashCode(), "SeedManager: SeederInstanceContainer for db '{0}' is being removed from table because PrepareDbSeed() did not pass (state ={1}, expected was SeedPrepared).", replayConfiguration.Name, seedState);
					ReplayEventLogConstants.Tuple_SeedInstancePrepareUnknownError.LogEvent(null, new object[]
					{
						configurationArgs.Name
					});
				}
			}
			ExTraceGlobals.SeederServerTracer.TraceDebug<string>((long)this.GetHashCode(), "SeedManager: SeederInstanceContainer for db '{0} is being queued for seeding since PrepareDbSeed() passed.", replayConfiguration.Name);
			try
			{
				seederInstanceContainer.BeginDbSeed();
				ReplayEventLogConstants.Tuple_SeedInstanceBeginSucceeded.LogEvent(null, new object[]
				{
					configurationArgs.Name
				});
			}
			finally
			{
				SeederState seedState2 = seederInstanceContainer.SeedState;
				if (seedState2 != SeederState.SeedInProgress && seedState2 != SeederState.SeedSuccessful)
				{
					this.m_seederInstances.RemoveInstance(seederInstanceContainer);
					ExTraceGlobals.SeederServerTracer.TraceDebug<string, SeederState>((long)this.GetHashCode(), "SeedManager: SeederInstanceContainer for db '{0}' is being removed from table because BeginDbSeed() did not pass (state = {1}).", replayConfiguration.Name, seedState2);
					ReplayEventLogConstants.Tuple_SeedInstanceBeginUnknownError.LogEvent(null, new object[]
					{
						configurationArgs.Name
					});
				}
			}
		}

		public RpcSeederStatus GetDbSeedStatus(Guid dbGuid)
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedManager: GetDbSeedStatus() called for DB ({0}).", dbGuid);
			SeederInstanceContainer seederInstanceContainer;
			if (this.m_seederInstances.TryGetInstance(SafeInstanceTable<SeederInstanceContainer>.GetIdentityFromGuid(dbGuid), out seederInstanceContainer))
			{
				ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedManager: Found instance for DB ({0}).", dbGuid);
				return seederInstanceContainer.GetSeedStatus();
			}
			ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedManager: A SeederInstanceContainer does NOT already exist for DB ({0}).", dbGuid);
			throw new SeederInstanceNotFoundException(dbGuid.ToString());
		}

		public void CancelDbSeed(Guid dbGuid, bool fAdminRequested)
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedManager: CancelDbSeed() called for DB ({0}).", dbGuid);
			SeederInstanceContainer seederInstanceContainer;
			if (this.m_seederInstances.TryGetInstance(SafeInstanceTable<SeederInstanceContainer>.GetIdentityFromGuid(dbGuid), out seederInstanceContainer))
			{
				ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedManager: Found instance for DB ({0}).", dbGuid);
				seederInstanceContainer.CancelDbSeed();
				if (fAdminRequested)
				{
					ReplayEventLogConstants.Tuple_SeedInstanceCancelRequestedByAdmin.LogEvent(null, new object[]
					{
						seederInstanceContainer.Name
					});
				}
				ReplayCrimsonEvents.SeedingCancelled.Log<Guid, string, bool>(dbGuid, seederInstanceContainer.Name, fAdminRequested);
				return;
			}
			ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedManager: A SeederInstanceContainer does NOT already exist for DB ({0}).", dbGuid);
			throw new SeederInstanceNotFoundException(dbGuid.ToString());
		}

		public void EndDbSeed(Guid dbGuid)
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedManager: EndDbSeed() called for DB ({0}).", dbGuid);
			SeederInstanceContainer seederInstanceContainer;
			if (!this.m_seederInstances.TryGetInstance(SafeInstanceTable<SeederInstanceContainer>.GetIdentityFromGuid(dbGuid), out seederInstanceContainer))
			{
				ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedManager: Did NOT find instance for DB ({0})!", dbGuid);
				throw new SeederInstanceNotFoundException(dbGuid.ToString());
			}
			SeederState seedState = seederInstanceContainer.SeedState;
			ExTraceGlobals.SeederServerTracer.TraceDebug<Guid, SeederState>((long)this.GetHashCode(), "SeedManager: Found instance for DB ({0}) in state '{1}'.", dbGuid, seedState);
			if (seedState == SeederState.SeedSuccessful || seedState == SeederState.SeedCancelled || seedState == SeederState.SeedFailed)
			{
				this.m_seederInstances.RemoveInstance(seederInstanceContainer);
				ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedManager: Removed seeder instance for DB ({0}) from table.", dbGuid);
				ReplayEventLogConstants.Tuple_SeedInstanceCleanupRequestedByAdmin.LogEvent(null, new object[]
				{
					seederInstanceContainer.Name
				});
				return;
			}
			throw new SeederInstanceInvalidStateForEndException(dbGuid.ToString());
		}

		private void CheckDbValidReplicationTarget(RpcSeederArgs seederArgs, out ReplayConfiguration replayConfig)
		{
			replayConfig = null;
			ADReplicationRetryTimer adreplicationRetryTimer = new ADReplicationRetryTimer();
			bool flag = !seederArgs.SeedDatabase && seederArgs.SeedCiFiles;
			try
			{
				bool flag2;
				while (!this.IsDBCurrentReplicaInstance(seederArgs.InstanceGuid, out replayConfig, out flag2))
				{
					if (SeedHelper.IsDbPendingLcrRcrTarget(seederArgs.InstanceGuid, out replayConfig, out flag2))
					{
						if (flag2)
						{
							if (flag)
							{
								ExTraceGlobals.SeederServerTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "SeedManager: Database '{0}' ({1}) is not a valid RCR replica target but the requested seeding is CI only.", replayConfig.Name, seederArgs.InstanceGuid);
								return;
							}
							this.HandleDbCopyNotTarget(seederArgs, replayConfig);
						}
						ExTraceGlobals.SeederServerTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "SeedManager: Database '{0}' ({1}) is a valid RCR replica target.", replayConfig.Name, seederArgs.InstanceGuid);
						return;
					}
					ExTraceGlobals.SeederServerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "SeedManager: Database '{0}' is NOT a valid RCR replica target!", seederArgs.InstanceGuid);
					if (adreplicationRetryTimer.IsExpired)
					{
						throw new InvalidDbForSeedSpecifiedException();
					}
					adreplicationRetryTimer.Sleep();
				}
				if (flag2)
				{
					if (flag)
					{
						ExTraceGlobals.SeederServerTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "SeedManager: Database '{0}' ({1}) is not running as a valid RCR replica target but the requested seeding is CI only.", replayConfig.Name, seederArgs.InstanceGuid);
						return;
					}
					this.HandleDbCopyNotTarget(seederArgs, replayConfig);
				}
				ExTraceGlobals.SeederServerTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "SeedManager: Database '{0}' ({1}) is currently running as a valid RCR replica target.", replayConfig.Name, seederArgs.InstanceGuid);
			}
			catch (DataSourceOperationException ex)
			{
				ExTraceGlobals.SeederServerTracer.TraceError<DataSourceOperationException>((long)this.GetHashCode(), "SeedManager: CheckDBValidReplicationTarget: Exception encountered: {0}", ex);
				throw new SeedPrepareException(ex.ToString(), ex);
			}
			catch (DataValidationException ex2)
			{
				ExTraceGlobals.SeederServerTracer.TraceError<DataValidationException>((long)this.GetHashCode(), "SeedManager: CheckDBValidReplicationTarget: Exception encountered: {0}", ex2);
				throw new SeedPrepareException(ex2.ToString(), ex2);
			}
			catch (ObjectNotFoundException ex3)
			{
				ExTraceGlobals.SeederServerTracer.TraceError<ObjectNotFoundException>((long)this.GetHashCode(), "SeedManager: CheckDBValidReplicationTarget: Exception encountered: {0}", ex3);
				throw new SeedPrepareException(ex3.ToString(), ex3);
			}
			catch (StoragePermanentException ex4)
			{
				ExTraceGlobals.SeederServerTracer.TraceError<StoragePermanentException>((long)this.GetHashCode(), "SeedManager: CheckDBValidReplicationTarget: Exception encountered: {0}", ex4);
				throw new SeedPrepareException(ex4.ToString(), ex4);
			}
			catch (TransientException ex5)
			{
				ExTraceGlobals.SeederServerTracer.TraceError<TransientException>((long)this.GetHashCode(), "SeedManager: CheckDBValidReplicationTarget: Exception encountered: {0}", ex5);
				throw new SeedPrepareException(ex5.ToString(), ex5);
			}
		}

		private void HandleDbCopyNotTarget(RpcSeederArgs seederArgs, ReplayConfiguration replayConfig)
		{
			ExTraceGlobals.SeederServerTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "SeedManager: Database '{0}' ({1}) is NOT a replication target on the local machine. It is a source!", replayConfig.Name, seederArgs.InstanceGuid);
			DbCopyNotTargetException ex = new DbCopyNotTargetException(replayConfig.DatabaseName, Environment.MachineName);
			throw ex;
		}

		private bool IsDBCurrentReplicaInstance(Guid guid, out ReplayConfiguration replayConfig, out bool fSource)
		{
			replayConfig = null;
			fSource = false;
			ReplicaInstance replicaInstance;
			if (this.m_replicaInstanceManager.TryGetReplicaInstance(guid, out replicaInstance))
			{
				replayConfig = replicaInstance.Configuration;
				fSource = (replayConfig.Type == ReplayConfigType.RemoteCopySource);
				return true;
			}
			return false;
		}

		private void ThrowExceptionForExistingInstance(RpcSeederArgs seederArgs, SeederInstanceContainer seederInstance)
		{
			SeederState seedState = seederInstance.SeedState;
			ExTraceGlobals.SeederServerTracer.TraceError<string, Guid, SeederState>((long)this.GetHashCode(), "SeedManager: A SeederInstanceContainer already exists for DB '{0}' ({1}) and is in SeederState '{2}'.", seederArgs.DatabaseName, seederArgs.InstanceGuid, seedState);
			if (seedState == SeederState.Unknown)
			{
				throw new SeederInstanceAlreadyAddedException(seederInstance.SeedingSource);
			}
			if (seedState == SeederState.SeedPrepared)
			{
				throw new SeederInstanceAlreadyAddedException(seederInstance.SeedingSource);
			}
			if (seedState == SeederState.SeedInProgress)
			{
				throw new SeederInstanceAlreadyInProgressException(seederInstance.SeedingSource);
			}
			if (seedState == SeederState.SeedSuccessful)
			{
				throw new SeederInstanceAlreadyCompletedException(seederInstance.SeedingSource);
			}
			if (seedState == SeederState.SeedCancelled)
			{
				throw new SeederInstanceAlreadyCancelledException(seederInstance.SeedingSource);
			}
			if (seedState == SeederState.SeedFailed)
			{
				throw new SeederInstanceAlreadyFailedException(seederInstance.GetSeedStatus(), seederInstance.SeedingSource);
			}
		}

		private void StopInstances()
		{
			SeederInstanceContainer[] allInstances = this.m_seederInstances.GetAllInstances();
			foreach (SeederInstanceContainer seederInstanceContainer in allInstances)
			{
				seederInstanceContainer.CancelDbSeed();
				ExTraceGlobals.SeederServerTracer.TraceDebug<string>((long)this.GetHashCode(), "SeedManager: StopInstances: CancelDbSeed() issued for DB ({0}) as part of shutdown.", seederInstanceContainer.Identity);
			}
			foreach (SeederInstanceContainer seederInstanceContainer2 in allInstances)
			{
				this.m_seederInstances.RemoveInstance(seederInstanceContainer2);
				ExTraceGlobals.SeederServerTracer.TraceDebug<string>((long)this.GetHashCode(), "SeedManager: StopInstances: Seeder instance for DB ({0}) removed as part of shutdown.", seederInstanceContainer2.Identity);
			}
			if (allInstances.Length > 0)
			{
				ReplayEventLogConstants.Tuple_SeedInstancesStoppedServiceShutdown.LogEvent(null, new object[0]);
			}
		}

		private IReplicaInstanceManager m_replicaInstanceManager;

		private SeederInstances m_seederInstances;

		private SeederInstanceCleaner m_cleaner;

		private FullServerReseeder m_serverSeeder;

		private bool m_fStarted;
	}
}
