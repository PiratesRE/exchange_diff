using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public class PolicyEvaluationExceptionBase : CompliancePolicyException
	{
		public PolicyEvaluationExceptionBase(string message) : this(message, null, false)
		{
		}

		public PolicyEvaluationExceptionBase(string message, Exception innerException) : this(message, innerException, false)
		{
		}

		public PolicyEvaluationExceptionBase(SerializationInfo serializationInfo, StreamingContext context) : this(serializationInfo, context, false)
		{
		}

		public PolicyEvaluationExceptionBase(string message, Exception innerException, bool isPerObjectException) : base(message, innerException)
		{
			this.IsPerObjectException = isPerObjectException;
		}

		public PolicyEvaluationExceptionBase(SerializationInfo serializationInfo, StreamingContext context, bool isPerObjectException) : base(serializationInfo, context)
		{
			this.IsPerObjectException = isPerObjectException;
		}

		public bool IsPerObjectException { get; private set; }
	}
}
