using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public class PolicyEvaluationTransientException : PolicyEvaluationExceptionBase
	{
		public PolicyEvaluationTransientException(string message) : this(message, null, false)
		{
		}

		public PolicyEvaluationTransientException(string message, Exception innerException) : this(message, innerException, false)
		{
		}

		public PolicyEvaluationTransientException(SerializationInfo serializationInfo, StreamingContext context) : this(serializationInfo, context, false)
		{
		}

		public PolicyEvaluationTransientException(string message, Exception innerException, bool isPerObjectException) : base(message, innerException, isPerObjectException)
		{
		}

		public PolicyEvaluationTransientException(SerializationInfo serializationInfo, StreamingContext context, bool isPerObjectException) : base(serializationInfo, context, isPerObjectException)
		{
		}
	}
}
