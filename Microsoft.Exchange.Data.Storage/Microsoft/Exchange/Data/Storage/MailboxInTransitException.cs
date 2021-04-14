using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class MailboxInTransitException : ConnectionFailedTransientException
	{
		public MailboxInTransitException(LocalizedString message) : base(message)
		{
		}

		public MailboxInTransitException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MailboxInTransitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
