using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ADConfig : IADConfig
	{
		public ADConfig(IMonitoringADConfigProvider cfgSource)
		{
			this.cfgSource = cfgSource;
			ReplayServerPerfmon.ADConfigRefreshCalls.RawValue = 0L;
			ReplayServerPerfmon.ADConfigRefreshCallsPerSec.RawValue = 0L;
			this.synchronizer = new SynchronizedAction(new Action(this.RunRefresh));
		}

		private IMonitoringADConfig GetCurrentConfig()
		{
			IMonitoringADConfig result = null;
			try
			{
				result = this.cfgSource.GetConfig(true);
			}
			catch (MonitoringADConfigException ex)
			{
				ADConfig.Tracer.TraceError<MonitoringADConfigException>((long)this.GetHashCode(), "GetCurrentConfig failed: {0}", ex);
				ReplayCrimsonEvents.ADConfigGetCurrentConfigFailed.LogPeriodic<string, string>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, ex.ToString(), Environment.StackTrace);
			}
			return result;
		}

		public IADDatabaseAvailabilityGroup GetLocalDag()
		{
			IMonitoringADConfig currentConfig = this.GetCurrentConfig();
			if (currentConfig == null)
			{
				return null;
			}
			return currentConfig.Dag;
		}

		public IADServer GetLocalServer()
		{
			IMonitoringADConfig currentConfig = this.GetCurrentConfig();
			if (currentConfig == null)
			{
				return null;
			}
			string localComputerFqdn = Dependencies.ManagementClassHelper.LocalComputerFqdn;
			new AmServerName(localComputerFqdn);
			return currentConfig.TargetMiniServer;
		}

		public IADServer GetServer(AmServerName serverName)
		{
			IMonitoringADConfig currentConfig = this.GetCurrentConfig();
			if (currentConfig == null)
			{
				return null;
			}
			IADServer iadserver = currentConfig.LookupMiniServerByName(serverName);
			if (iadserver == null)
			{
				iadserver = this.HandleMissingServer(serverName);
			}
			return iadserver;
		}

		public IADServer GetServer(string nodeOrFqdn)
		{
			if (string.IsNullOrEmpty(nodeOrFqdn))
			{
				return this.GetLocalServer();
			}
			return this.GetServer(new AmServerName(nodeOrFqdn));
		}

		private IADServer HandleMissingServer(AmServerName serverName)
		{
			ADConfig.Tracer.TraceError<string>((long)this.GetHashCode(), "Server {0} not found", serverName.Fqdn);
			IReplayAdObjectLookup replayAdObjectLookup = Dependencies.ReplayAdObjectLookup;
			IADServer iadserver = replayAdObjectLookup.ServerLookup.FindServerByFqdn(serverName.Fqdn);
			if (iadserver != null)
			{
				this.Refresh("HandleMissingServer");
			}
			return iadserver;
		}

		public IADDatabase GetDatabase(Guid dbGuid)
		{
			IMonitoringADConfig currentConfig = this.GetCurrentConfig();
			if (currentConfig == null)
			{
				return null;
			}
			IADDatabase result;
			if (currentConfig.DatabaseByGuidMap.TryGetValue(dbGuid, out result))
			{
				return result;
			}
			return this.HandleMissingDatabase(dbGuid);
		}

		private IADDatabase HandleMissingDatabase(Guid dbGuid)
		{
			ADConfig.Tracer.TraceError<Guid>((long)this.GetHashCode(), "Database {0} not found", dbGuid);
			IReplayAdObjectLookup replayAdObjectLookup = Dependencies.ReplayAdObjectLookup;
			IADDatabase iaddatabase = replayAdObjectLookup.DatabaseLookup.FindAdObjectByGuid(dbGuid);
			if (iaddatabase != null)
			{
				this.Refresh("HandleMissingDatabase");
			}
			return iaddatabase;
		}

		public IEnumerable<IADDatabase> GetDatabasesOnServer(AmServerName serverName)
		{
			IMonitoringADConfig currentConfig = this.GetCurrentConfig();
			if (currentConfig == null)
			{
				return null;
			}
			IEnumerable<IADDatabase> result = null;
			if (currentConfig.DatabaseMap.TryGetValue(serverName, out result))
			{
				return result;
			}
			return this.HandleMissingServerDatabases(serverName);
		}

		public IEnumerable<IADDatabase> HandleMissingServerDatabases(AmServerName serverName)
		{
			ADConfig.Tracer.TraceError<AmServerName>((long)this.GetHashCode(), "Databases for server {0} not found", serverName);
			IReplayAdObjectLookup replayAdObjectLookup = Dependencies.ReplayAdObjectLookup;
			IADServer iadserver = replayAdObjectLookup.ServerLookup.FindServerByFqdn(serverName.Fqdn);
			if (iadserver != null)
			{
				this.Refresh("HandleMissingServerDatabases");
				IMonitoringADConfig currentConfig = this.GetCurrentConfig();
				if (currentConfig == null)
				{
					return null;
				}
				IEnumerable<IADDatabase> result = null;
				if (currentConfig.DatabaseMap.TryGetValue(serverName, out result))
				{
					return result;
				}
			}
			return null;
		}

		public IEnumerable<IADDatabase> GetDatabasesOnServer(IADServer server)
		{
			AmServerName serverName = new AmServerName(server.Fqdn);
			return this.GetDatabasesOnServer(serverName);
		}

		public IEnumerable<IADDatabase> GetDatabasesOnLocalServer()
		{
			IMonitoringADConfig currentConfig = this.GetCurrentConfig();
			if (currentConfig == null)
			{
				return null;
			}
			return this.GetDatabasesOnServer(currentConfig.TargetServerName);
		}

		public IEnumerable<IADDatabase> GetDatabasesInLocalDag()
		{
			IMonitoringADConfig currentConfig = this.GetCurrentConfig();
			if (currentConfig == null)
			{
				return null;
			}
			return currentConfig.DatabaseByGuidMap.Values;
		}

		public void Refresh(string reason)
		{
			this.Refresh(reason, RegistryParameters.ADConfigRefreshDefaultTimeoutInSec);
		}

		public void Refresh(string reason, int timeoutInSec)
		{
			ADConfig.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Refresh: {0}", reason);
			this.synchronizer.TryAction(TimeSpan.FromSeconds((double)timeoutInSec));
		}

		private void RunRefresh()
		{
			MonitoringADConfigManager monitoringADConfigManager = this.cfgSource as MonitoringADConfigManager;
			if (monitoringADConfigManager != null)
			{
				monitoringADConfigManager.TryRefreshConfig();
			}
		}

		private IMonitoringADConfigProvider cfgSource;

		private static readonly Trace Tracer = ExTraceGlobals.ADCacheTracer;

		private SynchronizedAction synchronizer;
	}
}
