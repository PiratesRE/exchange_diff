using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	[Serializable]
	public abstract class PolicySyncFaultBase
	{
		protected PolicySyncFaultBase(int errorCode, string message, string serverIdentifier, SyncCallerContext callerContext)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("message", message);
			ArgumentValidator.ThrowIfNullOrEmpty("serverIdentifier", serverIdentifier);
			this.ErrorCode = errorCode;
			this.Message = message;
			this.ServerIdentifier = serverIdentifier;
			this.CallerContext = callerContext;
		}

		[DataMember]
		public int ErrorCode { get; private set; }

		[DataMember]
		public string Message { get; private set; }

		[DataMember]
		public string ServerIdentifier { get; private set; }

		[DataMember]
		public bool IsPerObjectException { get; private set; }

		[DataMember]
		public SyncCallerContext CallerContext { get; private set; }
	}
}
