using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[Serializable]
	public class PolicyConfigProviderTransientException : SyncAgentTransientException
	{
		public PolicyConfigProviderTransientException(string message) : this(message, null, false, SyncAgentErrorCode.Generic)
		{
		}

		public PolicyConfigProviderTransientException(string message, Exception innerException) : this(message, innerException, false, SyncAgentErrorCode.Generic)
		{
		}

		public PolicyConfigProviderTransientException(string message, bool isPerObjectException, SyncAgentErrorCode errorCode = SyncAgentErrorCode.Generic) : this(message, null, isPerObjectException, errorCode)
		{
		}

		public PolicyConfigProviderTransientException(string message, Exception innerException, bool isPerObjectException, SyncAgentErrorCode errorCode = SyncAgentErrorCode.Generic) : base(message, innerException, isPerObjectException, errorCode)
		{
		}

		protected PolicyConfigProviderTransientException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}
	}
}
