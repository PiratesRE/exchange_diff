using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public class SyncAgentPermanentException : SyncAgentExceptionBase
	{
		public SyncAgentPermanentException(string message) : this(message, false, SyncAgentErrorCode.Generic)
		{
		}

		public SyncAgentPermanentException(string message, Exception innerException) : this(message, innerException, false, SyncAgentErrorCode.Generic)
		{
		}

		public SyncAgentPermanentException(SerializationInfo serializationInfo, StreamingContext context) : this(serializationInfo, context, false, SyncAgentErrorCode.Generic)
		{
		}

		public SyncAgentPermanentException(string message, bool isPerObjectException, SyncAgentErrorCode errorCode = SyncAgentErrorCode.Generic) : this(message, null, isPerObjectException, errorCode)
		{
		}

		public SyncAgentPermanentException(string message, Exception innerException, bool isPerObjectException, SyncAgentErrorCode errorCode = SyncAgentErrorCode.Generic) : base(message, innerException, isPerObjectException, errorCode)
		{
		}

		public SyncAgentPermanentException(SerializationInfo serializationInfo, StreamingContext context, bool isPerObjectException, SyncAgentErrorCode errorCode = SyncAgentErrorCode.Generic) : base(serializationInfo, context, isPerObjectException, errorCode)
		{
		}
	}
}
