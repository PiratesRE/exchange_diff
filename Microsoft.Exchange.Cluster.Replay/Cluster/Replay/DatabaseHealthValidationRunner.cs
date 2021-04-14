using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	public class DatabaseHealthValidationRunner
	{
		private MonitoringADConfig ADConfig { get; set; }

		private ICopyStatusClientLookup CopyStatusLookup { get; set; }

		public DatabaseHealthValidationRunner(string serverName)
		{
			this.m_serverName = serverName;
		}

		public void Initialize()
		{
			if (Interlocked.CompareExchange(ref this.m_fInitialized, 1, 0) == 1)
			{
				return;
			}
			this.ADConfig = MonitoringADConfig.GetConfig(new AmServerName(this.m_serverName, true), Dependencies.ReplayAdObjectLookup, Dependencies.ReplayAdObjectLookupPartiallyConsistent, ADSessionFactory.CreateIgnoreInvalidRootOrgSession(true), ADSessionFactory.CreatePartiallyConsistentRootOrgSession(true), null);
			ActiveManager noncachingActiveManagerInstance = ActiveManager.GetNoncachingActiveManagerInstance();
			AmMultiNodeCopyStatusFetcher amMultiNodeCopyStatusFetcher = new AmMultiNodeCopyStatusFetcher(this.ADConfig.AmServerNames, this.ADConfig.DatabaseMap, RpcGetDatabaseCopyStatusFlags2.None, noncachingActiveManagerInstance, false);
			Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>> status = amMultiNodeCopyStatusFetcher.GetStatus();
			CopyStatusClientLookupTable copyStatusClientLookupTable = new CopyStatusClientLookupTable();
			copyStatusClientLookupTable.UpdateCopyStatusCachedEntries(status);
			this.CopyStatusLookup = new CopyStatusClientLookup(copyStatusClientLookupTable, null, noncachingActiveManagerInstance);
		}

		public IEnumerable<IHealthValidationResultMinimal> RunDatabaseRedundancyChecks(Guid? dbGuid = null, bool skipUnMonitoredDatabase = true)
		{
			return this.RunChecks<DatabaseRedundancyValidator>((IADDatabase db) => new DatabaseRedundancyValidator(db, RegistryParameters.DatabaseHealthCheckAtLeastNCopies, this.CopyStatusLookup, this.ADConfig, null, true), dbGuid, skipUnMonitoredDatabase);
		}

		public IEnumerable<IHealthValidationResultMinimal> RunDatabaseRedundancyChecksForCopyRemoval(bool ignoreActivationDisfavored, Guid? dbGuid = null, bool ignoreMaintenanceChecks = false, bool ignoreTooManyActivesCheck = false, int atLeastNCopies = -1)
		{
			if (atLeastNCopies == -1)
			{
				atLeastNCopies = RegistryParameters.DatabaseHealthCheckAtLeastNCopies;
			}
			return this.RunChecks<DatabaseRedundancyValidator>((IADDatabase db) => new DatabaseRedundancyValidator(db, atLeastNCopies, this.CopyStatusLookup, this.ADConfig, null, true, ignoreActivationDisfavored, ignoreMaintenanceChecks, ignoreTooManyActivesCheck, true), dbGuid, true);
		}

		public IEnumerable<IHealthValidationResultMinimal> RunDatabaseAvailabilityChecks()
		{
			return this.RunChecks<DatabaseAvailabilityValidator>((IADDatabase db) => new DatabaseAvailabilityValidator(db, RegistryParameters.DatabaseHealthCheckAtLeastNCopies, this.CopyStatusLookup, this.ADConfig, null, true), null, true);
		}

		private IEnumerable<IHealthValidationResultMinimal> RunChecks<TValidator>(Func<IADDatabase, TValidator> createValidatorFunc, Guid? dbGuid = null, bool skipUnMonitoredDatabase = true) where TValidator : DatabaseValidatorBase
		{
			IEnumerable<IADDatabase> databases = this.ADConfig.DatabasesIncludingMisconfiguredMap[this.ADConfig.TargetServerName];
			foreach (IADDatabase db in databases)
			{
				if ((!skipUnMonitoredDatabase || DatabaseHealthMonitor.ShouldMonitorDatabase(db)) && (dbGuid == null || db.Guid.Equals(dbGuid.Value)))
				{
					TValidator validator = createValidatorFunc(db);
					IHealthValidationResultMinimal result = validator.Run();
					yield return result;
				}
			}
			yield break;
		}

		private readonly string m_serverName;

		private int m_fInitialized;
	}
}
