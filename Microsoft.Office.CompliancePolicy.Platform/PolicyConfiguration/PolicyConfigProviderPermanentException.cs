using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[Serializable]
	public class PolicyConfigProviderPermanentException : SyncAgentPermanentException
	{
		public PolicyConfigProviderPermanentException(string message) : this(message, null, false, SyncAgentErrorCode.Generic)
		{
		}

		public PolicyConfigProviderPermanentException(string message, Exception innerException) : this(message, innerException, false, SyncAgentErrorCode.Generic)
		{
		}

		public PolicyConfigProviderPermanentException(string message, bool isPerObjectException, SyncAgentErrorCode errorCode = SyncAgentErrorCode.Generic) : this(message, null, isPerObjectException, errorCode)
		{
		}

		public PolicyConfigProviderPermanentException(string message, Exception innerException, bool isPerObjectException, SyncAgentErrorCode errorCode = SyncAgentErrorCode.Generic) : base(message, innerException, isPerObjectException, errorCode)
		{
		}

		protected PolicyConfigProviderPermanentException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}
	}
}
