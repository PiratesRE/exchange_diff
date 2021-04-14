using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public class SyncAgentTransientException : SyncAgentExceptionBase
	{
		public SyncAgentTransientException(string message) : this(message, false, SyncAgentErrorCode.Generic)
		{
		}

		public SyncAgentTransientException(string message, Exception innerException) : this(message, innerException, false, SyncAgentErrorCode.Generic)
		{
		}

		public SyncAgentTransientException(SerializationInfo serializationInfo, StreamingContext context) : this(serializationInfo, context, false, SyncAgentErrorCode.Generic)
		{
		}

		public SyncAgentTransientException(string message, bool isPerObjectException, SyncAgentErrorCode errorCode = SyncAgentErrorCode.Generic) : this(message, null, isPerObjectException, errorCode)
		{
		}

		public SyncAgentTransientException(string message, Exception innerException, bool isPerObjectException, SyncAgentErrorCode errorCode = SyncAgentErrorCode.Generic) : base(message, innerException, isPerObjectException, errorCode)
		{
		}

		public SyncAgentTransientException(SerializationInfo serializationInfo, StreamingContext context, bool isPerObjectException, SyncAgentErrorCode errorCode = SyncAgentErrorCode.Generic) : base(serializationInfo, context, isPerObjectException, errorCode)
		{
		}
	}
}
