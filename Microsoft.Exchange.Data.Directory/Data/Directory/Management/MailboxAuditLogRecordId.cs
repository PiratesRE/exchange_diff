using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class MailboxAuditLogRecordId : ObjectId
	{
		internal MailboxAuditLogRecordId(ObjectId storeId)
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
