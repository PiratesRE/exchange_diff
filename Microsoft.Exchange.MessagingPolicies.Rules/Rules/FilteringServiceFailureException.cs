using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	[Serializable]
	public class FilteringServiceFailureException : TransportRuleException
	{
		public FilteringServiceFailureException(string message) : base(message)
		{
		}

		public FilteringServiceFailureException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected FilteringServiceFailureException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}
	}
}
