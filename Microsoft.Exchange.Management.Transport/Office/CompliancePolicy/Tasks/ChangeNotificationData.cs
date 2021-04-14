using System;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public sealed class ChangeNotificationData
	{
		internal Guid Id { get; private set; }

		internal Guid ParentId { get; private set; }

		internal ConfigurationObjectType ObjectType { get; private set; }

		internal ChangeType ChangeType { get; private set; }

		internal PolicyVersion Version { get; private set; }

		internal Workload Workload { get; private set; }

		internal UnifiedPolicyErrorCode ErrorCode { get; set; }

		internal string ErrorMessage { get; set; }

		internal bool ShouldNotify { get; set; }

		internal bool UseFullSync { get; set; }

		internal ChangeNotificationData(Guid id, Guid parentId, ConfigurationObjectType objectType, ChangeType changeType, Workload workload, PolicyVersion version, UnifiedPolicyErrorCode errorCode = UnifiedPolicyErrorCode.Unknown, string errorMessage = "")
		{
			this.Id = id;
			this.ParentId = parentId;
			this.ObjectType = objectType;
			this.ChangeType = changeType;
			this.Workload = workload;
			this.Version = version;
			this.ErrorCode = errorCode;
			this.ErrorMessage = errorMessage;
			this.ShouldNotify = true;
			this.UseFullSync = false;
		}

		internal SyncChangeInfo CreateSyncChangeInfo(bool setObjectId)
		{
			return new SyncChangeInfo(this.ObjectType, this.ChangeType, this.Version, setObjectId ? new Guid?(this.Id) : null);
		}
	}
}
