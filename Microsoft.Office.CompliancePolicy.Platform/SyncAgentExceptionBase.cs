using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public class SyncAgentExceptionBase : CompliancePolicyException
	{
		public SyncAgentExceptionBase(string message) : this(message, false, SyncAgentErrorCode.Generic)
		{
		}

		public SyncAgentExceptionBase(string message, Exception innerException) : this(message, innerException, false, SyncAgentErrorCode.Generic)
		{
		}

		public SyncAgentExceptionBase(SerializationInfo serializationInfo, StreamingContext context) : this(serializationInfo, context, false, SyncAgentErrorCode.Generic)
		{
		}

		public SyncAgentExceptionBase(string message, bool isPerObjectException, SyncAgentErrorCode errorCode) : this(message, null, isPerObjectException, errorCode)
		{
		}

		public SyncAgentExceptionBase(string message, Exception innerException, bool isPerObjectException, SyncAgentErrorCode errorCode) : base(message, innerException)
		{
			this.IsPerObjectException = isPerObjectException;
			this.ErrorCode = errorCode;
		}

		public SyncAgentExceptionBase(SerializationInfo serializationInfo, StreamingContext context, bool isPerObjectException, SyncAgentErrorCode errorCode) : base(serializationInfo, context)
		{
			this.IsPerObjectException = isPerObjectException;
			this.ErrorCode = errorCode;
		}

		public bool IsPerObjectException { get; private set; }

		public SyncAgentErrorCode ErrorCode { get; private set; }
	}
}
