using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	[Serializable]
	public class TransportRuleTimeoutException : TransportRuleTransientException
	{
		public TransportRuleTimeoutException(string message) : this(message, null)
		{
		}

		public TransportRuleTimeoutException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected TransportRuleTimeoutException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}
	}
}
