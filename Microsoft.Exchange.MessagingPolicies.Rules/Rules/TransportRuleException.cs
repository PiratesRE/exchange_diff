using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	[Serializable]
	public class TransportRuleException : Exception
	{
		public TransportRuleException(string message) : this(message, null)
		{
		}

		public TransportRuleException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected TransportRuleException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}
	}
}
