using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	[Serializable]
	public class FilteringServiceTimeoutException : FilteringServiceFailureException
	{
		public FilteringServiceTimeoutException(string message) : base(message)
		{
		}

		public FilteringServiceTimeoutException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected FilteringServiceTimeoutException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}
	}
}
