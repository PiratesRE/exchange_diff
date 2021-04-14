using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseHealthMonitor : TimerComponent
	{
		private static Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoringTracer;
			}
		}

		public DatabaseHealthMonitor(IMonitoringADConfigProvider adConfigProvider, ICopyStatusClientLookup statusLookup) : base(TimeSpan.FromMilliseconds((double)RegistryParameters.DatabaseHealthMonitorPeriodicIntervalInMsec), TimeSpan.FromMilliseconds((double)RegistryParameters.DatabaseHealthMonitorPeriodicIntervalInMsec), "DatabaseHealthMonitor")
		{
			this.m_adConfigProvider = adConfigProvider;
			this.m_statusLookup = statusLookup;
			this.m_dbAlerts = new DatabaseLevelAlerts();
			this.m_propertyUpdateTracker = new PropertyUpdateTracker();
			this.stopWatch = new Stopwatch();
		}

		protected override void TimerCallbackInternal()
		{
			try
			{
				this.Run();
			}
			catch (MonitoringADServiceShuttingDownException arg)
			{
				DatabaseHealthMonitor.Tracer.TraceError<MonitoringADServiceShuttingDownException>((long)this.GetHashCode(), "DatabaseHealthMonitor: Got service shutting down exception when retrieving AD config: {0}", arg);
			}
			catch (MonitoringADConfigException ex)
			{
				DatabaseHealthMonitor.Tracer.TraceError<MonitoringADConfigException>((long)this.GetHashCode(), "DatabaseHealthMonitor: Got exception when retrieving AD config: {0}", ex);
				ReplayCrimsonEvents.DatabaseHealthMonitorError.LogPeriodic<string, MonitoringADConfigException>(this.GetHashCode(), DiagCore.DefaultEventSuppressionInterval, ex.Message, ex);
			}
		}

		private void Run()
		{
			if (!this.stopWatch.IsRunning)
			{
				this.stopWatch.Start();
			}
			IMonitoringADConfig configIgnoringStaleness = this.m_adConfigProvider.GetConfigIgnoringStaleness(true);
			if (configIgnoringStaleness.ServerRole == MonitoringServerRole.Standalone)
			{
				DatabaseHealthMonitor.Tracer.TraceDebug((long)this.GetHashCode(), "DatabaseHealthMonitor: Skipping running health checks because local server is not a member of a DAG.");
				return;
			}
			if (configIgnoringStaleness.Dag.ThirdPartyReplication == ThirdPartyReplicationMode.Enabled)
			{
				DatabaseHealthMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "DatabaseHealthMonitor: Dag '{0}' is in ThirdPartyReplication mode. Shutting down the DatabaseHealthMonitor component.", configIgnoringStaleness.Dag.Name);
				base.ChangeTimer(InvokeWithTimeout.InfiniteTimeSpan, InvokeWithTimeout.InfiniteTimeSpan);
				ThreadPool.QueueUserWorkItem(delegate(object stateNotUsed)
				{
					base.Stop();
				});
				return;
			}
			IEnumerable<IADDatabase> enumerable = configIgnoringStaleness.DatabasesIncludingMisconfiguredMap[configIgnoringStaleness.TargetServerName];
			HashSet<Guid> hashSet = new HashSet<Guid>();
			foreach (IADDatabase iaddatabase in enumerable)
			{
				hashSet.Add(iaddatabase.Guid);
				this.ProcessDatabase(iaddatabase, configIgnoringStaleness);
			}
			this.m_dbAlerts.Cleanup(hashSet);
			this.RaiseServerLevelAlertsIfNecessary(configIgnoringStaleness);
		}

		private void RaiseServerLevelAlertsIfNecessary(IMonitoringADConfig adConfig)
		{
			ServerValidationResult serverValidationResult = new ServerValidationResult(adConfig.TargetMiniServer.Name, adConfig.TargetMiniServer.Guid);
			if (this.m_serverRedundancyAlert == null)
			{
				this.m_serverRedundancyAlert = new ServerLevelDatabaseRedundancyAlert(serverValidationResult.Identity, serverValidationResult.IdentityGuid, this.m_dbAlerts.OneRedundantCopy);
			}
			this.m_serverRedundancyAlert.RaiseAppropriateAlertIfNecessary(serverValidationResult);
			serverValidationResult = new ServerValidationResult(adConfig.TargetMiniServer.Name, adConfig.TargetMiniServer.Guid);
			if (this.m_serverStaleStatusAlert == null)
			{
				this.m_serverStaleStatusAlert = new ServerLevelDatabaseStaleStatusAlert(serverValidationResult.Identity, serverValidationResult.IdentityGuid, this.m_dbAlerts.StaleStatus);
			}
			this.m_serverStaleStatusAlert.RaiseAppropriateAlertIfNecessary(serverValidationResult);
			serverValidationResult = new ServerValidationResult("", Guid.Empty);
			if (this.m_potentialRedundancyAlertByRemoteServer == null)
			{
				this.m_potentialRedundancyAlertByRemoteServer = new PotentialRedundancyAlertByRemoteServer(this.m_dbAlerts.PotentialOneRedundantCopy);
			}
			this.m_potentialRedundancyAlertByRemoteServer.RaiseAppropriateAlertIfNecessary(serverValidationResult);
		}

		private void ProcessDatabase(IADDatabase db, IMonitoringADConfig adConfig)
		{
			if (!DatabaseHealthMonitor.ShouldMonitorDatabase(db))
			{
				this.m_dbAlerts.ResetState(db.Guid);
				return;
			}
			this.RaiseDatabaseRedundancyAlertIfNecessary(db, adConfig);
			this.RaiseDatabaseAvailabilityAlertIfNecessary(db, adConfig);
			this.RaiseDatabasePotentialRedundancyAlertIfNecessary(db, adConfig);
		}

		private void RaiseDatabaseRedundancyAlertIfNecessary(IADDatabase db, IMonitoringADConfig adConfig)
		{
			DatabaseHealthMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "DatabaseHealthMonitor.RaiseDatabaseRedundancyAlertIfNecessary: DB '{0}': Starting DB-level redundancy checks.", db.Name);
			DatabaseRedundancyValidator databaseRedundancyValidator = new DatabaseRedundancyValidator(db, DatabaseHealthMonitor.NUM_HEALTHY_COPIES_MIN, this.m_statusLookup, adConfig, this.m_propertyUpdateTracker, false);
			IHealthValidationResult result = databaseRedundancyValidator.Run();
			this.m_dbAlerts.OneRedundantCopy.RaiseAppropriateAlertIfNecessary(result);
			this.m_dbAlerts.TwoCopy.RaiseAppropriateAlertIfNecessary(result);
			this.m_dbAlerts.StaleStatus.RaiseAppropriateAlertIfNecessary(result);
			if (adConfig.Dag.DatacenterActivationMode == DatacenterActivationModeOption.DagOnly)
			{
				this.m_dbAlerts.OneRedundantCopySite.RaiseAppropriateAlertIfNecessary(result);
				return;
			}
			DatabaseHealthMonitor.Tracer.TraceDebug<string, DatacenterActivationModeOption>((long)this.GetHashCode(), "DatabaseHealthMonitor.RaiseDatabaseRedundancyAlertIfNecessary: DB '{0}': Skipping raising site alerts because DatacenterActivationMode is '{1}'.", db.Name, adConfig.Dag.DatacenterActivationMode);
		}

		private void RaiseDatabasePotentialRedundancyAlertIfNecessary(IADDatabase db, IMonitoringADConfig adConfig)
		{
			if (this.stopWatch.Elapsed.TotalSeconds < (double)RegistryParameters.DatabaseHealthCheckDelayInRaisingDatabasePotentialRedundancyAlertDueToServiceStartInSec)
			{
				DatabaseHealthMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "DatabaseHealthMonitor.RaiseDatabasePotentialRedundancyAlertIfNecessary: Service just started, skip DB-level checks.", db.Name);
				return;
			}
			DatabaseHealthMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "DatabaseHealthMonitor.RaiseDatabasePotentialRedundancyAlertIfNecessary: DB '{0}': Starting DB-level checks.", db.Name);
			DatabaseReplicationNotStalledValidator databaseReplicationNotStalledValidator = new DatabaseReplicationNotStalledValidator(db, this.m_statusLookup, adConfig, this.m_propertyUpdateTracker, false);
			IHealthValidationResult healthValidationResult = databaseReplicationNotStalledValidator.Run();
			int databaseHealthCheckPotentialOneCopyTotalPassiveCopiesRequiredMin = RegistryParameters.DatabaseHealthCheckPotentialOneCopyTotalPassiveCopiesRequiredMin;
			if (!healthValidationResult.IsValidationSuccessful && healthValidationResult.TotalPassiveCopiesCount < databaseHealthCheckPotentialOneCopyTotalPassiveCopiesRequiredMin)
			{
				DatabaseHealthMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "DatabaseHealthMonitor.RaiseDatabasePotentialRedundancyAlertIfNecessary: DB '{0}': Skipping raising potential redundancy alert because not enough passive copies are found", db.Name);
				string text = string.Format("{0}-NotEnoughTotalPassiveCopies", healthValidationResult.Identity);
				ReplayCrimsonEvents.DatabaseLevelPotentialRedundancyCheckFailedButNotEnoughTotalPassiveCopies.LogPeriodic<string, int, int, int, int, string, Guid>(text, TimeSpan.FromMinutes(60.0), healthValidationResult.Identity, healthValidationResult.HealthyCopiesCount, healthValidationResult.HealthyPassiveCopiesCount, healthValidationResult.TotalPassiveCopiesCount, databaseHealthCheckPotentialOneCopyTotalPassiveCopiesRequiredMin, "", healthValidationResult.IdentityGuid);
				return;
			}
			this.m_dbAlerts.PotentialOneRedundantCopy.RaiseAppropriateAlertIfNecessary(healthValidationResult);
		}

		private void RaiseDatabaseAvailabilityAlertIfNecessary(IADDatabase db, IMonitoringADConfig adConfig)
		{
			DatabaseHealthMonitor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "DatabaseHealthMonitor.RaiseDatabaseAvailabilityAlertIfNecessary: DB '{0}': Starting DB-level availability checks.", db.Name);
			DatabaseAvailabilityValidator databaseAvailabilityValidator = new DatabaseAvailabilityValidator(db, DatabaseHealthMonitor.NUM_HEALTHY_COPIES_MIN, this.m_statusLookup, adConfig, this.m_propertyUpdateTracker, false);
			IHealthValidationResult result = databaseAvailabilityValidator.Run();
			this.m_dbAlerts.OneAvailableCopy.RaiseAppropriateAlertIfNecessary(result);
			if (adConfig.Dag.DatacenterActivationMode == DatacenterActivationModeOption.DagOnly)
			{
				this.m_dbAlerts.OneAvailableCopySite.RaiseAppropriateAlertIfNecessary(result);
				return;
			}
			DatabaseHealthMonitor.Tracer.TraceDebug<string, DatacenterActivationModeOption>((long)this.GetHashCode(), "DatabaseHealthMonitor.RaiseDatabaseAvailabilityAlertIfNecessary: DB '{0}': Skipping raising site alerts because DatacenterActivationMode is '{1}'.", db.Name, adConfig.Dag.DatacenterActivationMode);
		}

		public static bool ShouldMonitorDatabase(IADDatabase db)
		{
			if (db.Recovery)
			{
				DatabaseHealthMonitor.Tracer.TraceDebug<string>(0L, "ShouldMonitorDatabase: Skipping monitoring of database '{0}' because it is a Recovery DB.", db.Name);
				return false;
			}
			if (db.AutoDagExcludeFromMonitoring)
			{
				DatabaseHealthMonitor.Tracer.TraceDebug<string>(0L, "ShouldMonitorDatabase: Skipping monitoring of database '{0}' because it has been excluded from monitoring (AutoDagExcludeFromMonitoring = true).", db.Name);
				return false;
			}
			string databaseHealthCheckSkipDatabasesRegex = RegistryParameters.DatabaseHealthCheckSkipDatabasesRegex;
			if (!string.IsNullOrEmpty(databaseHealthCheckSkipDatabasesRegex) && Regex.IsMatch(db.Name, databaseHealthCheckSkipDatabasesRegex, RegexOptions.IgnoreCase))
			{
				DatabaseHealthMonitor.Tracer.TraceDebug<string, string>(0L, "ShouldMonitorDatabase: Skipping monitoring of database '{0}' because it has been excluded from monitoring since it matches the skip Regex:  {1}", db.Name, databaseHealthCheckSkipDatabasesRegex);
				return false;
			}
			return true;
		}

		public static readonly int NUM_HEALTHY_COPIES_MIN = RegistryParameters.DatabaseHealthCheckAtLeastNCopies;

		private IMonitoringADConfigProvider m_adConfigProvider;

		private ICopyStatusClientLookup m_statusLookup;

		private DatabaseLevelAlerts m_dbAlerts;

		private ServerLevelDatabaseRedundancyAlert m_serverRedundancyAlert;

		private ServerLevelDatabaseStaleStatusAlert m_serverStaleStatusAlert;

		private PotentialRedundancyAlertByRemoteServer m_potentialRedundancyAlertByRemoteServer;

		private PropertyUpdateTracker m_propertyUpdateTracker;

		private Stopwatch stopWatch;
	}
}
