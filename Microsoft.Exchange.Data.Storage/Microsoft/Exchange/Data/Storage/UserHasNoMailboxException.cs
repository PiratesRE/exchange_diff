using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class UserHasNoMailboxException : ObjectNotFoundException
	{
		public UserHasNoMailboxException() : base(ServerStrings.ADUserNoMailbox)
		{
		}

		public UserHasNoMailboxException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected UserHasNoMailboxException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
