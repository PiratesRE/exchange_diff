using System;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public class ComplianceTaskTransientException : CompliancePolicyException
	{
		public ComplianceTaskTransientException(string message) : this(message, null)
		{
		}

		public ComplianceTaskTransientException(string message, Exception innerException) : this(message, innerException, UnifiedPolicyErrorCode.Unknown)
		{
		}

		public ComplianceTaskTransientException(string message, UnifiedPolicyErrorCode errorCode) : this(message, null, errorCode)
		{
		}

		public ComplianceTaskTransientException(string message, Exception innerException, UnifiedPolicyErrorCode errorCode) : base(message, innerException)
		{
			this.ErrorCode = errorCode;
		}

		public UnifiedPolicyErrorCode ErrorCode { get; private set; }
	}
}
