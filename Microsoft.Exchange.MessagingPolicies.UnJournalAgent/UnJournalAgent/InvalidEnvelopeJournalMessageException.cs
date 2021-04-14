using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidEnvelopeJournalMessageException : LocalizedException
	{
		public InvalidEnvelopeJournalMessageException(LocalizedString message) : base(message)
		{
		}

		public InvalidEnvelopeJournalMessageException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidEnvelopeJournalMessageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
