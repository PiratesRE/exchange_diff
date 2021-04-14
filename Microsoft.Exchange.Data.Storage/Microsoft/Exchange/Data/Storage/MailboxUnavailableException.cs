using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class MailboxUnavailableException : StoragePermanentException
	{
		public MailboxUnavailableException(LocalizedString message) : base(message)
		{
		}

		public MailboxUnavailableException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MailboxUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
