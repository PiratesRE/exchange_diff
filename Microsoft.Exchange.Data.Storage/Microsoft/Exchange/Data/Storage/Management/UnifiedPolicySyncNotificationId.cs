using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class UnifiedPolicySyncNotificationId : ObjectId
	{
		public string IdValue { get; set; }

		public UnifiedPolicySyncNotificationId(string value)
		{
			this.IdValue = value;
		}

		public override byte[] GetBytes()
		{
			return null;
		}

		public override string ToString()
		{
			return this.IdValue;
		}
	}
}
