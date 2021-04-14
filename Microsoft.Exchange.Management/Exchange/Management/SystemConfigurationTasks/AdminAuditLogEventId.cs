using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public class AdminAuditLogEventId : ObjectId
	{
		internal AdminAuditLogEventId(ObjectId storeId)
		{
			if (storeId == null)
			{
				throw new ArgumentNullException("storeId");
			}
			this.storeId = storeId;
		}

		public override byte[] GetBytes()
		{
			return this.storeId.GetBytes();
		}

		public override string ToString()
		{
			return this.storeId.ToString();
		}

		private ObjectId storeId;
	}
}
