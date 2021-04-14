using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	internal class DbCopyHealthInfo
	{
		public string DbIdentity { get; private set; }

		public Guid DbGuid { get; private set; }

		public string DbName { get; private set; }

		public AmServerName ServerName { get; private set; }

		public DateTime LastTouchedTime { get; set; }

		public DateTime LastCopyStatusTransitionTime { get; set; }

		public StateTransitionInfo CopyStatusRetrieved { get; private set; }

		public StateTransitionInfo CopyIsAvailable { get; private set; }

		public StateTransitionInfo CopyIsRedundant { get; private set; }

		public StateTransitionInfo CopyStatusHealthy { get; private set; }

		public StateTransitionInfo CopyStatusActive { get; private set; }

		public StateTransitionInfo CopyStatusMounted { get; private set; }

		public StateTransitionInfo CopyFoundInAD { get; private set; }

		public DbCopyHealthInfo(Guid dbGuid, string dbName, AmServerName serverName)
		{
			this.DbGuid = dbGuid;
			this.DbName = dbName;
			this.DbIdentity = dbGuid.ToString();
			this.ServerName = serverName;
			this.CopyIsAvailable = new StateTransitionInfo();
			this.CopyIsRedundant = new StateTransitionInfo();
			this.CopyStatusRetrieved = new StateTransitionInfo();
			this.CopyStatusHealthy = new StateTransitionInfo();
			this.CopyStatusActive = new StateTransitionInfo();
			this.CopyStatusMounted = new StateTransitionInfo();
			this.CopyFoundInAD = new StateTransitionInfo();
		}

		public bool IsAvailable()
		{
			return this.CopyFoundInAD.IsSuccess && this.CopyStatusRetrieved.IsSuccess && this.CopyIsAvailable.IsSuccess;
		}

		public bool IsRedundant()
		{
			return this.CopyFoundInAD.IsSuccess && this.CopyStatusRetrieved.IsSuccess && this.CopyIsRedundant.IsSuccess;
		}

		public DbCopyHealthInfoPersisted ConvertToSerializable()
		{
			return new DbCopyHealthInfoPersisted(this.DbGuid, this.ServerName.Fqdn)
			{
				LastTouchedTime = this.LastTouchedTime,
				CopyStatusRetrieved = this.CopyStatusRetrieved.ConvertToSerializable(),
				CopyIsAvailable = this.CopyIsAvailable.ConvertToSerializable(),
				CopyIsRedundant = this.CopyIsRedundant.ConvertToSerializable(),
				CopyStatusHealthy = this.CopyStatusHealthy.ConvertToSerializable(),
				LastCopyStatusTransitionTime = this.LastCopyStatusTransitionTime,
				CopyStatusActive = this.CopyStatusActive.ConvertToSerializable(),
				CopyStatusMounted = this.CopyStatusMounted.ConvertToSerializable(),
				CopyFoundInAD = this.CopyFoundInAD.ConvertToSerializable()
			};
		}

		public void InitializeFromSerializable(DbCopyHealthInfoPersisted dbcHip)
		{
			this.LastTouchedTime = dbcHip.LastTouchedTime;
			this.CopyStatusRetrieved = StateTransitionInfo.ConstructFromPersisted(dbcHip.CopyStatusRetrieved);
			this.CopyIsAvailable = StateTransitionInfo.ConstructFromPersisted(dbcHip.CopyIsAvailable);
			this.CopyIsRedundant = StateTransitionInfo.ConstructFromPersisted(dbcHip.CopyIsRedundant);
			this.CopyStatusHealthy = StateTransitionInfo.ConstructFromPersisted(dbcHip.CopyStatusHealthy);
			this.LastCopyStatusTransitionTime = dbcHip.LastCopyStatusTransitionTime;
			this.CopyStatusActive = StateTransitionInfo.ConstructFromPersisted(dbcHip.CopyStatusActive);
			this.CopyStatusMounted = StateTransitionInfo.ConstructFromPersisted(dbcHip.CopyStatusMounted);
			this.CopyFoundInAD = StateTransitionInfo.ConstructFromPersisted(dbcHip.CopyFoundInAD);
		}
	}
}
