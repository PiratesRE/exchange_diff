using System;
using Microsoft.Exchange.Cluster.Replay.Monitoring;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class DatabaseCopyRedundancy
	{
		internal DatabaseCopyRedundancy(DbHealthInfoPersisted dbHealth, DbCopyHealthInfoPersisted dbchip)
		{
			this.DatabaseGuid = dbHealth.DbGuid;
			this.DatabaseName = dbHealth.DbName;
			this.ServerName = MachineName.GetNodeNameFromFqdn(dbchip.ServerFqdn).ToUpperInvariant();
			this.m_copyName = string.Format("{0}\\{1}", this.DatabaseName, this.ServerName);
			this.Identity = this.m_copyName;
			this.LastCopyStatusTransitionTime = DateTimeHelper.ToNullableLocalDateTime(dbchip.LastCopyStatusTransitionTime);
			this.CopyFoundInAD = TransitionInfo.ConstructFromRemoteSerializable(dbchip.CopyFoundInAD);
			this.CopyStatusRetrieved = TransitionInfo.ConstructFromRemoteSerializable(dbchip.CopyStatusRetrieved);
			this.CopyIsAvailable = TransitionInfo.ConstructFromRemoteSerializable(dbchip.CopyIsAvailable);
			this.CopyIsRedundant = TransitionInfo.ConstructFromRemoteSerializable(dbchip.CopyIsRedundant);
			this.CopyStatusHealthy = TransitionInfo.ConstructFromRemoteSerializable(dbchip.CopyStatusHealthy);
			this.CopyStatusActive = TransitionInfo.ConstructFromRemoteSerializable(dbchip.CopyStatusActive);
			this.CopyStatusMounted = TransitionInfo.ConstructFromRemoteSerializable(dbchip.CopyStatusMounted);
			this.IsCopyFoundInAD = this.CopyFoundInAD.IsSuccess;
			this.IsCopyAvailable = this.CopyIsAvailable.IsSuccess;
			this.IsCopyRedundant = this.CopyIsRedundant.IsSuccess;
		}

		public string Identity { get; private set; }

		public string ServerName { get; private set; }

		public string DatabaseName { get; private set; }

		public Guid DatabaseGuid { get; private set; }

		public bool IsCopyFoundInAD { get; private set; }

		public bool IsCopyRedundant { get; private set; }

		public bool IsCopyAvailable { get; private set; }

		public DateTime? LastCopyStatusTransitionTime { get; set; }

		public TransitionInfo CopyFoundInAD { get; set; }

		public TransitionInfo CopyStatusRetrieved { get; set; }

		public TransitionInfo CopyIsAvailable { get; set; }

		public TransitionInfo CopyIsRedundant { get; set; }

		public TransitionInfo CopyStatusHealthy { get; set; }

		public TransitionInfo CopyStatusActive { get; set; }

		public TransitionInfo CopyStatusMounted { get; set; }

		public override string ToString()
		{
			return this.m_copyName;
		}

		private readonly string m_copyName;
	}
}
