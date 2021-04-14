using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	internal class PredicateOrActionSequenceException : InvalidOperationException
	{
		internal PredicateOrActionSequenceException(string message = null, Exception innerException = null) : base(message, innerException)
		{
		}

		protected PredicateOrActionSequenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
