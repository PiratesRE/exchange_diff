using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	[Serializable]
	public class TransportRuleTransientException : TransportRuleException
	{
		public TransportRuleTransientException(string message) : this(message, null)
		{
		}

		public TransportRuleTransientException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected TransportRuleTransientException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}
	}
}
