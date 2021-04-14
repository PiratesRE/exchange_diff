using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public class CompliancePolicyException : Exception
	{
		public CompliancePolicyException(string message) : this(message, null)
		{
		}

		public CompliancePolicyException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected CompliancePolicyException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}
	}
}
