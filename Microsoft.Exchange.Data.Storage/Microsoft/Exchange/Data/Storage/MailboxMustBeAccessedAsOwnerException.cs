using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class MailboxMustBeAccessedAsOwnerException : StoragePermanentException
	{
		public MailboxMustBeAccessedAsOwnerException(LocalizedString message) : base(message)
		{
		}

		protected MailboxMustBeAccessedAsOwnerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
