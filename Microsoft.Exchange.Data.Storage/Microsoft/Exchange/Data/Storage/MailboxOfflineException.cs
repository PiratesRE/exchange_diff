using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class MailboxOfflineException : ConnectionFailedTransientException
	{
		public MailboxOfflineException(LocalizedString message) : base(message)
		{
		}

		public MailboxOfflineException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MailboxOfflineException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
