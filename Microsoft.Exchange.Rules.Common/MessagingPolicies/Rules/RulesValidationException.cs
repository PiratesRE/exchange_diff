using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	[Serializable]
	public class RulesValidationException : LocalizedException
	{
		public RulesValidationException(LocalizedString message) : base(message)
		{
		}

		public RulesValidationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected RulesValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
