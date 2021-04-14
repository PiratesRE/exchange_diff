using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public class ClassificationRuleStoreTransientException : ClassificationRuleStoreExceptionBase
	{
		public ClassificationRuleStoreTransientException(string message) : this(message, null, false)
		{
		}

		public ClassificationRuleStoreTransientException(string message, Exception innerException) : this(message, innerException, false)
		{
		}

		public ClassificationRuleStoreTransientException(SerializationInfo serializationInfo, StreamingContext context) : this(serializationInfo, context, false)
		{
		}

		public ClassificationRuleStoreTransientException(string message, Exception innerException, bool isPerObjectException) : base(message, innerException, isPerObjectException)
		{
		}

		public ClassificationRuleStoreTransientException(SerializationInfo serializationInfo, StreamingContext context, bool isPerObjectException) : base(serializationInfo, context, isPerObjectException)
		{
		}
	}
}
