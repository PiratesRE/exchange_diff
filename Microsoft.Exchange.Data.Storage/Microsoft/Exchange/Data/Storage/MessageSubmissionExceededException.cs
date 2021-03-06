using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class MessageSubmissionExceededException : StoragePermanentException
	{
		public MessageSubmissionExceededException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MessageSubmissionExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
