using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class AuditLogSearchId : ObjectId
	{
		public AuditLogSearchId(Guid requestId)
		{
			this.Guid = requestId;
		}

		public Guid Guid { get; private set; }

		public override byte[] GetBytes()
		{
			return this.Guid.ToByteArray();
		}

		public override string ToString()
		{
			return this.Guid.ToString();
		}
	}
}
