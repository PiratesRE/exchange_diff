using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	[Serializable]
	public class CompliancePolicyParserException : PolicyEvaluationPermanentException
	{
		public CompliancePolicyParserException(string message) : base(message, null)
		{
		}

		public CompliancePolicyParserException(string format, params object[] args) : base(string.Format(format, args), null)
		{
		}

		public CompliancePolicyParserException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected CompliancePolicyParserException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}
	}
}
