using System;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[Serializable]
	public class CompoundSyncObjectId : ObjectId
	{
		public CompoundSyncObjectId(SyncObjectId syncObjectId, ServiceInstanceId serviceInstanceId)
		{
			this.SyncObjectId = syncObjectId;
			this.ServiceInstanceId = serviceInstanceId;
		}

		public SyncObjectId SyncObjectId { get; private set; }

		public ServiceInstanceId ServiceInstanceId { get; private set; }

		public override byte[] GetBytes()
		{
			return Encoding.UTF8.GetBytes(this.ToString());
		}

		public override string ToString()
		{
			return string.Format("{0}\\{1}", this.ServiceInstanceId, this.SyncObjectId);
		}
	}
}
