using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public class ClassificationRuleStoreExceptionBase : CompliancePolicyException
	{
		public ClassificationRuleStoreExceptionBase(string message) : this(message, null, false)
		{
		}

		public ClassificationRuleStoreExceptionBase(string message, Exception innerException) : this(message, innerException, false)
		{
		}

		public ClassificationRuleStoreExceptionBase(SerializationInfo serializationInfo, StreamingContext context) : this(serializationInfo, context, false)
		{
		}

		public ClassificationRuleStoreExceptionBase(string message, Exception innerException, bool isPerObjectException) : base(message, innerException)
		{
			this.IsPerObjectException = isPerObjectException;
		}

		public ClassificationRuleStoreExceptionBase(SerializationInfo serializationInfo, StreamingContext context, bool isPerObjectException) : base(serializationInfo, context)
		{
			this.IsPerObjectException = isPerObjectException;
		}

		public bool IsPerObjectException { get; private set; }
	}
}
