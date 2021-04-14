using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Common
{
	[Serializable]
	internal class InboxRuleOperationException : LocalizedException
	{
		public InboxRuleOperationException(LocalizedString message) : base(message)
		{
		}

		public InboxRuleOperationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InboxRuleOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
