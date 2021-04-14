using System;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public class ComplianceTaskPermanentException : CompliancePolicyException
	{
		public ComplianceTaskPermanentException(string message) : this(message, null)
		{
		}

		public ComplianceTaskPermanentException(string message, Exception innerException) : this(message, innerException, UnifiedPolicyErrorCode.Unknown)
		{
		}

		public ComplianceTaskPermanentException(string message, UnifiedPolicyErrorCode errorCode) : this(message, null, errorCode)
		{
		}

		public ComplianceTaskPermanentException(string message, Exception innerException, UnifiedPolicyErrorCode errorCode) : base(message, innerException)
		{
			this.ErrorCode = errorCode;
		}

		public UnifiedPolicyErrorCode ErrorCode { get; private set; }
	}
}
