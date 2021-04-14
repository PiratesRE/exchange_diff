using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DatabaseCopyStatus
	{
		public DatabaseCopyStatus(DatabaseCopyStatusEntry statusEntry)
		{
			this.Name = statusEntry.Name;
			this.RawIdentity = statusEntry.Id.ObjectGuid.ToString();
			this.IsActive = statusEntry.ActiveCopy;
			this.StatusString = LocalizedDescriptionAttribute.FromEnum(typeof(CopyStatus), statusEntry.Status);
			this.CopyQueueLength = ((statusEntry.CopyQueueLength != null) ? statusEntry.CopyQueueLength.Value.ToString() : "0");
			this.ContentIndexStateString = LocalizedDescriptionAttribute.FromEnum(typeof(ContentIndexStatusType), statusEntry.ContentIndexState);
			this.SuspendComment = statusEntry.SuspendComment;
			this.CanSuspend = (statusEntry.Status == CopyStatus.Failed || statusEntry.Status == CopyStatus.Seeding || statusEntry.Status == CopyStatus.Healthy || statusEntry.Status == CopyStatus.Initializing || statusEntry.Status == CopyStatus.Resynchronizing || statusEntry.Status == CopyStatus.DisconnectedAndHealthy || statusEntry.Status == CopyStatus.DisconnectedAndResynchronizing);
			this.CanResume = (statusEntry.Status == CopyStatus.Suspended || statusEntry.Status == CopyStatus.FailedAndSuspended);
			this.CanRemove = (statusEntry.Status == CopyStatus.Failed || statusEntry.Status == CopyStatus.Seeding || statusEntry.Status == CopyStatus.Suspended || statusEntry.Status == CopyStatus.Healthy || statusEntry.Status == CopyStatus.Initializing || statusEntry.Status == CopyStatus.Resynchronizing || statusEntry.Status == CopyStatus.DisconnectedAndHealthy || statusEntry.Status == CopyStatus.FailedAndSuspended || statusEntry.Status == CopyStatus.DisconnectedAndResynchronizing || statusEntry.Status == CopyStatus.NonExchangeReplication || statusEntry.Status == CopyStatus.SeedingSource || statusEntry.Status == CopyStatus.Misconfigured);
			this.CanActivate = (statusEntry.Status == CopyStatus.Healthy || statusEntry.Status == CopyStatus.DisconnectedAndHealthy || statusEntry.Status == CopyStatus.DisconnectedAndResynchronizing || statusEntry.Status == CopyStatus.Initializing || statusEntry.Status == CopyStatus.Resynchronizing);
			this.CanUpdate = (statusEntry.Status == CopyStatus.Suspended || statusEntry.Status == CopyStatus.FailedAndSuspended);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			DatabaseCopyStatus databaseCopyStatus = obj as DatabaseCopyStatus;
			return databaseCopyStatus != null && (string.Equals(this.Name, databaseCopyStatus.Name) && string.Equals(this.RawIdentity, databaseCopyStatus.RawIdentity) && string.Equals(this.StatusString, databaseCopyStatus.StatusString) && string.Equals(this.CopyQueueLength, databaseCopyStatus.CopyQueueLength) && string.Equals(this.ContentIndexStateString, databaseCopyStatus.ContentIndexStateString) && string.Equals(this.SuspendComment, databaseCopyStatus.SuspendComment) && this.IsActive == databaseCopyStatus.IsActive && this.CanSuspend == databaseCopyStatus.CanSuspend && this.CanResume == databaseCopyStatus.CanResume && this.CanRemove == databaseCopyStatus.CanRemove && this.CanActivate == databaseCopyStatus.CanActivate) && this.CanUpdate == databaseCopyStatus.CanUpdate;
		}

		public override int GetHashCode()
		{
			return this.RawIdentity.GetHashCode();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string RawIdentity { get; set; }

		[DataMember]
		public bool IsActive { get; set; }

		[DataMember]
		public string StatusString { get; set; }

		[DataMember]
		public string CopyQueueLength { get; set; }

		[DataMember]
		public string ContentIndexStateString { get; set; }

		[DataMember]
		public string SuspendComment { get; set; }

		[DataMember]
		public bool CanSuspend { get; set; }

		[DataMember]
		public bool CanResume { get; set; }

		[DataMember]
		public bool CanRemove { get; set; }

		[DataMember]
		public bool CanActivate { get; set; }

		[DataMember]
		public bool CanUpdate { get; set; }
	}
}
