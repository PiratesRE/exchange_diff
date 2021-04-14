using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	[Serializable]
	public class TransportRulePermanentException : TransportRuleException
	{
		public TransportRulePermanentException(string message) : this(message, null)
		{
		}

		public TransportRulePermanentException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected TransportRulePermanentException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}
	}
}
