using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ProvisioningId : ObjectId, ISnapshotId
	{
		public ProvisioningId(Guid jobItemGuid, Guid jobGuid)
		{
			this.JobItemGuid = jobItemGuid;
			this.JobGuid = jobGuid;
		}

		public Guid JobItemGuid { get; private set; }

		public Guid JobGuid { get; private set; }

		public override byte[] GetBytes()
		{
			return this.JobItemGuid.ToByteArray().Concat(this.JobGuid.ToByteArray()).ToArray<byte>();
		}

		public override bool Equals(object obj)
		{
			ProvisioningId provisioningId = obj as ProvisioningId;
			return provisioningId != null && this.JobItemGuid == provisioningId.JobItemGuid && this.JobGuid == provisioningId.JobGuid;
		}

		public override int GetHashCode()
		{
			return this.JobItemGuid.GetHashCode() / 2 + this.JobGuid.GetHashCode() / 2;
		}
	}
}
