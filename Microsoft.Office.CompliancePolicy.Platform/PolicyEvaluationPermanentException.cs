using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public class PolicyEvaluationPermanentException : PolicyEvaluationExceptionBase
	{
		public PolicyEvaluationPermanentException(string message) : this(message, null, false)
		{
		}

		public PolicyEvaluationPermanentException(string message, Exception innerException) : this(message, innerException, false)
		{
		}

		public PolicyEvaluationPermanentException(SerializationInfo serializationInfo, StreamingContext context) : this(serializationInfo, context, false)
		{
		}

		public PolicyEvaluationPermanentException(string message, Exception innerException, bool isPerObjectException) : base(message, innerException, isPerObjectException)
		{
		}

		public PolicyEvaluationPermanentException(SerializationInfo serializationInfo, StreamingContext context, bool isPerObjectException) : base(serializationInfo, context, isPerObjectException)
		{
		}
	}
}
