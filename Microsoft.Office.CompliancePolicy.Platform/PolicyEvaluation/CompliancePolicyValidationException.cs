using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	[Serializable]
	public class CompliancePolicyValidationException : PolicyEvaluationPermanentException
	{
		public CompliancePolicyValidationException(string message) : base(message, null)
		{
		}

		public CompliancePolicyValidationException(string format, params object[] args) : base(string.Format(format, args), null)
		{
		}

		public CompliancePolicyValidationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected CompliancePolicyValidationException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}
	}
}
