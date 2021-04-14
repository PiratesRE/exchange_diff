using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Cluster.Replay.MountPoint;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AutoReseedManager : TimerComponent, IServiceComponent
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AutoReseedTracer;
			}
		}

		public AutoReseedManager(IMonitoringADConfigProvider adConfigProvider, ICopyStatusClientLookup statusLookup, IReplicaInstanceManager replicaInstanceManager) : base(TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedManagerPollerIntervalInSecs), TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedManagerPollerIntervalInSecs), "AutoReseedManager")
		{
			AutoReseedManager.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseedManager instance is now being constructed.");
			this.m_adConfigProvider = adConfigProvider;
			this.m_statusLookup = statusLookup;
			this.m_replicaInstanceManager = replicaInstanceManager;
			this.m_launcher = new AutoReseedWorkflowLauncher();
			this.m_volumeManager = new VolumeManager();
		}

		public string Name
		{
			get
			{
				return "Automatic Reseed Manager";
			}
		}

		public FacilityEnum Facility
		{
			get
			{
				return FacilityEnum.AutoReseedManager;
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
				return !RegistryParameters.AutoReseedManagerDisabled;
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

		public new bool Start()
		{
			base.Start();
			return true;
		}

		protected override void TimerCallbackInternal()
		{
			try
			{
				this.Run();
			}
			catch (MonitoringADServiceShuttingDownException arg)
			{
				AutoReseedManager.Tracer.TraceError<MonitoringADServiceShuttingDownException>((long)this.GetHashCode(), "AutoReseedManager: Got service shutting down exception when retrieving AD config: {0}", arg);
			}
			catch (MonitoringADConfigException ex)
			{
				AutoReseedManager.Tracer.TraceError<MonitoringADConfigException>((long)this.GetHashCode(), "AutoReseedManager: Got exception when retrieving AD config: {0}", ex);
				ReplayCrimsonEvents.AutoReseedManagerError.LogPeriodic<string, MonitoringADConfigException>(this.GetHashCode(), DiagCore.DefaultEventSuppressionInterval, ex.Message, ex);
			}
		}

		private void Run()
		{
			AutoReseedManager.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseedManager: Entering Run()...");
			this.ResetState();
			IMonitoringADConfig config = this.m_adConfigProvider.GetConfig(true);
			if (MountPointUtil.IsConfigureMountPointsEnabled())
			{
				AutoReseedManager.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseedManager: Skipping running AutoReseedManager because regkey '{0}' is enabled.");
				return;
			}
			if (config.ServerRole == MonitoringServerRole.Standalone)
			{
				AutoReseedManager.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseedManager: Skipping running AutoReseedManager because local server is not a member of a DAG.");
				return;
			}
			if (!config.Dag.AutoDagAutoReseedEnabled)
			{
				AutoReseedManager.Tracer.TraceDebug<string>((long)this.GetHashCode(), "AutoReseedManager: Skipping running AutoReseedManager because the local DAG '{0}' has AutoDagAutoReseedEnabled set to disabled.", config.Dag.Name);
				return;
			}
			if (DiskReclaimerManager.ShouldSkipDueToServerNotConfigured(config.TargetMiniServer))
			{
				AutoReseedManager.Tracer.TraceDebug<AmServerName>((long)this.GetHashCode(), "AutoReseedManager: Skipping running AutoReseedManager because the local server '{0}' is not yet fully configured in the AD.", config.TargetServerName);
				return;
			}
			this.m_volumeManager.Refresh(config);
			IEnumerable<IADDatabase> enumerable = config.DatabaseMap[config.TargetServerName];
			foreach (IADDatabase db in enumerable)
			{
				this.ProcessDatabase(db, enumerable, config);
			}
		}

		private void ResetState()
		{
			this.m_serverStatusResults = null;
			this.m_dagStatusResults = null;
			this.m_seedLimiter = null;
		}

		private void ProcessDatabase(IADDatabase db, IEnumerable<IADDatabase> databases, IMonitoringADConfig adConfig)
		{
			Exception ex = null;
			try
			{
				this.ProcessDatabaseInternal(db, databases, adConfig);
			}
			catch (AutoReseedException ex2)
			{
				ex = ex2;
			}
			catch (TaskServerException ex3)
			{
				ex = ex3;
			}
			catch (TaskServerTransientException ex4)
			{
				ex = ex4;
			}
			catch (AmServerException ex5)
			{
				ex = ex5;
			}
			catch (AmServerTransientException ex6)
			{
				ex = ex6;
			}
			catch (ADOperationException ex7)
			{
				ex = ex7;
			}
			catch (ADTransientException ex8)
			{
				ex = ex8;
			}
			if (ex != null)
			{
				AutoReseedManager.Tracer.TraceError<string>((long)this.GetHashCode(), "AutoReseedManager.ProcessDatabase: Got exception when processing database '{0}'. Skipping this database.", db.Name);
				string text = ex.Message;
				if (ex is AutoReseedException)
				{
					text = ((AutoReseedException)ex).ErrorMsg;
				}
				ReplayCrimsonEvents.AutoReseedManagerDatabaseError.LogPeriodic<string, Guid, string, Exception>(db.Guid, DiagCore.DefaultEventSuppressionInterval, db.Name, db.Guid, text, ex);
			}
		}

		private void ProcessDatabaseInternal(IADDatabase db, IEnumerable<IADDatabase> databases, IMonitoringADConfig adConfig)
		{
			AutoReseedManager.Tracer.TraceDebug<string>((long)this.GetHashCode(), "AutoReseedManager.ProcessDatabase: Processing Database '{0}'...", db.Name);
			if (db.ReplicationType != ReplicationType.Remote)
			{
				AutoReseedManager.Tracer.TraceDebug<string>((long)this.GetHashCode(), "AutoReseedManager.ProcessDatabase: Database '{0}' is not a replicated database.", db.Name);
			}
			IEnumerable<CopyStatusClientCachedEntry> enumerable = this.LookupCachedCopyStatuses(db);
			IEnumerable<IADDatabase> databases2 = adConfig.DatabaseMap[adConfig.TargetServerName];
			CopyStatusClientCachedEntry targetCopyStatus = enumerable.FirstOrDefault((CopyStatusClientCachedEntry status) => status.ServerContacted.Equals(adConfig.TargetServerName));
			this.LookupCachedCopyStatusesAcrossDag(adConfig);
			AutoReseedContext context = new AutoReseedContext(this.m_volumeManager, this.m_replicaInstanceManager, adConfig.Dag, db, databases2, adConfig.TargetServerName, adConfig.TargetMiniServer, targetCopyStatus, enumerable, this.m_serverStatusResults, this.m_dagStatusResults, this.m_seedLimiter, adConfig);
			this.m_launcher.BeginAutoReseedIfNecessary(context);
		}

		private void LookupCachedCopyStatusesAcrossDag(IMonitoringADConfig adConfig)
		{
			if (this.m_dagStatusResults == null)
			{
				this.m_dagStatusResults = new Dictionary<AmServerName, IEnumerable<CopyStatusClientCachedEntry>>(16);
			}
			foreach (KeyValuePair<AmServerName, IEnumerable<IADDatabase>> keyValuePair in adConfig.DatabaseMap)
			{
				AmServerName key = keyValuePair.Key;
				if (!this.m_dagStatusResults.ContainsKey(key))
				{
					AutoReseedManager.Tracer.TraceDebug<AmServerName>((long)this.GetHashCode(), "AutoReseedManager: Retrieving cached copy status results for all copies on server '{0}'.", key);
					IEnumerable<CopyStatusClientCachedEntry> copyStatusesByServer = this.m_statusLookup.GetCopyStatusesByServer(key, keyValuePair.Value, CopyStatusClientLookupFlags.None);
					this.m_dagStatusResults.Add(key, copyStatusesByServer);
				}
			}
			if (this.m_serverStatusResults == null)
			{
				this.m_dagStatusResults.TryGetValue(adConfig.TargetServerName, out this.m_serverStatusResults);
				this.m_seedLimiter = new AutoReseedServerLimiter(this.m_serverStatusResults);
			}
		}

		private IEnumerable<CopyStatusClientCachedEntry> LookupCachedCopyStatuses(IADDatabase database)
		{
			IADDatabaseCopy[] databaseCopies = database.DatabaseCopies;
			if (databaseCopies == null || databaseCopies.Length == 0)
			{
				throw new AutoReseedNoCopiesException(database.Name);
			}
			ICollection<AmServerName> servers = from dbCopy in databaseCopies
			select new AmServerName(dbCopy.HostServerName, false);
			return this.m_statusLookup.GetCopyStatusesByDatabase(database.Guid, servers, CopyStatusClientLookupFlags.None);
		}

		private readonly IMonitoringADConfigProvider m_adConfigProvider;

		private readonly ICopyStatusClientLookup m_statusLookup;

		private readonly AutoReseedWorkflowLauncher m_launcher;

		private readonly VolumeManager m_volumeManager;

		private readonly IReplicaInstanceManager m_replicaInstanceManager;

		private IEnumerable<CopyStatusClientCachedEntry> m_serverStatusResults;

		private Dictionary<AmServerName, IEnumerable<CopyStatusClientCachedEntry>> m_dagStatusResults;

		private AutoReseedServerLimiter m_seedLimiter;
	}
}
