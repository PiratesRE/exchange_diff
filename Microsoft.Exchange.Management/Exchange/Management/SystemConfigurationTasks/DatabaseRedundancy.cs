using System;
using Microsoft.Exchange.Cluster.Replay.Monitoring;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class DatabaseRedundancy : IConfigurable
	{
		internal DatabaseRedundancy(HealthInfoPersisted healthInfo, DbHealthInfoPersisted dbHealth, string serverContactedFqdn)
		{
			this.Identity = new ConfigObjectId(dbHealth.DbName);
			this.DbGuid = dbHealth.DbGuid;
			this.ServerContactedFqdn = serverContactedFqdn.ToUpperInvariant();
			this.HealthInfoCreateTime = DateTimeHelper.ParseIntoNullableLocalDateTimeIfPossible(healthInfo.CreateTimeUtcStr);
			this.HealthInfoLastUpdateTime = DateTimeHelper.ParseIntoNullableLocalDateTimeIfPossible(healthInfo.LastUpdateTimeUtcStr);
			this.DatabaseFoundInAD = TransitionInfo.ConstructFromRemoteSerializable(dbHealth.DbFoundInAD);
			this.IsDatabaseFoundInAD = this.DatabaseFoundInAD.IsSuccess;
			this.SkippedFromMonitoring = TransitionInfo.ConstructFromRemoteSerializable(dbHealth.SkippedFromMonitoring);
			this.AtLeast1RedundantCopy = TransitionInfo.ConstructFromRemoteSerializable(dbHealth.IsAtLeast1RedundantCopy);
			this.AtLeast2RedundantCopy = TransitionInfo.ConstructFromRemoteSerializable(dbHealth.IsAtLeast2RedundantCopy);
			this.AtLeast3RedundantCopy = TransitionInfo.ConstructFromRemoteSerializable(dbHealth.IsAtLeast3RedundantCopy);
			this.AtLeast4RedundantCopy = TransitionInfo.ConstructFromRemoteSerializable(dbHealth.IsAtLeast4RedundantCopy);
			this.AtLeast1AvailableCopy = TransitionInfo.ConstructFromRemoteSerializable(dbHealth.IsAtLeast1AvailableCopy);
			this.AtLeast2AvailableCopy = TransitionInfo.ConstructFromRemoteSerializable(dbHealth.IsAtLeast2AvailableCopy);
			this.AtLeast3AvailableCopy = TransitionInfo.ConstructFromRemoteSerializable(dbHealth.IsAtLeast3AvailableCopy);
			this.AtLeast4AvailableCopy = TransitionInfo.ConstructFromRemoteSerializable(dbHealth.IsAtLeast4AvailableCopy);
			this.DbCopies = new DatabaseCopyRedundancy[dbHealth.DbCopies.Count];
			for (int i = 0; i < dbHealth.DbCopies.Count; i++)
			{
				this.DbCopies[i] = new DatabaseCopyRedundancy(dbHealth, dbHealth.DbCopies[i]);
			}
			this.IsAtLeast2RedundantCopy = this.AtLeast2RedundantCopy.IsSuccess;
			this.IsAtLeast2AvailableCopy = this.AtLeast2AvailableCopy.IsSuccess;
			this.AtLeast2RedundantCopyTransitionTime = this.AtLeast2RedundantCopy.LastTransitionTime;
			this.AtLeast2AvailableCopyTransitionTime = this.AtLeast2AvailableCopy.LastTransitionTime;
		}

		public Guid DbGuid { get; private set; }

		public ObjectId Identity { get; private set; }

		public bool IsDatabaseFoundInAD { get; private set; }

		public bool IsAtLeast2RedundantCopy { get; private set; }

		public bool IsAtLeast2AvailableCopy { get; private set; }

		public DateTime? AtLeast2RedundantCopyTransitionTime { get; private set; }

		public DateTime? AtLeast2AvailableCopyTransitionTime { get; private set; }

		public string ServerContactedFqdn { get; private set; }

		public DateTime? HealthInfoCreateTime { get; private set; }

		public DateTime? HealthInfoLastUpdateTime { get; private set; }

		public TransitionInfo DatabaseFoundInAD { get; private set; }

		public TransitionInfo SkippedFromMonitoring { get; private set; }

		public TransitionInfo AtLeast1RedundantCopy { get; private set; }

		public TransitionInfo AtLeast2RedundantCopy { get; private set; }

		public TransitionInfo AtLeast3RedundantCopy { get; private set; }

		public TransitionInfo AtLeast4RedundantCopy { get; private set; }

		public TransitionInfo AtLeast1AvailableCopy { get; private set; }

		public TransitionInfo AtLeast2AvailableCopy { get; private set; }

		public TransitionInfo AtLeast3AvailableCopy { get; private set; }

		public TransitionInfo AtLeast4AvailableCopy { get; private set; }

		public DatabaseCopyRedundancy[] DbCopies { get; private set; }

		internal bool IsValid
		{
			get
			{
				return true;
			}
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return this.IsValid;
			}
		}

		internal ObjectState ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return this.ObjectState;
			}
		}

		public ValidationError[] Validate()
		{
			return new ValidationError[0];
		}

		public void CopyChangesFrom(IConfigurable source)
		{
			throw new NotImplementedException();
		}

		public void ResetChangeTracking()
		{
			throw new NotImplementedException();
		}
	}
}
