using System;

namespace Microsoft.Exchange.Cluster.Shared
{
	public class RequestInfo
	{
		public RequestInfo(OperationCategory operationCategory, OperationType operationType, string debugStr)
		{
			this.ClientRequestId = Guid.NewGuid();
			this.OperationCategory = operationCategory;
			this.OperationType = operationType;
			this.DebugStr = debugStr;
			this.InitiatedTime = DateTimeOffset.Now;
		}

		public Guid ClientRequestId { get; set; }

		public DateTimeOffset InitiatedTime { get; set; }

		public OperationCategory OperationCategory { get; set; }

		public OperationType OperationType { get; set; }

		public string DebugStr { get; set; }

		public bool IsCloseKeyRequest
		{
			get
			{
				return this.OperationCategory == OperationCategory.CloseKey;
			}
		}

		public bool IsGetBaseKeyRequest
		{
			get
			{
				return this.OperationCategory == OperationCategory.GetBaseKey;
			}
		}
	}
}
