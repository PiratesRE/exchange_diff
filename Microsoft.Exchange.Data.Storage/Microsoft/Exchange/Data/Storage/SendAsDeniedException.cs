using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class SendAsDeniedException : StoragePermanentException
	{
		public SendAsDeniedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected SendAsDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
