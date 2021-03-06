using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class MailboxInSiteFailoverException : MailboxInTransitException
	{
		public MailboxInSiteFailoverException(LocalizedString message) : base(message)
		{
		}

		public MailboxInSiteFailoverException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MailboxInSiteFailoverException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
