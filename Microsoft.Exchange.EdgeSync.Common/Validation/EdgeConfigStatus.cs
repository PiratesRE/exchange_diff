using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	[Serializable]
	public class EdgeConfigStatus
	{
		public EdgeConfigStatus()
		{
			this.syncStatus = SyncStatus.Skipped;
			this.orgOnlyObjects = new MultiValuedProperty<ADObjectId>();
			this.edgeOnlyObjects = new MultiValuedProperty<ADObjectId>();
			this.conflictObjects = new MultiValuedProperty<ADObjectId>();
		}

		public SyncStatus SyncStatus
		{
			get
			{
				return this.syncStatus;
			}
			set
			{
				this.syncStatus = value;
			}
		}

		public uint SynchronizedObjects
		{
			get
			{
				return this.synchronizedObjects;
			}
			set
			{
				this.synchronizedObjects = value;
			}
		}

		public MultiValuedProperty<ADObjectId> OrgOnlyObjects
		{
			get
			{
				return this.orgOnlyObjects;
			}
		}

		public MultiValuedProperty<ADObjectId> EdgeOnlyObjects
		{
			get
			{
				return this.edgeOnlyObjects;
			}
		}

		public MultiValuedProperty<ADObjectId> ConflictObjects
		{
			get
			{
				return this.conflictObjects;
			}
		}

		public override string ToString()
		{
			return this.syncStatus.ToString();
		}

		private SyncStatus syncStatus;

		private uint synchronizedObjects;

		private MultiValuedProperty<ADObjectId> orgOnlyObjects;

		private MultiValuedProperty<ADObjectId> edgeOnlyObjects;

		private MultiValuedProperty<ADObjectId> conflictObjects;
	}
}
