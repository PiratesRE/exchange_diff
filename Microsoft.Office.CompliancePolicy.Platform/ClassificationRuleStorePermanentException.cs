using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public class ClassificationRuleStorePermanentException : ClassificationRuleStoreExceptionBase
	{
		public ClassificationRuleStorePermanentException(string message) : this(message, null, false)
		{
		}

		public ClassificationRuleStorePermanentException(string message, Exception innerException) : this(message, innerException, false)
		{
		}

		public ClassificationRuleStorePermanentException(SerializationInfo serializationInfo, StreamingContext context) : this(serializationInfo, context, false)
		{
		}

		public ClassificationRuleStorePermanentException(string message, Exception innerException, bool isPerObjectException) : base(message, innerException, isPerObjectException)
		{
		}

		public ClassificationRuleStorePermanentException(SerializationInfo serializationInfo, StreamingContext context, bool isPerObjectException) : base(serializationInfo, context, isPerObjectException)
		{
		}
	}
}
