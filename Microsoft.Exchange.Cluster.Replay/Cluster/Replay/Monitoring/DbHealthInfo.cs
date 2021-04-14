using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	internal class DbHealthInfo
	{
		public Guid DbGuid { get; private set; }

		public string DbName { get; set; }

		public Dictionary<AmServerName, DbCopyHealthInfo> DbServerInfos { get; private set; }

		public StateTransitionInfo DbFoundInAD { get; private set; }

		public StateTransitionInfo SkippedFromMonitoring { get; private set; }

		public DbAvailabilityRedundancyInfo DbAvailabilityRedundancyInfo { get; private set; }

		public int RedundancyCount
		{
			get
			{
				return this.DbAvailabilityRedundancyInfo.RedundancyCount;
			}
		}

		public int AvailabilityCount
		{
			get
			{
				return this.DbAvailabilityRedundancyInfo.AvailabilityCount;
			}
		}

		public DbHealthInfo(Guid dbGuid, string dbName)
		{
			this.DbGuid = dbGuid;
			this.DbName = dbName;
			this.DbFoundInAD = new StateTransitionInfo();
			this.SkippedFromMonitoring = new StateTransitionInfo();
			this.DbAvailabilityRedundancyInfo = new DbAvailabilityRedundancyInfo(this);
			this.DbServerInfos = new Dictionary<AmServerName, DbCopyHealthInfo>(5);
		}

		public void UpdateAvailabilityRedundancyStates()
		{
			this.DbAvailabilityRedundancyInfo.Update();
		}

		public bool ContainsDbCopy(AmServerName serverName)
		{
			return this.DbServerInfos.ContainsKey(serverName);
		}

		public DbCopyHealthInfo GetDbCopy(AmServerName serverName)
		{
			return this.DbServerInfos[serverName];
		}

		public DbCopyHealthInfo GetOrAddDbCopy(AmServerName serverName)
		{
			DbCopyHealthInfo dbCopyHealthInfo;
			if (!this.ContainsDbCopy(serverName))
			{
				dbCopyHealthInfo = new DbCopyHealthInfo(this.DbGuid, this.DbName, serverName);
				this.DbServerInfos[serverName] = dbCopyHealthInfo;
			}
			else
			{
				dbCopyHealthInfo = this.DbServerInfos[serverName];
			}
			return dbCopyHealthInfo;
		}

		public DbCopyHealthInfo AddNewDbCopy(AmServerName serverName)
		{
			DbCopyHealthInfo dbCopyHealthInfo = new DbCopyHealthInfo(this.DbGuid, this.DbName, serverName);
			this.DbServerInfos[serverName] = dbCopyHealthInfo;
			return dbCopyHealthInfo;
		}

		public void RemoveDbCopy(AmServerName serverName)
		{
			this.DbServerInfos.Remove(serverName);
			this.UpdateAvailabilityRedundancyStates();
		}

		public DbHealthInfoPersisted ConvertToSerializable()
		{
			DbHealthInfoPersisted dbHealthInfoPersisted = new DbHealthInfoPersisted(this.DbGuid, this.DbName);
			dbHealthInfoPersisted.DbFoundInAD = this.DbFoundInAD.ConvertToSerializable();
			dbHealthInfoPersisted.SkippedFromMonitoring = this.SkippedFromMonitoring.ConvertToSerializable();
			foreach (DbCopyHealthInfo dbCopyHealthInfo in this.DbServerInfos.Values)
			{
				dbHealthInfoPersisted.DbCopies.Add(dbCopyHealthInfo.ConvertToSerializable());
			}
			DbAvailabilityRedundancyInfo dbAvailabilityRedundancyInfo = this.DbAvailabilityRedundancyInfo;
			dbHealthInfoPersisted.IsAtLeast1AvailableCopy = dbAvailabilityRedundancyInfo.AvailabilityInfo[1].ConvertToSerializable();
			dbHealthInfoPersisted.IsAtLeast2AvailableCopy = dbAvailabilityRedundancyInfo.AvailabilityInfo[2].ConvertToSerializable();
			dbHealthInfoPersisted.IsAtLeast3AvailableCopy = dbAvailabilityRedundancyInfo.AvailabilityInfo[3].ConvertToSerializable();
			dbHealthInfoPersisted.IsAtLeast4AvailableCopy = dbAvailabilityRedundancyInfo.AvailabilityInfo[4].ConvertToSerializable();
			dbHealthInfoPersisted.IsAtLeast1RedundantCopy = dbAvailabilityRedundancyInfo.RedundancyInfo[1].ConvertToSerializable();
			dbHealthInfoPersisted.IsAtLeast2RedundantCopy = dbAvailabilityRedundancyInfo.RedundancyInfo[2].ConvertToSerializable();
			dbHealthInfoPersisted.IsAtLeast3RedundantCopy = dbAvailabilityRedundancyInfo.RedundancyInfo[3].ConvertToSerializable();
			dbHealthInfoPersisted.IsAtLeast4RedundantCopy = dbAvailabilityRedundancyInfo.RedundancyInfo[4].ConvertToSerializable();
			return dbHealthInfoPersisted;
		}

		public void InitializeFromSerializable(DbHealthInfoPersisted dbHip)
		{
			this.DbFoundInAD = StateTransitionInfo.ConstructFromPersisted(dbHip.DbFoundInAD);
			this.SkippedFromMonitoring = StateTransitionInfo.ConstructFromPersisted(dbHip.SkippedFromMonitoring);
			if (dbHip.DbCopies != null)
			{
				foreach (DbCopyHealthInfoPersisted dbCopyHealthInfoPersisted in dbHip.DbCopies)
				{
					AmServerName serverName = new AmServerName(dbCopyHealthInfoPersisted.ServerFqdn);
					DbCopyHealthInfo dbCopyHealthInfo = this.AddNewDbCopy(serverName);
					dbCopyHealthInfo.InitializeFromSerializable(dbCopyHealthInfoPersisted);
				}
			}
			DbAvailabilityRedundancyInfo dbAvailabilityRedundancyInfo = this.DbAvailabilityRedundancyInfo;
			dbAvailabilityRedundancyInfo.AvailabilityInfo[1] = StateTransitionInfo.ConstructFromPersisted(dbHip.IsAtLeast1AvailableCopy);
			dbAvailabilityRedundancyInfo.AvailabilityInfo[2] = StateTransitionInfo.ConstructFromPersisted(dbHip.IsAtLeast2AvailableCopy);
			dbAvailabilityRedundancyInfo.AvailabilityInfo[3] = StateTransitionInfo.ConstructFromPersisted(dbHip.IsAtLeast3AvailableCopy);
			dbAvailabilityRedundancyInfo.AvailabilityInfo[4] = StateTransitionInfo.ConstructFromPersisted(dbHip.IsAtLeast4AvailableCopy);
			dbAvailabilityRedundancyInfo.RedundancyInfo[1] = StateTransitionInfo.ConstructFromPersisted(dbHip.IsAtLeast1RedundantCopy);
			dbAvailabilityRedundancyInfo.RedundancyInfo[2] = StateTransitionInfo.ConstructFromPersisted(dbHip.IsAtLeast2RedundantCopy);
			dbAvailabilityRedundancyInfo.RedundancyInfo[3] = StateTransitionInfo.ConstructFromPersisted(dbHip.IsAtLeast3RedundantCopy);
			dbAvailabilityRedundancyInfo.RedundancyInfo[4] = StateTransitionInfo.ConstructFromPersisted(dbHip.IsAtLeast4RedundantCopy);
		}
	}
}
